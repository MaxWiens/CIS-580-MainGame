using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MainGame.JsonConverters {
	class Vector2Converter : JsonConverter<Vector2> {
		public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			Vector2 v = new Vector2();
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
					}
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
