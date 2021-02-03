using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
namespace MainGame.Util {
	public static class XnaExtensions {
		public static float AsFloat(this Vector2 value)
			=> value.X != 0 ? value.X : value.Y;
		public static bool AsBool(this Vector2 value)
			=> value != Vector2.Zero;
	}
}
