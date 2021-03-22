using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace InputSystem {
	public static class InputExtensions {
		public static Vector2 ToVector2(this GamePadDPad dpad)
			=> new Vector2(dpad.Left == ButtonState.Pressed ? -1 : 0 + dpad.Right == ButtonState.Pressed ? 1 : 0, dpad.Down == ButtonState.Pressed ? -1 : 0 + dpad.Up == ButtonState.Pressed ? 1 : 0);
		public static float AsFloat(this Vector2 value)
			=> value.X != 0 ? value.X : value.Y;
		public static bool AsBool(this Vector2 value)
			=> value != Vector2.Zero;
	}
}
