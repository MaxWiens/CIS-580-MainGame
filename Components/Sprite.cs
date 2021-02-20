using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
namespace MainGame.Components {
	public struct Sprite {
		[JsonInclude] public Texture2D Texture;
		[JsonInclude] public Vector2 Offset;
		[JsonInclude] public Color Albedo;
		[JsonInclude] public Vector2 Scale;
		[JsonInclude] public Rectangle SourceRectangle;
		public SpriteEffects SpriteEffect;
	}
}
