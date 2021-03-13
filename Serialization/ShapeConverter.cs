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
	class ShapeConverter : JsonConverter<Shape> {
		public override Shape Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			using JsonDocument doc = JsonDocument.ParseValue(ref reader);
			JsonElement root = doc.RootElement;
			float density = 1;
			if(root.TryGetProperty("Density", out JsonElement elm)) {
				density = elm.GetSingle();
			}
			Vector2 offset = Vector2.Zero;
			if(root.TryGetProperty("Offset", out elm)) {
				offset = JsonSerializer.Deserialize<Vector2>(elm.GetRawText(), options);
			}
			float offsetX = offset.X;
			float offsetY = offset.Y;
			var typePropElm = root.GetProperty("Type");
			switch(typePropElm.GetString()) {
				case "Rectangle":
					Vector2 halfDimentions = JsonSerializer.Deserialize<Vector2>(root.GetProperty("Dimentions").GetRawText(), options)/2;
					return new PolygonShape(new Vertices(new Vector2[]{
						new Vector2(offsetX+halfDimentions.X, offsetY-halfDimentions.Y),
						new Vector2(offsetX+halfDimentions.X, offsetY+halfDimentions.Y),
						new Vector2(offsetX-halfDimentions.X, offsetY+halfDimentions.Y),
						new Vector2(offsetX-halfDimentions.X, offsetY-halfDimentions.Y)
					}), density);
				case "Polygon":
					break;
				case "Edge":
					break;
				case "Circle":
					float radius = root.GetProperty("Radius").GetSingle();
					var c = new CircleShape(radius, density);
					return c;
				case "Chain":
					break;
			}


			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, Shape value, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}
	}
}
