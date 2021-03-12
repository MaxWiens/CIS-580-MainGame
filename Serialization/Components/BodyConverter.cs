using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Collision.Shapes;
using Microsoft.Xna.Framework;
namespace MainGame.Serialization {
	using MainGame.Components;
	public class BodyConverter : JsonConverter<Body> {
		private readonly World _world;
		public BodyConverter(World world) {
			_world = world;
		}

		public override Body Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			using JsonDocument doc = JsonDocument.ParseValue(ref reader);
			JsonElement root = doc.RootElement;
			Vector2 position = Vector2.Zero;
			if(root.TryGetProperty("Position", out JsonElement value)) {
				position = JsonSerializer.Deserialize<Vector2>(value.GetRawText(), options);
			}
			BodyType type = BodyType.Static;
			if(root.TryGetProperty("BodyType", out value)) {
				type = (BodyType)Enum.Parse(typeof(BodyType), value.ToString());
			}
			Body b = new Body() {
				FixedRotation = true,
				Position = position,
				BodyType = type
			};

			Shape shape = null;
			bool isSensor = false;
			if(root.TryGetProperty("Fixture", out value)) {
				if(value.TryGetProperty("IsSensor", out JsonElement sensorElm)) {
					isSensor = sensorElm.GetBoolean();
				}
				JsonElement elm = value.GetProperty("Shape");
				shape = JsonSerializer.Deserialize<Shape>(elm.GetRawText(), options);
				var f = b.CreateFixture(shape);
				f.IsSensor = isSensor;
			}
			_world.Add(b);
			return b;
		}

		public override void Write(Utf8JsonWriter writer, Body value, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}
	}
}
