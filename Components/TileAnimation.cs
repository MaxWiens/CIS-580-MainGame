using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MainGame.Components {
	public struct TileAnimation {
		[JsonInclude] public Assets.Asset Asset;
		[JsonInclude] public int FrameIdx;
		[JsonInclude] public float FrameDelay;
		public float Timer;
	}
}
