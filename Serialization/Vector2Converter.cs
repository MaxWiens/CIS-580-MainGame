using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MainGame.Serialization {
	class Vector2Converter : JsonConverter<Vector2> {
		public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			Vector2 v = new Vector2();
			if(JsonDocument.TryParseValue(ref reader, out JsonDocument document)) {
				JsonElement root = document.RootElement;
				JsonElement elm;
				if(root.TryGetProperty("X", out elm) && elm.TryGetSingle(out v.X) && root.TryGetProperty("Y", out elm) && elm.TryGetSingle(out v.Y)) {
					document.Dispose();
					return v;
				}
			}
			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options) {
			writer.WriteStartObject();
			writer.WritePropertyName("X");
			writer.WriteNumberValue(value.X);
			writer.WritePropertyName("Y");
			writer.WriteNumberValue(value.Y);
			writer.WriteEndObject();
		}
	}
}