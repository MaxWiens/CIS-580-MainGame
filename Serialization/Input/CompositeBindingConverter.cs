using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MainGame.Serialization {
	using Input;
	using Util;
	using BindingReader = Func<Input.InputState, Vector2>;
	class CompositeBindingConverter : JsonConverter<CompositeBinding> {
		public override CompositeBinding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			JsonDocument document = JsonDocument.ParseValue(ref reader);
			JsonElement root = document.RootElement;
			string[] bindingNames = new string[4];
			BindingReader upClosure = null;
			BindingReader downClosure = null;
			BindingReader leftClosure = null;
			BindingReader rightClosure = null;

			switch(root.ValueKind) {
				case JsonValueKind.Object:
					Directions directionsFound = Directions.None;
					JsonElement directionProperty;
					if(root.TryGetProperty("Up", out directionProperty)) {
						bindingNames[0] = directionProperty.GetString();
						upClosure = Binding.GetInputClosure(bindingNames[0]);
						directionsFound |= Directions.Up;
					}
					if(root.TryGetProperty("Down", out directionProperty)) {
						bindingNames[1] = directionProperty.GetString();
						downClosure = Binding.GetInputClosure(bindingNames[1]);
						directionsFound |= Directions.Down;
					}
					if(root.TryGetProperty("Left", out directionProperty)) {
						bindingNames[2] = directionProperty.GetString();
						leftClosure = Binding.GetInputClosure(bindingNames[2]);
						directionsFound |= Directions.Left;
					}
					if(root.TryGetProperty("Right", out directionProperty)) {
						bindingNames[3]  = directionProperty.GetString();
						rightClosure = Binding.GetInputClosure(bindingNames[3]);
						directionsFound |= Directions.Right;
					}
					switch(directionsFound) {
						case Directions.Up:
							return new CompositeBinding(bindingNames, (state)=>new Vector2(0f, upClosure(state).Y));
						case Directions.Down:
							return new CompositeBinding(bindingNames, (state)=>new Vector2(0f, -downClosure(state).Y));
						case Directions.Left:
							return new CompositeBinding(bindingNames, (state)=>new Vector2(-leftClosure(state).X, 0f));
						case Directions.Right:
							return new CompositeBinding(bindingNames, (state)=>new Vector2(rightClosure(state).X, 0f));
						case Directions.Up | Directions.Down:
							return new CompositeBinding(bindingNames, (state)=>new Vector2(0f, upClosure(state).Y - downClosure(state).Y));
						case Directions.Up | Directions.Left:
							return new CompositeBinding(bindingNames, (state)=>new Vector2(-leftClosure(state).X, upClosure(state).Y));
						case Directions.Up | Directions.Right:
							return new CompositeBinding(bindingNames, (state)=>new Vector2(rightClosure(state).X, upClosure(state).Y));
						case Directions.Down | Directions.Left:
							return new CompositeBinding(bindingNames, (state)=>new Vector2(-leftClosure(state).X, -downClosure(state).Y));
						case Directions.Down | Directions.Right:
							return new CompositeBinding(bindingNames, (state) => new Vector2(rightClosure(state).X, -downClosure(state).Y));
						case Directions.Left | Directions.Right:
							return new CompositeBinding(bindingNames, (state) => new Vector2(rightClosure(state).X-leftClosure(state).X,0f));
						case Directions.Up | Directions.Down | Directions.Left:
							return new CompositeBinding(bindingNames, (state) => new Vector2(-leftClosure(state).X, upClosure(state).Y - downClosure(state).Y));
						case Directions.Up | Directions.Down | Directions.Right:
							return new CompositeBinding(bindingNames, (state) => new Vector2(rightClosure(state).X, upClosure(state).Y - downClosure(state).Y));
						case Directions.Up | Directions.Left | Directions.Right:
							return new CompositeBinding(bindingNames, (state) => new Vector2(rightClosure(state).X - leftClosure(state).X, upClosure(state).Y));
						case Directions.Down | Directions.Left | Directions.Right:
							return new CompositeBinding(bindingNames, (state) => new Vector2(rightClosure(state).X - leftClosure(state).X, -downClosure(state).Y));
						case Directions.All:
							return new CompositeBinding(bindingNames, (state) => new Vector2(rightClosure(state).X-leftClosure(state).X, upClosure(state).Y - downClosure(state).Y));
					}

					break;
				case JsonValueKind.Array:
					var enumerator = root.EnumerateArray();
					if(!enumerator.MoveNext()) throw new JsonException();
					bindingNames[0] = enumerator.Current.GetString();
					if(!enumerator.MoveNext()) throw new JsonException();
					bindingNames[1] = enumerator.Current.GetString();
					if(!enumerator.MoveNext()) throw new JsonException();
					bindingNames[2] = enumerator.Current.GetString();
					if(!enumerator.MoveNext()) throw new JsonException();
					bindingNames[3] = enumerator.Current.GetString();

					upClosure = Binding.GetInputClosure(bindingNames[0]);
					downClosure = Binding.GetInputClosure(bindingNames[1]);
					leftClosure = Binding.GetInputClosure(bindingNames[2]);
					rightClosure = Binding.GetInputClosure(bindingNames[3]);

					return new CompositeBinding(bindingNames, (state) => new Vector2(rightClosure(state).X- leftClosure(state).X, upClosure(state).Y - downClosure(state).Y));
				default:
					throw new JsonException("Not composite binding");
			}

			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, CompositeBinding value, JsonSerializerOptions options) {
			string upName = value.BindingNames[0];
			string downName = value.BindingNames[1];
			string leftName = value.BindingNames[2];
			string rightName = value.BindingNames[3];
			if(upName != null && downName != null && leftName != null && rightName != null) {
				writer.WriteStartArray();
				writer.WriteStringValue(upName);
				writer.WriteStringValue(downName);
				writer.WriteStringValue(leftName);
				writer.WriteStringValue(rightName);
				writer.WriteEndArray();
			} else {
				writer.WriteStartObject();
				if(upName != null)
					writer.WriteString("Up", upName);
				if(downName != null)
					writer.WriteString("Down", downName);
				if(leftName != null)
					writer.WriteString("Left", leftName);
				if(rightName != null)
					writer.WriteString("Right", rightName);
				writer.WriteEndObject();
			}
		}
	}
}
