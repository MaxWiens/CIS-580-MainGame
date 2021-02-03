using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
namespace MainGame.Components {
	public class Color : ECS.Component {
		[JsonInclude]
		public Microsoft.Xna.Framework.Color Value = Microsoft.Xna.Framework.Color.White;
	}
}
