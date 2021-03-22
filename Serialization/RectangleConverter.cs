using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Serialization {
	public class RectangleConverter : JsonConverter<Rectangle> {
		public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			Rectangle r = new Rectangle();
			if(JsonDocument.TryParseValue(ref reader, out JsonDocument document)) {
				JsonElement root = document.RootElement;
				switch(root.ValueKind) {
					case JsonValueKind.Array:
						r.X = root[0].GetInt32();
						r.Y = root[1].GetInt32();
						r.Width = root[2].GetInt32();
						r.Height = root[3].GetInt32();
						document.Dispose();
						return r;
					case JsonValueKind.Object:
						JsonElement elm;
						if(root.TryGetProperty("X", out elm) && elm.TryGetInt32(out r.X) && root.TryGetProperty("Y", out elm) && elm.TryGetInt32(out r.Y) && root.TryGetProperty("Width", out elm) && elm.TryGetInt32(out r.Width) && root.TryGetProperty("Height", out elm) && elm.TryGetInt32(out r.Height)) {
							document.Dispose();
							return r;
						}
						break;
				}
			}
			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options) {
			writer.WriteStartArray();
			writer.WriteNumberValue(value.X);
			writer.WriteNumberValue(value.Y);
			writer.WriteNumberValue(value.Width);
			writer.WriteNumberValue(value.Height);
			writer.WriteEndArray();
		}
	}
}