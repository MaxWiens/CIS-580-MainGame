using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Serialization {
	public class PointConverter : JsonConverter<Point> {
		public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			Point p = new Point();
			if(JsonDocument.TryParseValue(ref reader, out JsonDocument document)) {
				JsonElement root = document.RootElement;
				switch(root.ValueKind) {
					case JsonValueKind.Array:
						p.X = root[0].GetInt32();
						p.Y = root[1].GetInt32();
						document.Dispose();
						return p;
					case JsonValueKind.Object:
						JsonElement elm;
						if(root.TryGetProperty("X", out elm) && elm.TryGetInt32(out p.X) && root.TryGetProperty("Y", out elm) && elm.TryGetInt32(out p.Y)) {
							document.Dispose();
							return p;
						}
						break;
				}
			}
			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options) {
			writer.WriteStartArray();
			writer.WriteNumberValue(value.X);
			writer.WriteNumberValue(value.Y);
			writer.WriteEndArray();
		}
	}
}