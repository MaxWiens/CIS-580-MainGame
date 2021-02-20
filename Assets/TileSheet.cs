using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Assets {
	public class TileSheet : Asset {
		[JsonInclude] public Texture2D Texture;
		[JsonInclude] public Point TileDimentions;
		public Point NumTiles;
	}
}