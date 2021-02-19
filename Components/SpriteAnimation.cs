using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
namespace MainGame.Components {
	public struct SpriteAnimation {
		public Texture2D SpriteSheet;
		[JsonInclude] public bool AutoOffset;
		[JsonInclude] public Vector2 Offset;
		[JsonInclude] public string TextureName;
		[JsonInclude] public Color Albedo;
		[JsonInclude] public Vector2 Scale;
		[JsonInclude] public int AnimationIdx;
		[JsonInclude] public bool Loop;
		[JsonInclude] public Rectangle[][] Tiles;
		public Rectangle[] currentAnimation;
	}

	/*public struct Animation {
		int idx;

	}*/
}
