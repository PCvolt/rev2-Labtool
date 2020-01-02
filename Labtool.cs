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

		private readonly string standAnim = "CmnActStand";
		private readonly string crouchAnim = "CmnActCrouch";
		private readonly string jumpAnim = "CmnActJump";
		private readonly string stand2crouchAnim = "CmnActStand2Crouch";
		private readonly string crouch2standAnim = "CmnActCrouch2Stand";
		private readonly string fwalkAnim = "CmnActFWalk";
		private readonly string bwalkAnim = "CmnActBWalk";

		private readonly string lowBlockLoop = "CmnActCrouchGuardLoop";
		private readonly string midBlockLoop = "CmnActMidGuardLoop";
		private readonly string highBlockLoop = "CmnActHighGuardLoop";
		private readonly string airBlockLoop = "CmnActAirGuardLoop";

		private readonly string lowBlockEnd = "CmnActCrouchGuardEnd";
		private readonly string midBlockEnd = "CmnActMidGuardEnd";
		private readonly string highBlockEnd = "CmnActHighGuardEnd";
		private readonly string airBlockEnd = "CmnActAirGuardEnd";
		#endregion

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

			_memorySharp = new MemorySharp(process);
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
		if (anim.Equals(standAnim) || anim.Equals(crouchAnim) || anim.Equals(jumpAnim) || anim.Equals(crouch2standAnim) || anim.Equals(stand2crouchAnim) || anim.Equals(fwalkAnim) || anim.Equals(bwalkAnim))
			return true;

		if (anim.Equals(lowBlockEnd) || anim.Equals(midBlockEnd) || anim.Equals(highBlockEnd) || anim.Equals(airBlockEnd))
			return true;

		//TODO: preparing to block animations return true
		return false;
	}

	public bool IsBlocking(string anim)
	{
		if (anim.Equals(lowBlockLoop) || anim.Equals(midBlockLoop) || anim.Equals(highBlockLoop) || anim.Equals(airBlockLoop))
			return true;
		return false;
	}

	public bool blockstring = false;
	public int frameAdvantage;
	public bool updateFA;
	public bool updateGap = false;
	public int idleCount;
	public int rememberGap;
	public void frameAdvantageLoop() //rename
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
