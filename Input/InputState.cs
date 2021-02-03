using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MainGame.Input {
	public class InputState {
		public readonly KeyboardState Keyboard;
		public readonly MouseState Mouse;
		public readonly GamePadState GamePad;
		public InputState(KeyboardState keyboard, MouseState mouse, GamePadState gamePad) {
			Keyboard = keyboard;
			Mouse = mouse;
			GamePad = gamePad;
		}
	}
}
