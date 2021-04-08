using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components.UI {
	[MoonSharpUserData]
	public class Sprite : Component {
		public Sprite(Entity entity) : base(entity) { }

		[JsonInclude] public Texture2D Texture;
		[JsonInclude] public Vector2 Offset;
		[JsonInclude] public Vector2 Anchor;
		[JsonInclude] public Color Albedo = Color.White;
		[JsonInclude] public Vector2 Scale = Vector2.One;
		[JsonInclude] public Rectangle SourceRectangle;
		public SpriteEffects SpriteEffect;

		public override IComponent Clone(Entity entity)
			=> new Sprite(entity) {
				Texture = Texture,
				Offset = Offset,
				Albedo = Albedo,
				Scale = Scale,
				SourceRectangle = SourceRectangle,
				SpriteEffect = SpriteEffect,
				Anchor = Anchor,
			};
	}
}
