using System;

class Labtool
{
	public Controller controller = new Controller();
	public KeyboardInput keyboard = new KeyboardInput();
	public Player player1 = new Player();
	public Player player2 = new Player();

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
					g.rememberGap = g.idleCount;
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
			currFrame = MemoryAccessor.FrameCount();
		}
		while (currFrame == prevFrame);

		player1.characterIndex = MemoryAccessor.ReadStaticInt(player1._playerNumber);
		player2.characterIndex = MemoryAccessor.ReadStaticInt(player2._playerNumber);
		player1.assignPlayerPtr("Player1Ptr");
		player2.assignPlayerPtr("Player2Ptr");


		player1._isBlocking = MemoryAccessor.ReadInfoInt(ref player1, MemoryAccessor._BlockstunOffset) != 0;
		player2._isBlocking = MemoryAccessor.ReadInfoInt(ref player2, MemoryAccessor._BlockstunOffset) != 0;
		player1._isHit = MemoryAccessor.ReadInfoInt(ref player1, MemoryAccessor._HitstunOffset) != 0;
		player2._isHit = MemoryAccessor.ReadInfoInt(ref player2, MemoryAccessor._HitstunOffset) != 0;
		player1._currentAnim = MemoryAccessor.ReadAnimationString(ref player1);
		player2._currentAnim = MemoryAccessor.ReadAnimationString(ref player2);

		frameAdvantage(f, ref player1, ref player2);
		gap(g1, ref player2);
		gap(g2, ref player1);

		player1._HP = MemoryAccessor.ReadInfoInt(ref player1, MemoryAccessor._HPOffset);
		player2._HP = MemoryAccessor.ReadInfoInt(ref player2, MemoryAccessor._HPOffset);
		player1._meter = MemoryAccessor.ReadInfoInt(ref player1, MemoryAccessor._MeterOffset);
		player2._meter = MemoryAccessor.ReadInfoInt(ref player2, MemoryAccessor._MeterOffset);
		player1._RISC = MemoryAccessor.ReadInfoInt(ref player1, MemoryAccessor._RISCOffset);
		player2._RISC = MemoryAccessor.ReadInfoInt(ref player2, MemoryAccessor._RISCOffset);
		player1._stun = MemoryAccessor.ReadInfoInt(ref player1, MemoryAccessor._DizzyOffset);
		player2._stun = MemoryAccessor.ReadInfoInt(ref player2, MemoryAccessor._DizzyOffset);

		player1._pos.x = MemoryAccessor.ReadInfoInt(ref player1, MemoryAccessor._PositionXOffset);
		player1._pos.y = MemoryAccessor.ReadInfoInt(ref player1, MemoryAccessor._PositionYOffset);
		player2._pos.x = MemoryAccessor.ReadInfoInt(ref player2, MemoryAccessor._PositionXOffset);
		player2._pos.y = MemoryAccessor.ReadInfoInt(ref player2, MemoryAccessor._PositionYOffset);

		if (MemoryAccessor.FrameCount() - currFrame != 0)
		{
			Console.WriteLine("frame skipped");
		}
		prevFrame = currFrame;
	}
	#endregion

	#region InputProcessing
	Position savedPosP1;
	Position savedPosP2;
	public void macroButtons()
	{
		controller.GetState();
		keyboard.readKeys();

		if (keyboard.save)
		{
			savedPosP1.x = player1._pos.x;
			savedPosP1.y = player1._pos.y;
			savedPosP2.x = player2._pos.x;
			savedPosP2.y = player2._pos.y;

		}
		else if (controller._pressedButtons[6] || keyboard.reset)
		{
			MemoryAccessor.WriteInfoInt(ref player1, MemoryAccessor._PositionXOffset, savedPosP1.x);
			MemoryAccessor.WriteInfoInt(ref player1, MemoryAccessor._PositionYOffset, savedPosP1.y);
			MemoryAccessor.WriteInfoInt(ref player2, MemoryAccessor._PositionXOffset, savedPosP2.x);
			MemoryAccessor.WriteInfoInt(ref player2, MemoryAccessor._PositionYOffset, savedPosP2.y);
		}
	}
	#endregion
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
 * Dizzy renamed to Stun to avoid confusions with the playable character
 * Stun values corrected: Johnny 7000, I-no 5500
 * Leo backstance is now considered idle
 * Labtool automatically sets Xrd as the foreground window, once booted up
 * 
 * NOTES
 * If stuck falling on the ground indefinitely because of the position reset, you can jump to go back to normal
 */
