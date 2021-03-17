using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct Sprite : IComponent{
		[JsonInclude] public Texture2D Texture;
		[JsonInclude] public Vector2 Offset;
		[JsonInclude] public Color Albedo;
		[JsonInclude] public Vector2 Scale;
		[JsonInclude] public Rectangle SourceRectangle;
		public SpriteEffects SpriteEffect;

		public object Clone() => this;
	}
}
