using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MainGame.Input {
	public struct InputData {
		public readonly InputPhase State;
		private readonly Vector2 _value;

		public InputData(InputPhase state) {
			State = state;
			if(state == InputPhase.Pressed) {
				_value = Vector2.One;
			} else {
				_value = Vector2.Zero;
			}
		}

		public InputData(ButtonState state) {
			if(state == ButtonState.Pressed) {
				State = InputPhase.Pressed;
				_value = Vector2.One;
			} else {
				State = InputPhase.Released;
				_value = Vector2.Zero;
			}
		}

		public InputData(Vector2 value, InputPhase state) {
			State = state;
			_value = value;
		}

		public InputData(float value, InputPhase state) {
			State = state;
			_value = new Vector2(value);
		}

		public bool ReadBool() => _value != Vector2.Zero;
		public float ReadFloat() => _value.X;
		public Vector2 ReadVector2() => _value;
	}
}
