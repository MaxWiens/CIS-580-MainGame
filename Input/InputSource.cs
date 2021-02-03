using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Input {
	[Flags]
	public enum InputSource : byte {
		None = 0,
		
		Pointer  = 0b0001_0000,

		Keyboard = 0b0000_0001,
		Mouse    = 0b0001_0010,
		GamePad  = 0b0000_0100,
		Touch    = 0b0001_1000,
	}
}
