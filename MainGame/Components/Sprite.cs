using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Sprite : Component {
		[JsonInclude] public Texture2D Texture;
		[JsonInclude] public Vector2 Offset;
		[JsonInclude] public Color Albedo;
		[JsonInclude] public Vector2 Scale;
		[JsonInclude] public Rectangle SourceRectangle;
		public SpriteEffects SpriteEffect;

		public Sprite(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new Sprite(entity) { 
			Texture = Texture,
			Offset = Offset,
			Albedo = Albedo,
			Scale = Scale,
			SourceRectangle = SourceRectangle,
			SpriteEffect = SpriteEffect
		};
	}
}
