using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Collision.Shapes;
using Microsoft.Xna.Framework;

namespace MainGame.Serialization.Components {
	using Body = global::MainGame.Components.Body;
	public class BodyConverter : JsonConverter<Body> {
		public override Body Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			using JsonDocument doc = JsonDocument.ParseValue(ref reader);
			JsonElement root = doc.RootElement;
			Vector2 position = Vector2.Zero;
			if(root.TryGetProperty("Position", out JsonElement value)) {
				position = JsonSerializer.Deserialize<Vector2>(value.GetRawText(), options);
			}
			BodyType type = BodyType.Static;
			if(root.TryGetProperty("Type", out value)) {
				type = (BodyType)Enum.Parse(typeof(BodyType), value.ToString());
			}
			Body b = new Body(null) {
				FixedRotation = true,
				Position = position,
				BodyType = type
			};

			Shape shape = null;
			if(root.TryGetProperty("Fixture", out value)) {
				bool isSensor = false;
				if(value.TryGetProperty("IsSensor", out JsonElement sensorElm)) {
					isSensor = sensorElm.GetBoolean();
				}
				JsonElement elm = value.GetProperty("Shape");
				shape = JsonSerializer.Deserialize<Shape>(elm.GetRawText(), options);
				var f = b.CreateFixture(shape);
				f.IsSensor = isSensor;
			}
			return b;
		}

		public override void Write(Utf8JsonWriter writer, Body value, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}
	}
}
