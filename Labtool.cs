using Binarysharp.MemoryManagement;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

class Labtool
{
	public Player player1 = new Player();
	public Player player2 = new Player();

	private readonly string _ggprocname = ConfigurationManager.AppSettings.Get("GGProcessName");
	private MemorySharp _memorySharp;

	#region Pointers
	private readonly IntPtr _frameCountOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("FrameCountOffset"), 16));
	private readonly IntPtr _p1CharIndex = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("Player1CharIndex"), 16));
	private readonly IntPtr _p2CharIndex = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("Player2CharIndex"), 16));
	#endregion

	#region Offsets
	private readonly int _HPOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HPOffset"), 16);
	private readonly int _MeterOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MeterOffset"), 16);
	private readonly int _RISCOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("RISCOffset"), 16);
	private readonly int _PositionOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PositionOffset"), 16);
	private readonly int _AnimStringOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("AnimStringOffset"), 16);
	private readonly int _DizzyOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DizzyOffset"), 16);
	#endregion


	public void AttachToProcess()
	{
		Process process = Process.GetProcessesByName(_ggprocname).FirstOrDefault();

		if (process == null)
		{
			throw new Exception("GGxrd is not open.");
		}

		_memorySharp = new MemorySharp(process);

		Player.setHashSets();
	}

	public int ReadStaticInt(int playerNumber)
	{
		try
		{
			if (playerNumber == 1)
			{
				return _memorySharp.Read<int>(_p1CharIndex);
			}
			else if (playerNumber == 2)
			{
				return _memorySharp.Read<int>(_p2CharIndex);
			}
			else
			{
				return 0;
			}
		}
		catch (System.ArgumentException exception)
		{
			Dispose();
			return 0;
		}
	}

	public int ReadInfoInt(ref Player player, int offset)
	{
		try
		{
			IntPtr ptr = _memorySharp[player.playerPtr].Read<IntPtr>(); //player.playerPtr is not updated
			int integer = _memorySharp.Read<int>(ptr + offset, false);
			return integer;
		}
		catch (ArgumentException)
		{
			Dispose();
		}

		return -1;
	}

	public float ReadInfoFloat(ref Player player, int offset)
	{
		try
		{
			IntPtr ptr = _memorySharp[player.playerPtr].Read<IntPtr>();//
			float floatnumber = _memorySharp.Read<float>(ptr + offset, false);
			return floatnumber;
		}
		catch (ArgumentException)
		{
			Dispose();
		}

		return -1;
	}

	public string ReadAnimationString(ref Player player)
	{
		try
		{
			var ptr = _memorySharp[player.playerPtr].Read<IntPtr>();
			string str = _memorySharp.ReadString(ptr + _AnimStringOffset, false, 32);
			return str;
		}
		catch (ArgumentException)
		{
			Dispose();
		}

		return string.Empty;
	}


	public int FrameCount()
	{
		try
		{
			return _memorySharp.Read<int>(_frameCountOffset); //exception: handle not valid or closed, happens when closing match
		}
		catch (System.ArgumentException exception)
		{
			Dispose();
			return 0;
		}
	}

	public class Framedata
	{
		public bool blockstring = false;
		public int frameAdvantage;
		public bool updateFA;
		public bool updateGap = false;
		public int idleCount;
		public int rememberGap;
	}

	public Framedata f1 = new Framedata();
	public Framedata f2 = new Framedata();
	public void playerFrameData(Framedata f, ref Player p1, ref Player p2)
	{
		f.updateFA = false;
		if (!p1.IsIdle() && !p2.IsIdle())
		{
			f.frameAdvantage = 0;
			f.blockstring = true;
		}
		if ((p1.IsIdle() || p2.IsIdle()) && f.blockstring)
		{
			if (p1.IsIdle() && p2.IsIdle())
			{
				f.blockstring = false;
				f.updateFA = true;
			}
			else if (p1.IsIdle())
				++f.frameAdvantage;
			else if (p2.IsIdle())
				--f.frameAdvantage;
		}


		if (p2.IsBlocking())
		{
			if (f.idleCount != -1)
			{
				++f.idleCount;

				if (f.idleCount <= 30)
				{
					f.rememberGap = f.idleCount;
					f.updateGap = true;
				}
				f.idleCount = -1;
			}
		}
		else
		{
			++f.idleCount;
		}

		prevFrame = currFrame;
	}

	public int prevFrame = 0;
	public int currFrame = 0;
	public void updateFrameInfo()
	{
		do
		{
			currFrame = FrameCount();
		}
		while (currFrame == prevFrame);

		//pass the refs, not the whole class
		player1.characterIndex = ReadStaticInt(player1.playerNumber);
		player2.characterIndex = ReadStaticInt(player2.playerNumber);
		player1.assignPlayerPtr("Player1Ptr"); //why does it fail ?
		player2.assignPlayerPtr("Player2Ptr");
		player1.HP = ReadInfoInt(ref player1, _HPOffset);
		player2.HP = ReadInfoInt(ref player2, _HPOffset);
		player1.meter = ReadInfoInt(ref player1, _MeterOffset);
		player2.meter = ReadInfoInt(ref player2, _MeterOffset);
		player1.RISC = ReadInfoInt(ref player1, _RISCOffset);
		player2.RISC = ReadInfoInt(ref player2, _RISCOffset);
		player1.dizzy = ReadInfoInt(ref player1, _DizzyOffset);
		player2.dizzy = ReadInfoInt(ref player2, _DizzyOffset);
		player1.currentAnim = ReadAnimationString(ref player1);
		player2.currentAnim = ReadAnimationString(ref player2);
		player1.xPos = ReadInfoFloat(ref player1, _PositionOffset);
		player2.xPos = ReadInfoFloat(ref player2, _PositionOffset);

		playerFrameData(f1, ref player1, ref player2);
		playerFrameData(f2, ref player2, ref player1);
	}

	public void Dispose()
	{
		_memorySharp?.Dispose();
	}
}

/*
 * TODO
 * - Fix character pointers
 * - Find real positions
 * - Fix display when playing as p2 (most probably, find real positions and/or which side they are facing)
 * - Find difference between "idle" blocking and actual blocking. Blockstop found
 * - Add guts
 */
