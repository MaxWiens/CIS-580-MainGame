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
	using Assets;
	class AssetConverter : JsonConverter<Asset> {
		private IDictionary<Guid, Asset> _assets;
		public AssetConverter(IDictionary<Guid,Asset> assets) {
			_assets = assets;
		}

		public override Asset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			return _assets[reader.GetGuid()];
		}

		public override void Write(Utf8JsonWriter writer, Asset value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ID);
	}
}
