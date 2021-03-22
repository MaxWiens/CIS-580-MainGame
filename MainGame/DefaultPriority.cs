using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame {
	public enum DefaultPriority : uint {
		VeryImportant = 0,
		Physics = 1,
		PlayerInput = 10,
		Any = uint.MaxValue
	}
}
