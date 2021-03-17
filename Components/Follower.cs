using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct Follower : IComponent {
		[JsonInclude] public ID Target;
		[JsonInclude] public float Strength;
		[JsonInclude] public float SnapDistance;
		public Vector2 PreviousTargetPosition;

		public object Clone() => new Follower() {
			Target = (ID)Target.Clone(),
			Strength = Strength,
			SnapDistance = SnapDistance,
			PreviousTargetPosition = PreviousTargetPosition
		};
	}
}
