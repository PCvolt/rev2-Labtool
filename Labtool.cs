using Binarysharp.MemoryManagement;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;


class Labtool
{
	public Controller controller = new Controller();
	public KeyboardInput keyboard = new KeyboardInput();
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
	private readonly int _PositionXOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PositionXOffset"), 16);
	private readonly int _PositionYOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PositionYOffset"), 16);
	private readonly int _AnimStringOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("AnimStringOffset"), 16);
	private readonly int _DizzyOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DizzyOffset"), 16);

	private readonly int _BlockstunOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("BlockstunOffset"), 16);
	private readonly int _HitstunOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HitstunOffset"), 16);
	#endregion

	public Process process;

	// READING MEMORY
	public void AttachToProcess()
	{
		process = Process.GetProcessesByName(_ggprocname).FirstOrDefault();

		if (process == null)
		{
			throw new Exception("GGxrd is not open.");
		}

		_memorySharp = new MemorySharp(process);

		Player.setHashSets();
		controller.getSticks();
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
		catch (System.ArgumentException)
		{
			Dispose();
			return 0;
		}
	}

	public int ReadInfoInt(ref Player player, int offset)
	{
		try
		{
			IntPtr ptr = _memorySharp[player.playerPtr].Read<IntPtr>(); //player.playerPtr is sometimes not updated
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
		catch (System.ArgumentException)
		{
			Dispose();
			return 0;
		}
	}

	// COMPUTE
	public class Framedata
	{
		public bool blockstring = false;
		public int frameAdvantage;
		public bool updateFA;
		public bool updateGap = false;
		public int idleCount = 0;
		public int rememberGap;
	}

	public Framedata f1 = new Framedata();
	public Framedata f2 = new Framedata();
	public void frameAdvantage(Framedata f, ref Player p1, ref Player p2)
	{
		bool p1idle = p1.IsCompletelyIdle();
		bool p2idle = p2.IsCompletelyIdle();

		if (!p1idle && !p2idle)
		{
			f.frameAdvantage = 0;
			f.blockstring = true;
		}

		if ((p1idle || p2idle) && f.blockstring)
		{
			if (p1idle && p2idle)
			{
				f.blockstring = false;
				f.updateFA = true;
			}

			if (!p1idle)
			{
				--f.frameAdvantage;
			}
			if (!p2idle)
			{
				++f.frameAdvantage;
			}
		}
	}

	public void gap(Framedata f, ref Player opponent)
	{
		if (opponent.IsUnderAttack())
		{
			if (f.idleCount != -1)
			{
				if (f.idleCount <= 30)
				{
					f.rememberGap = f.idleCount;
					f.updateGap = true;
					f.idleCount = 0;
				}
				f.idleCount = -1;
			}
		}
		else
		{
			++f.idleCount;
		}
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

		player1.characterIndex = ReadStaticInt(player1.playerNumber);
		player2.characterIndex = ReadStaticInt(player2.playerNumber);
		player1.assignPlayerPtr("Player1Ptr");
		player2.assignPlayerPtr("Player2Ptr");


		player1._isBlocking = ReadInfoInt(ref player1, _BlockstunOffset) != 0;
		player2._isBlocking = ReadInfoInt(ref player2, _BlockstunOffset) != 0;
		player1._isHit = ReadInfoInt(ref player1, _HitstunOffset) != 0;
		player2._isHit = ReadInfoInt(ref player2, _HitstunOffset) != 0;
		player1.currentAnim = ReadAnimationString(ref player1);
		player2.currentAnim = ReadAnimationString(ref player2);

		frameAdvantage(f1, ref player1, ref player2);
		gap(f1, ref player2);
		gap(f2, ref player1);

		player1.HP = ReadInfoInt(ref player1, _HPOffset);
		player2.HP = ReadInfoInt(ref player2, _HPOffset);
		player1.meter = ReadInfoInt(ref player1, _MeterOffset);
		player2.meter = ReadInfoInt(ref player2, _MeterOffset);
		player1.RISC = ReadInfoInt(ref player1, _RISCOffset);
		player2.RISC = ReadInfoInt(ref player2, _RISCOffset);
		player1.stun = ReadInfoInt(ref player1, _DizzyOffset);
		player2.stun = ReadInfoInt(ref player2, _DizzyOffset);

		player1.pos.x = ReadInfoInt(ref player1, _PositionXOffset);
		player1.pos.y = ReadInfoInt(ref player1, _PositionYOffset);
		player2.pos.x = ReadInfoInt(ref player2, _PositionXOffset);
		player2.pos.y = ReadInfoInt(ref player2, _PositionYOffset);

		if (FrameCount() - currFrame != 0)
		{
			Console.WriteLine("frame skipped");
		}
		prevFrame = currFrame;
	}

	public void WriteInfoInt(ref Player player, int offset, int value)
	{
		IntPtr ptr = _memorySharp[player.playerPtr].Read<IntPtr>();
		_memorySharp.Write<int>(ptr + offset, value, false);
	}


	// READ INPUTS AND WRITE TO MEMORY
	Position savedPosP1;
	Position savedPosP2;
	public void macroButtons()
	{
		controller.GetState();
		keyboard.readKeys();

		if (keyboard.save)
		{
			savedPosP1.x = player1.pos.x;
			savedPosP1.y = player1.pos.y;
			savedPosP2.x = player2.pos.x;
			savedPosP2.y = player2.pos.y;
			
		}
		else if (controller.pressedButtons[6] || keyboard.reset)
		{
			WriteInfoInt(ref player1, _PositionXOffset, savedPosP1.x);
			WriteInfoInt(ref player1, _PositionYOffset, savedPosP1.y);
			WriteInfoInt(ref player2, _PositionXOffset, savedPosP2.x);
			WriteInfoInt(ref player2, _PositionYOffset, savedPosP2.y);
		}
	}

	public void Dispose()
	{
		_memorySharp?.Dispose();
	}
}

/*
 * TODO
 * - Find Camera center
 * - Fix character pointers (retry)
 * - Fix display when playing as p2 (most probably, find real positions and/or which side they are facing at first)
 * - Find dizzy and guts in memory
 * 
 * DONE
 * Positions are displayed (divided by 1000 for lisibility's sake)
 * keyboard key "1" or joystick button 6 (select) do reset the positions you saved by using keyboard key "2". Custom buttons and keys implementation is planned.
 * Consistency of framedata readings increased
 * Using actual blockstun and hitstun values to compute gaps
 * Displays 0F when a combo/blockstring is tight by a frame (accidental, but useful)
 * Stun values corrected: Johnny 7000, I-no 5500
 * Leo backstance is now considered idle
 * Labtool automatically sets Xrd as the foreground window, once booted up
 */
