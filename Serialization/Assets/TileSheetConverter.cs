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
	class TileSheetConverter : JsonConverter<TileSheet> {

		public override TileSheet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			var op = new JsonSerializerOptions(options);
			op.Converters.Remove(this);
			TileSheet tileSheet = JsonSerializer.Deserialize<TileSheet>(ref reader, op);
			tileSheet.NumTiles = new Point(tileSheet.Texture.Width / tileSheet.TileDimentions.X, tileSheet.Texture.Height / tileSheet.TileDimentions.Y);
			return tileSheet;
		}

		public override void Write(Utf8JsonWriter writer, TileSheet value, JsonSerializerOptions options) {
			var op = new JsonSerializerOptions(options);
			op.Converters.Remove(this);
			JsonSerializer.Serialize<TileSheet>(writer, value, op);
		}
	}
}
