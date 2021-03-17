using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct Inventory : IComponent {
		public object Clone() => this;
	}
}
