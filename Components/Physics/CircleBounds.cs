using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MainGame.Components {
	public struct CircleBounds {
		[JsonInclude] public Vector2 Offset;
		[JsonInclude] public float radius;
		public ICollection<Guid> collisions;
	}	
}
