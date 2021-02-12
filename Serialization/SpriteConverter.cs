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
	class SpriteConverter : JsonConverter<Components.Sprite> {
		private ContentManager _contentManager;
		public SpriteConverter(ContentManager contentManager) {
			_contentManager = contentManager;
		}

		public override Sprite Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			JsonSerializerOptions ops = new JsonSerializerOptions(options);
			ops.Converters.Remove(this);
			Sprite s = (Sprite)JsonSerializer.Deserialize(ref reader, typeToConvert, ops);
			s.Texture = _contentManager.Load<Texture2D>(s.TextureName);
			return s;
		}

		public override void Write(Utf8JsonWriter writer, Sprite value, JsonSerializerOptions options)
			=> JsonSerializer.Serialize(writer, value);
	}
}
