using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MainGame.Components {
	public struct RectCollider {
		[JsonInclude] public bool CollidesWithSelf;
		[JsonInclude] public Vector2 Offset;
		[JsonInclude] public Vector2 Dimentions;
		public ICollection<Guid> collisions;
	}	
}
