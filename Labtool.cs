using Binarysharp.MemoryManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace rev2_LabTool
{
	class Labtool
	{
		private readonly string _ggprocname = ConfigurationManager.AppSettings.Get("GGProcessName");
		private MemorySharp _memorySharp;

		#region Animations
		private readonly IntPtr _p1AnimStringPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1AnimStringPtr"), 16));
		private readonly IntPtr _p2AnimStringPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2AnimStringPtr"), 16));
		private readonly int _AnimStringPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("AnimStringPtrOffset"), 16);
		private readonly IntPtr _frameCountOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("FrameCountOffset"), 16));

		private HashSet<string> idleAnimHash;
		private HashSet<string> blockingAnimHash;

		string[] idleAnimArray =
		{
			"CmnActStand",
			"CmnActCrouch",
			"CmnActJump",
			"CmnActStand2Crouch",
			"CmnActFWalk",
			"CmnActFWalk",
			"CmnActBWalk",
			"CmnActCrouchGuardEnd",
			"CmnActMidGuardEnd",
			"CmnActHighGuardEnd",
			"CmnActAirGuardEnd"
		};

		string[] blockingAnimArray =
		{
			"CmnActCrouchGuardLoop",
			"CmnActMidGuardLoop",
			"CmnActHighGuardLoop",
			"CmnActAirGuardLoop"
		};
		#endregion

		public readonly int _menuLabel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MenuLabel"), 16);

		public string p1CurrentAnim;
		public string p2CurrentAnim;
		public bool p1isIdle = true;
		public bool p2isIdle = true;
		public int prevFrame = 0;
		public int currFrame = 0;

		public void AttachToProcess()
		{
			Process process = System.Diagnostics.Process.GetProcessesByName(_ggprocname).FirstOrDefault();

			if (process == null)
			{
				throw new Exception("GGxrd is not open.");
			}

			idleAnimHash = new HashSet<string>(idleAnimArray);
			blockingAnimHash = new HashSet<string>(blockingAnimArray);
			_memorySharp = new MemorySharp(process);
		}

		public string ReadMenuLabel()
		{
			var str = _memorySharp.ReadString(new IntPtr(0x0) + _menuLabel, false, 32);

			return str;
		}

		public string ReadAnimationString(int player)
		{
			if (player == 1)
			{
				try
				{
					var ptr = _memorySharp[_p1AnimStringPtr].Read<IntPtr>();
					var str = _memorySharp.ReadString(ptr + _AnimStringPtrOffset, false, 32);
					return str;
				}
				catch (System.ArgumentException ex)
				{
					Dispose();
				}
			}

			if (player == 2)
			{
				try
				{
					var ptr = _memorySharp[_p2AnimStringPtr].Read<IntPtr>();
					var str = _memorySharp.ReadString(ptr + _AnimStringPtrOffset, false, 32);
					return str;
				}
				catch (System.ArgumentException ex)
				{
					Dispose();
				}
			}

			return string.Empty;
		}

		public int FrameCount()
		{
			try
			{
				return _memorySharp.Read<int>(_frameCountOffset); //exception: handle not valid or closed

			}
			catch (System.ArgumentException exception)
			{
				Dispose();
				return 0;
			}
		}

		public bool IsIdle(string anim)
		{
			if (idleAnimHash.Contains(anim))
				return true;

			//TODO: preparing to block animations return true
			return false;
		}

		public bool IsBlocking(string anim)
		{
			if (blockingAnimHash.Contains(anim))
				return true;
			return false;
		}

		public bool blockstring = false;
		public int frameAdvantage;
		public bool updateFA;
		public bool updateGap = false;
		public int idleCount;
		public int rememberGap;
		public void updateFrameInfo()
		{

			do
			{
				currFrame = FrameCount();
			}
			while (currFrame == prevFrame);

			p1CurrentAnim = ReadAnimationString(1);
			p2CurrentAnim = ReadAnimationString(2);
			p1isIdle = IsIdle(p1CurrentAnim);
			p2isIdle = IsIdle(p2CurrentAnim);



			updateFA = false;
			if (!p1isIdle && !p2isIdle)
			{
				frameAdvantage = 0;
				blockstring = true;
			}
			if ((p1isIdle || p2isIdle) && blockstring)
			{
				if (p1isIdle && p2isIdle)
				{
					blockstring = false;
					updateFA = true;
				}
				else if (p1isIdle)
					++frameAdvantage;
				else if (p2isIdle)
					--frameAdvantage;
			}


			if (IsBlocking(p2CurrentAnim))
			{

				if (idleCount != -1)
				{
					++idleCount;

					if (idleCount <= 30)
					{
						rememberGap = idleCount;
						updateGap = true;
					}
					idleCount = -1;
				}
			}
			else
			{
				++idleCount;
			}

			prevFrame = currFrame;
		}

		public void Dispose()
		{
			_memorySharp?.Dispose();
		}
	}
}
