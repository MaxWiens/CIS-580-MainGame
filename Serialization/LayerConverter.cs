using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MainGame.Physics;

namespace MainGame.Serialization {
	class LayerConverter : JsonConverter<Layer> {
		public override Layer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> Enum.Parse<Layer>(reader.GetString());

		public override void Write(Utf8JsonWriter writer, Layer value, JsonSerializerOptions options)
			=> writer.WriteStringValue(Enum.GetName(typeof(Layer), value));
	}
}