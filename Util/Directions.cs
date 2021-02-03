using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Util {
	[Flags]
	enum Directions : byte{
		None = 0,
		Up    = 0b0001,
		Down  = 0b0010,
		Left  = 0b0100,
		Right = 0b1000,
		All = Up | Down | Left | Right
	}
}
