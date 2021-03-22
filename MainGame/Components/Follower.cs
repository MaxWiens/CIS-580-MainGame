using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Follower : Component {
		[JsonInclude] public ID Target;
		[JsonInclude] public float Strength;
		[JsonInclude] public float SnapDistance;
		public Vector2 PreviousTargetPosition;

		public Follower(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new Follower(entity) {
			Target = (ID)Target.Clone(),
			Strength = Strength,
			SnapDistance = SnapDistance,
			PreviousTargetPosition = PreviousTargetPosition
		};
	}
}
