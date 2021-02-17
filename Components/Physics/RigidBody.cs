using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MainGame.Physics;
namespace MainGame.Components {
	public struct RigidBody {
		public Vector2 Acceleration;
		public Vector2 Velocity;
		public Vector2 Force;
		[JsonInclude] public bool IsTrigger;
		[JsonInclude] public Layer Layer;
		public ICollection<(Guid, Vector2)> Collisions;
	}
}
