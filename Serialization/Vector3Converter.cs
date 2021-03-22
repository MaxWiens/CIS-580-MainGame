using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Serialization {
	public class Vector3Converter : JsonConverter<Vector3> {
		public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			Vector3 v = new Vector3();
			if(JsonDocument.TryParseValue(ref reader, out JsonDocument document)) {
				JsonElement root = document.RootElement;
				JsonElement elm;
				if(root.TryGetProperty("X", out elm) && elm.TryGetSingle(out v.X) && root.TryGetProperty("Y", out elm) && elm.TryGetSingle(out v.Y) && root.TryGetProperty("Z", out elm) && elm.TryGetSingle(out v.Z)) {
					document.Dispose();
					return v;
				}
			}
			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options) {
			writer.WriteStartObject();
			writer.WritePropertyName("X");
			writer.WriteNumberValue(value.X);
			writer.WritePropertyName("Y");
			writer.WriteNumberValue(value.Y);
			writer.WritePropertyName("Z");
			writer.WriteNumberValue(value.Z);
			writer.WriteEndObject();
		}
	}
}
