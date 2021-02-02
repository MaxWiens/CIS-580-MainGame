using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MainGame.JsonConverters {
	class Vector3Converter : JsonConverter<Vector3> {
		public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			Vector3 v = new Vector3();
			string propertyName = string.Empty;
			while(reader.Read()) {
				if(reader.TokenType == JsonTokenType.EndObject) {
					return v;
				}
				if(reader.TokenType == JsonTokenType.PropertyName) {
					propertyName = reader.GetString();
					reader.Read();
					switch(propertyName) {
						case "X":
							v.X = reader.GetSingle();
							break;
						case "Y":
							v.Y = reader.GetSingle();
							break;
						case "Z":
							v.Z = reader.GetSingle();
							break;
					}
				}
			}
			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options) {
			writer.WriteNumberValue(value.X);
			writer.WriteNumberValue(value.Y);
		}
	}
}
