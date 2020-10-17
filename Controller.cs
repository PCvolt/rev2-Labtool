using System;
using System.Collections.Generic;
using SlimDX.DirectInput;
using SlimDX.XInput;

class Controller
{
	DirectInput joystick = new DirectInput();
	public JoystickState state = new JoystickState();
	public Joystick[] sticks;
	public bool[] pressedButtons;

	public Controller()
	{
		getSticks();
	}

	public void getSticks()
	{
		List<Joystick> sticks = new List<Joystick>();
		foreach (DeviceInstance device in joystick.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
		{
			try
			{
				Joystick stick = new Joystick(joystick, device.InstanceGuid);
				stick.Acquire();

				foreach(DeviceObjectInstance deviceObject in stick.GetObjects())
				{
					if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
						stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-100, 100);
				}

				sticks.Add(stick);
			}
			catch(DirectInputException)
			{
			}
		}
		this.sticks = sticks.ToArray();
	}

	public void GetState()
	{
		state = sticks[0].GetCurrentState();
		pressedButtons = state.GetButtons();
	}
}

