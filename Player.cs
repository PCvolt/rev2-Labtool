using System;
using System.Collections;
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
	public int _hitstop;
	public int _flags0x4d3c;
	public int _flags0x4d48;
	public int _forceDisableFlags;

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
	
	private class IsIdleResponse {
		public Player player;
		public bool isIdle;
	};
	static private Dictionary<(string charName, string animName), EventHandler<IsIdleResponse>> idleAnimHash;


	public static void setHashSets()
	{
		idleAnimHash = new Dictionary<(string charName, string animName), EventHandler<IsIdleResponse>>
		{
			{ ("Chipp", "HaritsukiKeep"), IsIdleHaritsukiKeep },
			{ ("Faust", "Souten"), IsIdleSouten },
			{ ("Faust", "SoutenA"), IsIdleSouten },
			{ ("Faust", "Souten9"), IsIdleSouten },
			{ ("Faust", "Souten44"), IsIdleSouten },
			{ ("Faust", "Souten66"), IsIdleSouten },
			{ ("Faust", "SoutenB"), IsIdleSouten },
			{ ("Faust", "SoutenC"), IsIdleSouten },
			{ ("Faust", "SoutenE"), IsIdleSouten },
			{ ("Faust", "Souten8"), IsIdleSouten8 },
			{ ("Axl", "DaiRensen"), IsIdleDaiRensen },
			{ ("Elphelt", "Rifle_Start"), IsIdleRifle },
			{ ("Elphelt", "Rifle_Reload"), IsIdleRifle },
			{ ("Elphelt", "Rifle_Reload_Perfect"), IsIdleRifle },
			{ ("Elphelt", "Rifle_Reload_Roman"), IsIdleRifle },
			{ ("Leo", "Semuke"), IsIdleSemuke },
			{ ("Jam", "NeoHochihu"), IsIdleNeoHochihu },
			{ ("Answer", "Ami_Hold"), IsIdleAmi_Hold },
			{ ("Answer", "Ami_Move"), IsIdleAmi_Move }
		};
	}

	public void assignPlayerPtr(string playerPointerNumber)
	{
		_playerPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get(playerPointerNumber), 16));
	}


	public bool IsCompletelyIdle()
	{
		(string charName, string animName) charAndAnim = (charactersList[characterIndex], _currentAnim);
		if (!idleAnimHash.ContainsKey(charAndAnim))
		{
			return (_flags0x4d3c & 0x1000) != 0;  // enableNormals
		}
		EventHandler<IsIdleResponse> handler = idleAnimHash[charAndAnim];
		IsIdleResponse response = new IsIdleResponse{
			isIdle = false,
			player = this
		};
		handler(this, response);
		return response.isIdle;
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
	// Chipp wall cling idle/moving up/down
	static private void IsIdleHaritsukiKeep(object sender, IsIdleResponse e)
	{
		e.isIdle = (e.player._flags0x4d48 & 0x2) != 0;  // enableWhiffCancels
	}
	// Faust pogo
	static private void IsIdleSouten(object sender, IsIdleResponse e)
	{
		e.isIdle = (e.player._flags0x4d48 & 0x2) != 0;  // enableWhiffCancels
	}
	// Faust pogo helicopter
	static private void IsIdleSouten8(object sender, IsIdleResponse e)
	{
		e.isIdle = (e.player._flags0x4d48 & 0x1) == 0;  // !enableGatlings
	}
	// Axl Haitaka stance
	static private void IsIdleDaiRensen(object sender, IsIdleResponse e)
	{
		e.isIdle = (e.player._flags0x4d48 & 0x2) != 0;  // enableWhiffCancels
	}
	// Elphelt Ms. Confille (rifle)
	static private void IsIdleRifle(object sender, IsIdleResponse e)
	{
		e.isIdle = (e.player._flags0x4d48 & 0x2) != 0  // enableWhiffCancels
			&& (e.player._forceDisableFlags & 0x2) == 0;  // 0x2 is the force disable flag for Rifle_Fire
	}
	// Leo backturn idle
	static private void IsIdleSemuke(object sender, IsIdleResponse e)
	{
		e.isIdle = true;
	}
	// Jam parry
	static private void IsIdleNeoHochihu(object sender, IsIdleResponse e)
	{
		e.isIdle = false;
	}
	// Answer scroll cling idle
	static private void IsIdleAmi_Hold(object sender, IsIdleResponse e)
	{
		e.isIdle = (e.player._flags0x4d48 & 0x2) != 0;
	}
	// Answer s.D
	static private void IsIdleAmi_Move(object sender, IsIdleResponse e)
	{
		e.isIdle = (e.player._flags0x4d48 & 0x2) != 0
			&& e.player._hitstop == 0;
	}
}