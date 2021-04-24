using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Text.Json;
using System.IO;

namespace InputSystem {
	public static class InputManager {
		private static readonly Dictionary<string, InputAction> _actions = new Dictionary<string, InputAction>();
		private static readonly JsonSerializerOptions _options = new JsonSerializerOptions() {
			Converters = {
				new Serialization.SoloBindingConverter(),
				new Serialization.CompositeBindingConverter(),
			}
		};

		public static event Action AnyKey;

		public static event Action<Vector2> MousePressed;

		public static void AddListener(string actionName, Action<Vector2> inputAction) {
			if(_actions.TryGetValue(actionName, out InputAction action)) {
				action.ValueChanged += inputAction;
			}
		}
		public static void RemoveListener(string actionName, Action<Vector2> inputAction) {
			if(_actions.TryGetValue(actionName, out InputAction action)) {
				action.ValueChanged -= inputAction;
			}
		}
		private static InputState _prevState;
		static InputManager() {
			_prevState = new InputState(Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(0));
		}
		public static void Update() {
			InputState state = new InputState(Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(0));
			
			if(AnyKey != null && !_prevState.Equals(state) && (state.GetKeyboardPressed(_prevState.Keyboard) || state.GetMousePressed(_prevState.Mouse))) {
				AnyKey();
			}

			foreach(InputAction action in _actions.Values) {
				action.Update(state);
			}

			if(state.Mouse.LeftButton != _prevState.Mouse.LeftButton) {
				if(state.Mouse.LeftButton == ButtonState.Pressed) {
					MousePressed?.Invoke(state.Mouse.Position.ToVector2());
				} else {

				}
			}
			
			_prevState = state;
		}

		public static void LoadBindings(string filePath) {
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
								bindings.Add(JsonSerializer.Deserialize<SoloBinding>(bindingJson.GetRawText(), _options));
								break;
							case JsonValueKind.Object:
							case JsonValueKind.Array:
								// composite type
								bindings.Add(JsonSerializer.Deserialize<CompositeBinding>(bindingJson.GetRawText(), _options));
								break;
							default:
								throw new JsonException();
						}
					}
					_actions.Add(actionJson.Name, new InputAction(actionJson.Name, bindings));
				}
			}
		}

		private class InputAction {
			public readonly string Name;
			private Vector2 _previousValue;
			private IEnumerable<Binding> _bindings;
			public event Action<Vector2> ValueChanged;
			public InputAction(string name, IEnumerable<Binding> bindings) {
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
