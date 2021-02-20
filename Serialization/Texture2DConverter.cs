using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MainGame.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Serialization {
	class Texture2DConverter : JsonConverter<Texture2D> {
		private ContentManager _contentManager;
		public Texture2DConverter(ContentManager contentManager) {
			_contentManager = contentManager;
		}

		public override Texture2D Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			return _contentManager.Load<Texture2D>(reader.GetString());
		}

		public override void Write(Utf8JsonWriter writer, Texture2D value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.Name);
	}
}
