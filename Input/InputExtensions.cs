using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace MainGame.Input {
	public static class InputExtensions {
		public static Vector2 ToVector2(this GamePadDPad dpad)
			=> new Vector2(dpad.Left == ButtonState.Pressed ? -1 : 0 + dpad.Right == ButtonState.Pressed ? 1 : 0, dpad.Down == ButtonState.Pressed ? -1 : 0 + dpad.Up == ButtonState.Pressed ? 1 : 0);
	}
}
