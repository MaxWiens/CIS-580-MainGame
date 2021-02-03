using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MainGame.Components {
	public class Position2D : ECS.Component {
		[JsonInclude]
		public Vector2 Position = new Vector2();
	}
}
