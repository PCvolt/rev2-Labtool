using System;
using System.Collections.Generic;
using System.Configuration;


class Player
{
	public static int nbOfPlayers = 0;
	public IntPtr playerPtr;
	public int playerNumber;
	public int characterIndex;

	public string currentAnim;
	public int HP;
	public int meter;
	public int RISC;
	public int dizzy;

	public float xPos;

	public Player()
	{
		++nbOfPlayers;
		this.playerNumber = nbOfPlayers;
	}

	static public List<string> charactersList = new List<string>() { "Sol", "Ky", "May", "Millia", "Zato=1", "Potemkin", "Chipp", "Faust", "Axl", "Venom", "Slayer", "I-no", "Bedman", "Ramlethal", "Sin", "Elphelt", "Leo", "Johnny", "Jack-O'", "Jam", "Haehyun", "Raven", "Dizzy", "Baiken", "Answer" };
	static public List<int> dizzyList = new List<int>() { 6000, 6000, 7000, 5500, 6000, 8000, 5000, 6500, 6000, 6000, 7000, 6000, 6000, 6000, 6000, 6000, 7000, 6000, 6000, 6500, 7000, 5500, 5000, 5500, 5500 };
	static public List<float> defmodifList = new List<float>() { (float)1.00, (float)1.03, (float)1.06, (float)1.21, (float)1.09, (float)0.93, (float)1.30, (float)1.00, (float)1.06, (float)1.03, (float)0.96, (float)1.06, (float)0.98, (float)1.06, (float)1.04, (float)1.03, (float)1.00, (float)1.00, (float)1.03, (float)1.06, (float)0.96, (float)1.10, (float)1.06, (float)1.12, (float)1.03 };
	// 90%		76% 	60% 	50% 	40% 	Answer, Bedman, Elphelt, Faust, Zato 
	// 87%		72% 	58% 	48% 	40% 	Axl, I-No, Ramlethal, Sin, Slayer, Sol, Venom, Dizzy 
	// 84%	 	68% 	56% 	46% 	38% 	Ky, Haehyun, Jack-O' 
	// 81% 		66% 	54% 	44% 	38% 	Johnny, Leo, May, Millia, Potemkin, Jam
	// 78%		64% 	50% 	42% 	38% 	Baiken, Chipp 
	// 75% 		60% 	48% 	40% 	36% 	Raven

	static private HashSet<string> idleAnimHash;
	static private HashSet<string> blockingAnimHash;
	static string[] idleAnimArray =
	{
		"CmnActStand",
		"CmnActCrouch",
		"CmnActJump",
		"CmnActStand2Crouch",
		"CmnActCrouch2Stand",
		"CmnActFWalk",
		"CmnActBWalk",

		"CmnActCrouchGuardEnd",
		"CmnActMidGuardEnd",
		"CmnActHighGuardEnd",
		"CmnActAirGuardEnd",
	}; // Preparing to block and actually blocking are the same animations...

	static string[] blockingAnimArray =
	{
		"CmnActCrouchGuardLoop",
		"CmnActMidGuardLoop",
		"CmnActHighGuardLoop",
		"CmnActAirGuardLoop"
	};

	static public List<string> gutsRating0 = new List<string>() { };
	static public List<string> gutsRating1 = new List<string>() { };
	static public List<string> gutsRating2 = new List<string>() { };
	static public List<string> gutsRating3 = new List<string>() { };
	static public List<string> gutsRating4 = new List<string>() { };
	static public List<string> gutsRating5 = new List<string>() { };



	public static void setHashSets()
	{
		idleAnimHash = new HashSet<string>(idleAnimArray);
		blockingAnimHash = new HashSet<string>(blockingAnimArray);
	}

	public void assignPlayerPtr(string playerPointerNumber)
	{
		this.playerPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get(playerPointerNumber), 16));
	}


	public bool IsIdle() //TODO: preparing to block animations return true instead of false
	{
		if (idleAnimHash.Contains(currentAnim))
			return true;
		return false;
	}

	public bool IsBlocking()
	{
		if (blockingAnimHash.Contains(currentAnim))
			return true;
		return false;
	}
}