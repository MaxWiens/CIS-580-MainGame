using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Assets {
	public class TileSheet {
		public readonly Texture2D Texture;
		[JsonInclude] public readonly Point TileDimentions;
	}

	
}
