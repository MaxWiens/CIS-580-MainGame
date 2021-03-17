using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	using Assets;
	[MoonSharpUserData]
	public struct Mover : IComponent {
		[JsonInclude] public Asset FrontWalkAnimation;
		[JsonInclude] public Asset BackWalkAnimation;
		[JsonInclude] public Asset Front;
		[JsonInclude] public Asset Back;
		[JsonInclude] public Util.Directions Direction;

		public object Clone() => this;
	}
}
