using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MainGame.Components {
	using Assets;
	public struct Mover {
		[JsonInclude] public Asset FrontWalkAnimation;
		[JsonInclude] public Asset BackWalkAnimation;
		[JsonInclude] public Asset Front;
		[JsonInclude] public Asset Back;
		[JsonInclude] public Util.Directions Direction;
	}
}
