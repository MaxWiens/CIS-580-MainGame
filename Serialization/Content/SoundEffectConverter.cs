using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Serialization.Content {
	public class SoundEffectConverter : JsonConverter<SoundEffect> {
		private readonly ContentManager _contentManager;
		public SoundEffectConverter(ContentManager contentManager) {
			_contentManager = contentManager;
		}

		public override SoundEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			SoundEffect e = _contentManager.Load<SoundEffect>(reader.GetString());
			return e;
		}

		public override void Write(Utf8JsonWriter writer, SoundEffect value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.Name);
	}
}
