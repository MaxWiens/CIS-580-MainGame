using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
namespace MainGame.Components.UI {
	public struct Sprite {
		[JsonConstructor]
		public Sprite(Texture2D Texture, Rectangle SourceRectangle) {
			this.Texture = Texture;
			this.SourceRectangle = SourceRectangle;
			Offset = Vector2.Zero;
			Albedo = Color.White;
			Scale = Vector2.One;
			SpriteEffect = SpriteEffects.None;
		}

		[JsonInclude] public Texture2D Texture;
		[JsonInclude] public Vector2 Offset;
		[JsonInclude] public Color Albedo;
		[JsonInclude] public Vector2 Scale;
		[JsonInclude] public Rectangle SourceRectangle;
		public SpriteEffects SpriteEffect;
	}
}
