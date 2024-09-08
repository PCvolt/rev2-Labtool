using System;
using System.Runtime.InteropServices;

class Labtool
{
	public Controller _controller = new Controller();
	public KeyboardInput _keyboard = new KeyboardInput();
	public Player _player1 = new Player();
	public Player _player2 = new Player();

	public class Framedata
	{
		public bool blockstring = false;
		public int frameAdvantage;
		public bool updateFA;
	}

	public class Gapdata
	{
		public bool updateGap = false;
		public int idleCount = 0;
		public int rememberGap;
	}

	#region Framedata
	public Framedata f = new Framedata();
	public Gapdata g1 = new Gapdata();
	public Gapdata g2 = new Gapdata();
	private void frameAdvantage(Framedata f, ref Player p1, ref Player p2)
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

	private void gap(Gapdata g, ref Player opponent)
	{
		if (opponent.IsUnderAttack())
		{
			if (g.idleCount != -1)
			{
				if (g.idleCount <= 30)
				{
					g.rememberGap = g.idleCount + 1;
					g.updateGap = true;
					g.idleCount = 0;
				}
				g.idleCount = -1;
			}
		}
		else
		{
			++g.idleCount;
		}
	}
	#endregion

	#region DataRefreshing
	private int prevFrame = 0;
	private int currFrame = 0;
	public void updateFrameInfo()
	{
		do
		{
			timeBeginPeriod(1);
			System.Threading.Thread.Sleep(1);
			timeEndPeriod(1);
			currFrame = MemoryAccessor.FrameCount();
		}
		while (currFrame == prevFrame);

		IntPtr aswEngPtr = MemoryAccessor.GetAswEnginePtr();
		if ((int)aswEngPtr == 0)
		{
		    // match not currently running
		    System.Threading.Thread.Sleep(1000);
		    return;
		}
		_player1.assignPlayerPtr(aswEngPtr + MemoryAccessor._p1Offset);
		_player2.assignPlayerPtr(aswEngPtr + MemoryAccessor._p2Offset);


		_player1.characterIndex = MemoryAccessor.ReadInfoInt(ref _player1, MemoryAccessor._charIndexOffset);
		_player2.characterIndex = MemoryAccessor.ReadInfoInt(ref _player2, MemoryAccessor._charIndexOffset);
		_player1._isBlocking = MemoryAccessor.ReadInfoInt(ref _player1, MemoryAccessor._BlockstunOffset) != 0;
		_player2._isBlocking = MemoryAccessor.ReadInfoInt(ref _player2, MemoryAccessor._BlockstunOffset) != 0;
		_player1._isHit = MemoryAccessor.ReadInfoInt(ref _player1, MemoryAccessor._HitstunOffset) != 0;
		_player2._isHit = MemoryAccessor.ReadInfoInt(ref _player2, MemoryAccessor._HitstunOffset) != 0;
		_player1._currentAnim = MemoryAccessor.ReadAnimationString(ref _player1);
		_player2._currentAnim = MemoryAccessor.ReadAnimationString(ref _player2);
		_player1._hitstop = MemoryAccessor.ReadHitstop(ref _player1);
		_player2._hitstop = MemoryAccessor.ReadHitstop(ref _player2);
		_player1._flags0x4d3c = MemoryAccessor.ReadFlags0x4d3c(ref _player1);
		_player2._flags0x4d3c = MemoryAccessor.ReadFlags0x4d3c(ref _player2);
		_player1._flags0x4d48 = MemoryAccessor.ReadFlags0x4d48(ref _player1);
		_player2._flags0x4d48 = MemoryAccessor.ReadFlags0x4d48(ref _player2);
		_player1._forceDisableFlags = MemoryAccessor.ReadForceDisableFlags(ref _player1);
		_player2._forceDisableFlags = MemoryAccessor.ReadForceDisableFlags(ref _player2);

		frameAdvantage(f, ref _player1, ref _player2);
		gap(g1, ref _player2);
		gap(g2, ref _player1);

		_player1._HP = MemoryAccessor.ReadInfoInt(ref _player1, MemoryAccessor._HPOffset);
		_player2._HP = MemoryAccessor.ReadInfoInt(ref _player2, MemoryAccessor._HPOffset);
		_player1._meter = MemoryAccessor.ReadInfoInt(ref _player1, MemoryAccessor._MeterOffset);
		_player2._meter = MemoryAccessor.ReadInfoInt(ref _player2, MemoryAccessor._MeterOffset);
		_player1._RISC = MemoryAccessor.ReadInfoInt(ref _player1, MemoryAccessor._RISCOffset);
		_player2._RISC = MemoryAccessor.ReadInfoInt(ref _player2, MemoryAccessor._RISCOffset);
		_player1._stun = MemoryAccessor.ReadInfoInt(ref _player1, MemoryAccessor._StunOffset);
		_player2._stun = MemoryAccessor.ReadInfoInt(ref _player2, MemoryAccessor._StunOffset);

		_player1._pos.x = MemoryAccessor.ReadInfoInt(ref _player1, MemoryAccessor._PositionXOffset);
		_player1._pos.y = MemoryAccessor.ReadInfoInt(ref _player1, MemoryAccessor._PositionYOffset);
		_player2._pos.x = MemoryAccessor.ReadInfoInt(ref _player2, MemoryAccessor._PositionXOffset);
		_player2._pos.y = MemoryAccessor.ReadInfoInt(ref _player2, MemoryAccessor._PositionYOffset);

		if (MemoryAccessor.FrameCount() - currFrame != 0)
		{
			Console.WriteLine("frame skipped");
		}
		prevFrame = currFrame;
	}
	#endregion

	#region InputProcessing
	Position savedPosP1 = new Position(-252000,0); //P1 roundstart position
	Position savedPosP2 = new Position(252000, 0); //P2 roundstart position
	public static bool processKeys = false;
	public void macroButtons()
	{
		_keyboard.readKeys();

		if (processKeys)
		{

			if (_keyboard._save)
			{
				savedPosP1.x = _player1._pos.x;
				savedPosP1.y = _player1._pos.y;
				savedPosP2.x = _player2._pos.x;
				savedPosP2.y = _player2._pos.y;

			}
			else if (_keyboard._reset)
			{
				MemoryAccessor.WriteInfoInt(ref _player1, MemoryAccessor._PositionXOffset, savedPosP1.x);
				MemoryAccessor.WriteInfoInt(ref _player1, MemoryAccessor._PositionYOffset, savedPosP1.y);
				MemoryAccessor.WriteInfoInt(ref _player2, MemoryAccessor._PositionXOffset, savedPosP2.x);
				MemoryAccessor.WriteInfoInt(ref _player2, MemoryAccessor._PositionYOffset, savedPosP2.y);
			}

			else if (_controller._isStickEnabled == true)
            {
				_controller.GetState();
				/*
				if (_controller._pressedButtons[6])
                {
                    MemoryAccessor.WriteInfoInt(ref _player1, MemoryAccessor._PositionXOffset, savedPosP1.x);
					MemoryAccessor.WriteInfoInt(ref _player1, MemoryAccessor._PositionYOffset, savedPosP1.y);
					MemoryAccessor.WriteInfoInt(ref _player2, MemoryAccessor._PositionXOffset, savedPosP2.x);
					MemoryAccessor.WriteInfoInt(ref _player2, MemoryAccessor._PositionYOffset, savedPosP2.y);
                }
				*/
			}
		}
	}
	#endregion
	
    #region DLL Imports
    [DllImport("winmm.dll")]
    private static extern int timeBeginPeriod(uint uPeriod);

    [DllImport("winmm.dll")]
    private static extern int timeEndPeriod(uint uPeriod);

    #endregion
}

/*
 * TODO
 * - Find Camera center
 * - Fix character pointers
 * - Fix display when playing as p2 (most probably, find real positions and/or which side they are facing at first)
 * - Find dizzy thresholds and guts in memory
 * 
 * DONE
 * Positions are displayed (divided by 1000 for lisibility's sake)
 * keyboard key "1" or joystick button 6 (select) do reset the positions you saved by using keyboard key "2". Custom buttons and keys implementation is planned.
 * Consistency of framedata readings improved (the tool may be wrong once every twenty times now, and only by a frame)
 * Using actual blockstun and hitstun values to compute gaps
 * Displays 0F when a combo/blockstring is tight by a frame (accidental, but useful)
 * Dizzy renamed to Stun to avoid confusions with the playable character
 * Stun values corrected: Johnny 7000, I-no 5500
 * Leo backstance is now considered idle
 * Labtool automatically sets Xrd as the foreground window, once booted up
 * 
 * NOTES
 * If stuck falling on the ground indefinitely because of the position reset, you can jump to go back to normal
 */
