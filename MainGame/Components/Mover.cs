using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	using Assets;
	[MoonSharpUserData]
	public class Mover : Component {
		[JsonInclude] public Asset FrontWalkAnimation;
		[JsonInclude] public Asset BackWalkAnimation;
		[JsonInclude] public Asset Front;
		[JsonInclude] public Asset Back;
		[JsonInclude] public Util.Directions Direction;

		public Mover(Entity entity) : base(entity) {}

		public override IComponent Clone(Entity entity) => new Mover(entity) {
			FrontWalkAnimation = FrontWalkAnimation,
			BackWalkAnimation = BackWalkAnimation,
			Front = Front,
			Back = Back
		};
	}
}
