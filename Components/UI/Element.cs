using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components.UI {
	[MoonSharpUserData]
	public struct Element : IComponent {
		Vector2 RelativeAnchor;
		Vector2 Offset;

		public object Clone() => this;
	}
}
