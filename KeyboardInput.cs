using System.Runtime.InteropServices;


class KeyboardInput
{
	[DllImport("User32.dll")]
	private static extern short GetAsyncKeyState(int vKey);

	private static readonly int VK_1 = 0x31;
	private static readonly int VK_2 = 0x32;

	public bool reset;
	public bool save;

	public void readKeys()
	{
		short resetKeyState = GetAsyncKeyState(VK_1);
		short saveKeyState = GetAsyncKeyState(VK_2);

		reset = ((resetKeyState >> 15) & 0x0001) == 0x0001;
		save = ((saveKeyState >> 15) & 0x0001) == 0x0001;
	}
}

