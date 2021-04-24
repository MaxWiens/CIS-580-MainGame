using Microsoft.Xna.Framework.Input;
using System.Linq;
namespace InputSystem {
	public struct InputState {
		public readonly KeyboardState Keyboard;
		public readonly MouseState Mouse;
		public readonly GamePadState GamePad;
		public InputState(KeyboardState keyboard, MouseState mouse, GamePadState gamePad) {
			Keyboard = keyboard;
			Mouse = mouse;
			GamePad = gamePad;
		}

		public override bool Equals(object obj) {
			return obj is InputState s && Keyboard == s.Keyboard && Mouse == s.Mouse && GamePad == s.GamePad;
		}

		public bool GetMousePressed(MouseState prev) {
			if(Mouse.LeftButton == ButtonState.Pressed && Mouse.LeftButton != prev.LeftButton) return true;
			if(Mouse.RightButton == ButtonState.Pressed && Mouse.RightButton != prev.RightButton) return true;
			return false;
		}

		public bool GetKeyboardPressed(KeyboardState prev) {
			return Keyboard.GetPressedKeys().Except(prev.GetPressedKeys()).Count() > 0; 
		}
	}
}
