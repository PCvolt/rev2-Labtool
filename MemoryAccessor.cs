using Binarysharp.MemoryManagement;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

class MemoryAccessor
{
	private static readonly string _ggprocname = ConfigurationManager.AppSettings.Get("GGProcessName");
	private static MemorySharp _memorySharp;
	public static Process process;

	#region Pointers
	public static readonly IntPtr _aswEnginePtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("AswEnginePtr"), 16));
	#endregion
	#region Offsets
	public static readonly int _p1Offset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Player1Offset"), 16);
	public static readonly int _p2Offset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Player2Offset"), 16);
	public static readonly int[] _pOffset = new int[2] { _p1Offset, _p2Offset };
	public static readonly int _frameCountOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("FrameCountOffset"), 16);
	
	public static readonly int _charIndexOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CharIndexOffset"), 16);
	public static readonly int _HPOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HPOffset"), 16);
	public static readonly int _DefenseModifierOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DefenseModifierOffset"), 16);
	public static readonly int _MeterOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MeterOffset"), 16);
	public static readonly int _RISCOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("RISCOffset"), 16);
	public static readonly int _PositionXOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PositionXOffset"), 16);
	public static readonly int _PositionYOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PositionYOffset"), 16);
	public static readonly int _AnimStringOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("AnimStringOffset"), 16);
	public static readonly int _StunOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("StunOffset"), 16);
	public static readonly int _StunThresholdOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("StunThresholdOffset"), 16);

	public static readonly int _BlockstunOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("BlockstunOffset"), 16);
	public static readonly int _HitstunOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HitstunOffset"), 16);
	public static readonly int _HitstopOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HitstopOffset"), 16);
	public static readonly int _Flags0x4d3cOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Flags0x4d3cOffset"), 16);
	public static readonly int _Flags0x4d48Offset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Flags0x4d48Offset"), 16);
	public static readonly int _ForceDisableFlagsOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ForceDisableFlagsOffset"), 16);
	#endregion


	public static void AttachToProcess()
	{
		process = Process.GetProcessesByName(_ggprocname).FirstOrDefault();

		if (process == null)
		{
			throw new Exception("GGxrd is not open.");
		}

		_memorySharp = new MemorySharp(process);

		Player.setHashSets();
		//Controller.getSticks();
	}
	
	public static IntPtr GetAswEnginePtr()
	{
	    return _memorySharp[_aswEnginePtr].Read<IntPtr>();
	}

	public static int ReadInfoInt(ref Player player, int offset)
	{
		try
		{
			int integer;
			try
			{
				integer = _memorySharp.Read<int>(player._playerPtr + offset, false);
			}
			catch (Win32Exception)
			{
				integer = 0;
			}
			return integer;
		}
		catch (ArgumentException)
		{
			Dispose();
		}

		return -1;
	}

	public static float ReadInfoFloat(ref Player player, int offset)
	{
		try
		{
			float floatnumber;
            try
            {
				floatnumber = _memorySharp.Read<float>(player._playerPtr + offset, false);
            }
            catch (Win32Exception)
			{
				floatnumber = 0;
			}
			return floatnumber;
		}
		catch (ArgumentException)
		{
			Dispose();
		}

		return -1;
	}

	public static string ReadAnimationString(ref Player player)
	{
		try
		{
			string str;
            try
            {
				str = _memorySharp.ReadString(player._playerPtr + _AnimStringOffset, false, 32);
            }
            catch (Win32Exception)
			{
            	str = string.Empty;
            }
			return str;
		}
		catch (ArgumentException)
		{
			Dispose();
		}

		return string.Empty;
	}

	public static void WriteInfoInt(ref Player player, int offset, int value)
	{
		_memorySharp.Write<int>(player._playerPtr + offset, value, false);
	}

	public static int FrameCount()
	{
		try
		{
		    IntPtr aswEngPtr = GetAswEnginePtr();
		    if ((int)aswEngPtr == 0)
            {
                return 0;
            }
            try
            {
				return _memorySharp.Read<int>(aswEngPtr + _frameCountOffset, false);
            }
            catch (Win32Exception)
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

	public static int ReadHitstop(ref Player player)
	{
		return ReadInfoInt(ref player, _HitstopOffset);
	}

	public static int ReadFlags0x4d3c(ref Player player)
	{
		return ReadInfoInt(ref player, _Flags0x4d3cOffset);
	}

	public static int ReadFlags0x4d48(ref Player player)
	{
		return ReadInfoInt(ref player, _Flags0x4d48Offset);
	}

	public static int ReadForceDisableFlags(ref Player player)
	{
		return ReadInfoInt(ref player, _ForceDisableFlagsOffset);
	}

	public static void Dispose()
	{
		_memorySharp?.Dispose();
	}
}

