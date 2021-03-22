using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Assets;

namespace Serialization.Assets {
	using Assets;
	public class TileSheetConverter : JsonConverter<TileSheet> {

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
