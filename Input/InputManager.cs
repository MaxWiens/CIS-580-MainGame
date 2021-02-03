using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace MainGame.Input {
	using InputIdentifier = ValueTuple<InputSource, ValueType>;
	public delegate void InputAction(InputData data);

	public class InputManager {
		public event Action<float> Moved;

		private Dictionary<string, List<Binding>> _actionBindings;

		private KeyboardState _previousKeyboardState;
		private Keys[] _previousKeysDown = new Keys[0];

		private GamePadData _previousGamePadData;
		private MouseState _previousMouseState;
		private Dictionary<InputIdentifier, InputAction> _actionMap;

		private Dictionary<string, Action> _actions;

		public InputManager() {
			_actionBindings = new Dictionary<string, List<Binding>>();

			_actionMap = new Dictionary<InputIdentifier, InputAction>() {
				{(InputSource.Keyboard, Keys.A), DebugPrintKey},
				{(InputSource.Keyboard, Keys.D), DebugPrintKey},
			};
			_actions = new Dictionary<string, Action>();

			LoadBindings(@"Content\Controlls.json");
		}

		public void AddListener(string actionName, Action<Vector2> inputAction) {
			if(_actions.TryGetValue(actionName, out Action action)) {
				action.ValueChanged += inputAction;
			}
		}

		public void RemoveListener(string actionName, Action<Vector2> inputAction) {
			if(_actions.TryGetValue(actionName, out Action action)) {
				action.ValueChanged -= inputAction;
			}
		}

		private void DebugPrintKey(InputData data) {
			System.Diagnostics.Debug.WriteLine(data.State);
			Moved?.Invoke(data.ReadFloat());
		}

		public void Update(float deltaTime) {
			InputState state = new InputState(Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(0));
			System.Diagnostics.Debug.WriteLine($"{state.GamePad.ThumbSticks.Left},\t {_actions["Move"].PollValue(state)}");
			foreach(Action action in _actions.Values) {
				action.Update(state);
			}
		}

		private struct GamePadData {
			public readonly GamePadButtons Buttons;
			public readonly ButtonState A;
			public readonly ButtonState B;
			public readonly ButtonState X;
			public readonly ButtonState Y;
			public readonly ButtonState Start;
			public readonly ButtonState Back;
			public readonly ButtonState BigButton;
			public readonly ButtonState LeftShoulder;
			public readonly ButtonState RightShoulder;
			public readonly ButtonState LeftStickPress;
			public readonly ButtonState RightStickPress;
			public readonly GamePadTriggers Triggers;
			public readonly float LeftTrigger;
			public readonly float RightTrigger;
			public readonly GamePadDPad DPad;
			public readonly ButtonState Left;
			public readonly ButtonState Right;
			public readonly ButtonState Up;
			public readonly ButtonState Down;
			public readonly GamePadThumbSticks Sticks;
			public readonly Vector2 LeftStick;
			public readonly Vector2 RightStick;
			public readonly GamePadState State;
			public GamePadData(GamePadState state) {
				Buttons = state.Buttons;
				A = Buttons.A;
				B = Buttons.B;
				X = Buttons.X;
				Y = Buttons.Y;
				Start = Buttons.Start;
				Back = Buttons.Back;
				BigButton = Buttons.BigButton;
				LeftShoulder = Buttons.LeftShoulder;
				RightShoulder = Buttons.RightShoulder;
				LeftStickPress = Buttons.LeftStick;
				RightStickPress = Buttons.RightStick;
				Triggers = state.Triggers;
				LeftTrigger = Triggers.Left;
				RightTrigger = Triggers.Right;
				DPad = state.DPad;
				Left = DPad.Left;
				Right = DPad.Right;
				Up = DPad.Up;
				Down = DPad.Down;
				Sticks = state.ThumbSticks;
				LeftStick = Sticks.Left;
				RightStick = Sticks.Right;
				State = state;
			}

		}

		public void LoadBindings(string filePath) {
			using(FileStream stream = File.OpenRead(filePath)) {
				JsonDocument doc = JsonDocument.Parse(stream);
				_actions.Clear();
				foreach(JsonProperty actionJson in doc.RootElement.EnumerateObject()) {
					if(_actions.ContainsKey(actionJson.Name))
						throw new Exception($"Binding already exists for action {actionJson.Name}");
					List<Binding> bindings = new List<Binding>();
					foreach(JsonElement bindingJson in actionJson.Value.EnumerateArray()) {
						switch(bindingJson.ValueKind) {
							case JsonValueKind.String:
								// single binding
								bindings.Add(JsonSerializer.Deserialize<SoloBinding>(bindingJson.GetRawText(), Serialization.JsonDefaults.Options));
								break;
							case JsonValueKind.Object:
							case JsonValueKind.Array:
								// composite type
								bindings.Add(JsonSerializer.Deserialize<CompositeBinding>(bindingJson.GetRawText(), Serialization.JsonDefaults.Options));
								break;
							default:
								throw new JsonException();
						}
					}
					_actions.Add(actionJson.Name, new Action(actionJson.Name, bindings));
				}
			}
		}

		private class Action {
			public readonly string Name;
			private Vector2 _previousValue;
			private IEnumerable<Binding> _bindings;
			public event Action<Vector2> ValueChanged;
			public Action(string name, IEnumerable<Binding> bindings) {
				Name = name;
				_bindings = bindings;
				_previousValue = Vector2.Zero;
			}

			public Vector2 PollValue(InputState state) {
				Vector2 value = Vector2.Zero;
				foreach(Binding b in _bindings) {
					value += b.Read(state);
				}
				if(value.Length() > 1f)
					value.Normalize();
				return value;
			}

			public void Update(InputState state) {
				Vector2 value = Vector2.Zero;
				foreach(Binding b in _bindings) {
					value += b.Read(state);
				}
				if(value.Length() > 1f)
					value.Normalize();

				if(value != _previousValue && ValueChanged != null)
					ValueChanged(value);
				_previousValue = value;
			}

		}
	}
}
