using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MainGame.Serialization {
	public class ColorConverter : JsonConverter<Color> {
		public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			JsonDocument document = JsonDocument.ParseValue(ref reader);
			var root = document.RootElement;
			switch(root.ValueKind) {
				case JsonValueKind.Array:
					var enumerator = root.EnumerateArray();
					byte r, g, b, a;
					if(enumerator.MoveNext() && enumerator.Current.TryGetByte(out r) && enumerator.MoveNext() && enumerator.Current.TryGetByte(out g) && enumerator.MoveNext() && enumerator.Current.TryGetByte(out b)) {
						if(!enumerator.MoveNext() || !enumerator.Current.TryGetByte(out a)) {
							a = 255;
						}
						document.Dispose();
						return new Color(r, g, b, a);
					}
					throw new JsonException();
				case JsonValueKind.String:
					string s = root.GetString();
					System.Drawing.Color color = System.Drawing.Color.FromName(s);
					document.Dispose();
					return new Color(color.R,color.G, color.B, color.A);
			}
			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) {
			writer.WriteStartArray();
			writer.WriteNumberValue(value.R);
			writer.WriteNumberValue(value.G);
			writer.WriteNumberValue(value.B);
			writer.WriteNumberValue(value.A);
			writer.WriteEndArray();
		}
	}
}