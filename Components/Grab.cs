using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Grab : IComponent {
		public object Clone() => this;
	}
}
