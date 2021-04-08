using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components.UI {
	[MoonSharpUserData]
	public class Element : Component {
		[JsonInclude] public Vector2 Anchor;
		[JsonInclude] public Vector2 Offset;

		public Element(Entity entity) : base(entity) { }

		[MessageHandler]
		public bool OnEnable(Message message) {
			Entity.World.GetSystem<Systems.UI.ElementRepositionSystem>().RepositionElement(this);
			return true;
		}

		public override IComponent Clone(Entity entity) {
			return new Element(entity) {
				Anchor = Anchor,
				Offset = Offset
			};
		}
	}
}
