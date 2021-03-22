using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InputSystem {
	using BindingReader = Func<InputState, Vector2>;

	public abstract class Binding {
		public readonly BindingReader Read;
		public Binding(BindingReader reader) {
			Read = reader;
		}

		public static Func<InputState, Vector2> GetInputClosure(string bindingName) {
			if(bindingName == string.Empty) return null;

			string[] bindingNames = bindingName.Split('.');
			string source = bindingNames[0];
			System.Reflection.PropertyInfo property;
			switch(source) {
				case "Keys":
					Keys k = (Keys)Enum.Parse(typeof(Keys), bindingNames[1]);
					return (state) => state.Keyboard.IsKeyDown(k) ? Vector2.One : Vector2.Zero;
				case "Mouse":
					switch(bindingNames[1]) {
						case "Position":
							return (state) => state.Mouse.Position.ToVector2();
					}

					property = typeof(MouseState).GetProperty(bindingNames[1]);
					if(property.PropertyType == typeof(int))
						return (state) => new Vector2((int)property.GetValue(state.Mouse));
					else if(property.PropertyType == typeof(ButtonState))
						return (state) => ((ButtonState)property.GetValue(state.Mouse)) == ButtonState.Pressed ? Vector2.One : Vector2.Zero;

					break;

				case "GamePad":
					switch(bindingNames[1]) {
						case "DPad":
							return (state) => state.GamePad.DPad.ToVector2();

						case "Up":
							return (state) => state.GamePad.DPad.Up == ButtonState.Pressed ? Vector2.One : Vector2.Zero;
						case "Down":
							return (state) => state.GamePad.DPad.Down == ButtonState.Pressed ? Vector2.One : Vector2.Zero;
						case "Left":
							return (state) => state.GamePad.DPad.Left == ButtonState.Pressed ? Vector2.One : Vector2.Zero;
						case "Right":
							return (state) => state.GamePad.DPad.Right == ButtonState.Pressed ? Vector2.One : Vector2.Zero;

						case "LeftStick":
							return (state) => state.GamePad.ThumbSticks.Left;
						case "LeftStickPress":
							return (state) => state.GamePad.Buttons.LeftStick == ButtonState.Pressed ? Vector2.One : Vector2.Zero;

						case "RightStick":
							return (state) => state.GamePad.ThumbSticks.Right;
						case "RightStickPress":
							return (state) => state.GamePad.Buttons.RightStick == ButtonState.Pressed ? Vector2.One : Vector2.Zero;

						case "LeftTrigger":
							return (state) => new Vector2(state.GamePad.Triggers.Left);
						case "RightTrigger":
							return (state) => new Vector2(state.GamePad.Triggers.Right);
					}

					property = typeof(GamePadButtons).GetProperty(bindingNames[1]);
					if(property.PropertyType == typeof(ButtonState))
						return (state) => ((ButtonState)property.GetValue(state.GamePad.Buttons)) == ButtonState.Pressed ? Vector2.One : Vector2.Zero;
					break;
			}
			throw new Exception($"Could not get input closure for \"{bindingName}\"");
		}
	}
}
