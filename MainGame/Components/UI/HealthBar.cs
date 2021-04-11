using MoonSharp.Interpreter;
using System.Text.Json.Serialization;
using ECS;
using System;
namespace MainGame.Components.UI {
	[MoonSharpUserData]
	public class HealthBar : Component {
		public HealthBar(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new HealthBar(entity);
	}
}
