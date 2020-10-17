using Binarysharp.MemoryManagement;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

class MemoryAccessor
{
	private static readonly string _ggprocname = ConfigurationManager.AppSettings.Get("GGProcessName");
	private static MemorySharp _memorySharp;
	public static Process process;

	#region Pointers
	public static readonly IntPtr _frameCountOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("FrameCountOffset"), 16));
	public static readonly IntPtr _p1CharIndex = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("Player1CharIndex"), 16));
	public static readonly IntPtr _p2CharIndex = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("Player2CharIndex"), 16));
	#endregion
	#region Offsets
	public static readonly int _HPOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HPOffset"), 16);
	public static readonly int _MeterOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MeterOffset"), 16);
	public static readonly int _RISCOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("RISCOffset"), 16);
	public static readonly int _PositionXOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PositionXOffset"), 16);
	public static readonly int _PositionYOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PositionYOffset"), 16);
	public static readonly int _AnimStringOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("AnimStringOffset"), 16);
	public static readonly int _DizzyOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DizzyOffset"), 16);

	public static readonly int _BlockstunOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("BlockstunOffset"), 16);
	public static readonly int _HitstunOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HitstunOffset"), 16);
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
		Controller.getSticks();
	}

	public static int ReadStaticInt(int playerNumber)
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

	public static int ReadInfoInt(ref Player player, int offset)
	{
		try
		{
			IntPtr ptr = _memorySharp[player._playerPtr].Read<IntPtr>(); //player.playerPtr is sometimes not updated
			int integer = _memorySharp.Read<int>(ptr + offset, false);
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
			IntPtr ptr = _memorySharp[player._playerPtr].Read<IntPtr>();//
			float floatnumber = _memorySharp.Read<float>(ptr + offset, false);
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
			var ptr = _memorySharp[player._playerPtr].Read<IntPtr>();
			string str = _memorySharp.ReadString(ptr + _AnimStringOffset, false, 32);
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
		IntPtr ptr = _memorySharp[player._playerPtr].Read<IntPtr>();
		_memorySharp.Write<int>(ptr + offset, value, false);
	}

	public static int FrameCount()
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

	public static void Dispose()
	{
		_memorySharp?.Dispose();
	}
}

