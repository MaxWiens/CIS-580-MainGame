using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MainGame.Components {
	public struct TileAnimation {
		[JsonInclude] public Guid AssetId;
		[JsonInclude] public int FrameIdx;
		[JsonInclude] public int AnimationIdx;
		[JsonInclude] public float FrameDelay;

		public int[][] Animations;

		public float Timer;

	}

	public class Animation {
		public float Timer;
		public TileSheetAsset TileSheet;
		public float FrameDelay;

	}

	public class TileSheetAsset {
		[JsonInclude] public Texture2D Texture;
		[JsonInclude] public String TextureName;
		[JsonInclude] public Point TileDimentions;
	}

	//public class TileAnimation {
	//	public TileSheet tileSheet;
	//	[JsonInclude] public readonly Rectangle currentTileBounds;
	//	[JsonInclude] public readonly int currentTile;
	//}
}
