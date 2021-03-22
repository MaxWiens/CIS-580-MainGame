using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components.UI {
	[MoonSharpUserData]
	public class Element : Component {
		[JsonInclude] public Vector2 RelativeAnchor;
		[JsonInclude] public Vector2 Offset;

		public Element(Entity entity) : base(entity) { }

		public IComponent Clone(Guid newEID, tainicom.Aether.Physics2D.Dynamics.World world) => this;

		public override IComponent Clone(Entity entity) {
			return new Element(entity) {
				RelativeAnchor = RelativeAnchor,
				Offset = Offset
			};
		}
	}
}
