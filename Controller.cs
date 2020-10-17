using SlimDX.DirectInput;
using System.Collections.Generic;

class Controller
{
	private static DirectInput _joystick = new DirectInput();
	private static JoystickState _state = new JoystickState();
	private static Joystick[] _sticks;
	public bool[] _pressedButtons;

	public Controller()
	{
		getSticks();
	}

	public static void getSticks()
	{
		List<Joystick> sticks = new List<Joystick>();
		foreach (DeviceInstance device in _joystick.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
		{
			try
			{
				Joystick stick = new Joystick(_joystick, device.InstanceGuid);
				stick.Acquire();

				foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
				{
					if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
						stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-100, 100);
				}

				sticks.Add(stick);
			}
			catch (DirectInputException)
			{
			}
		}
		_sticks = sticks.ToArray();
	}

	public void GetState()
	{
		_state = _sticks[0].GetCurrentState();
		_pressedButtons = _state.GetButtons();
	}
}

