using System;
using System.Collections.Generic;
using System.Configuration;

public struct Position
{
	public int x;
	public int y;

	public Position(int x, int y)
    {
		this.x = x;
		this.y = y;
    }
}

class Player
{
	public static int _nbOfPlayers = 0;
	public IntPtr _playerPtr;
	public int _playerNumber;


	public string _currentAnim;
	public int _HP;
	public int _meter;
	public int _RISC;
	public int _stun;
	public bool _isBlocking = false;
	public bool _isHit = false;

	public Position _pos;

	public Player()
	{
		++_nbOfPlayers;
		_playerNumber = _nbOfPlayers;
	}

	public int characterIndex;
	static public List<string> charactersList = new List<string>() { "Sol", "Ky", "May", "Millia", "Zato=1", "Potemkin", "Chipp", "Faust", "Axl", "Venom", "Slayer", "I-no", "Bedman", "Ramlethal", "Sin", "Elphelt", "Leo", "Johnny", "Jack-O'", "Jam", "Haehyun", "Raven", "Dizzy", "Baiken", "Answer" };
	static public List<int> stunList = new List<int>() { 6000, 6000, 7000, 5500, 6000, 8000, 5000, 6500, 6000, 6000, 7000, 5500, 6000, 6000, 6000, 6000, 7000, 7000, 6000, 6500, 7000, 5500, 5000, 5500, 5500 };
	static public List<float> defmodifList = new List<float>() { (float)1.00, (float)1.03, (float)1.06, (float)1.21, (float)1.09, (float)0.93, (float)1.30, (float)1.00, (float)1.06, (float)1.03, (float)0.96, (float)1.06, (float)0.98, (float)1.06, (float)1.04, (float)1.03, (float)1.00, (float)1.00, (float)1.03, (float)1.06, (float)0.96, (float)1.10, (float)1.06, (float)1.12, (float)1.03 };
	// 90%		76% 	60% 	50% 	40% 	Answer, Bedman, Elphelt, Faust, Zato 
	// 87%		72% 	58% 	48% 	40% 	Axl, I-No, Ramlethal, Sin, Slayer, Sol, Venom, Dizzy 
	// 84%	 	68% 	56% 	46% 	38% 	Ky, Haehyun, Jack-O' 
	// 81% 		66% 	54% 	44% 	38% 	Johnny, Leo, May, Millia, Potemkin, Jam
	// 78%		64% 	50% 	42% 	38% 	Baiken, Chipp 
	// 75% 		60% 	48% 	40% 	36% 	Raven

	static private HashSet<string> idleAnimHash;
	static string[] idleAnimArray =
	{
		"CmnActStand",
		"CmnActCrouch",
		"CmnActJump",
		"CmnActStand2Crouch",
		"CmnActCrouch2Stand",
		"CmnActFWalk",
		"CmnActBWalk",

		"Semuke", //Leo backstance

		"CmnActCrouchGuardEnd",
		"CmnActMidGuardEnd",
		"CmnActHighGuardEnd",
		"CmnActAirGuardEnd",
	};


	public static void setHashSets()
	{
		idleAnimHash = new HashSet<string>(idleAnimArray);
	}

	public void assignPlayerPtr(string playerPointerNumber)
	{
		_playerPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get(playerPointerNumber), 16));
	}


	public bool IsCompletelyIdle()
	{
		if (idleAnimHash.Contains(_currentAnim))
			return true;
		return false;
	}

	public bool IsBlocking()
	{
		return _isBlocking;
	}

	public bool IsHit()
	{
		return _isHit;
	}

	public bool IsUnderAttack()
	{
		return IsBlocking() || IsHit();
	}
}