using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MainGame.Components {
	public struct Follower {
		[JsonInclude] public Guid Target;
		[JsonInclude] public float Strength;
		[JsonInclude] public float SnapDistance;
		public Vector2 PreviousTargetPosition;
	}
}
