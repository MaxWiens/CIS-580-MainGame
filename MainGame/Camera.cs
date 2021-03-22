using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
namespace MainGame {
	public class Camera {
		public Vector2 Position;
		public Point Resolution;
		public Vector2 Center => Position - (Resolution.ToVector2() * 0.5f);
	}
}
