using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MainGame.Physics;
namespace MainGame.Components {
	public struct StaticBody {
		[JsonInclude] public bool IsTrigger;
		[JsonInclude] public Layer Layer;
		public ICollection<(Guid, Vector2)> Collisions;
	}
}
