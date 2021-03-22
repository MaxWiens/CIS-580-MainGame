using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Inventory : Component {
		public Inventory(Entity entity) : base(entity) {}

		public override IComponent Clone(Entity entity) => new Inventory(entity) { };
	}
}
