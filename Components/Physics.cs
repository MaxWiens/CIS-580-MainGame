using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;


namespace MainGame.Components {
	public struct Physics {
		[JsonInclude] public Vector2 Velocity;
		[JsonInclude] public Vector2 Forces;
	}
}
