using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using Microsoft.Xna.Framework.Graphics;
using ECS;
namespace MainGame.Components.UI {
	[MoonSharpUserData]
	public class TextBox : Component {
		[JsonInclude] public Vector2 RelativeAnchor;
		[JsonInclude] public Vector2 Offset;
		[JsonInclude] public string Text;
		[JsonInclude] public Rectangle Box;
		[JsonInclude] public Color Color;
		[JsonInclude] public SpriteFont Font;
		[JsonInclude] public bool IsCentered;
		[JsonInclude] public float Scale = 1f;

		public TextBox(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) {
			return new TextBox(entity) {
				RelativeAnchor = RelativeAnchor,
				Offset = Offset
			};
		}
	}
}
