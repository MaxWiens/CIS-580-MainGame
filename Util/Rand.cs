using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
namespace Util {
	public static class Rand {
		private static readonly Random _r;
		static Rand() {
			_r = new Random((int)DateTime.Now.Ticks);
		}

		public static float Float()
			=> (float)_r.NextDouble();

		public static Vector2 UnitCircle() {
			double angle = _r.NextDouble()*(System.Math.PI*2f);
			return new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle));
		}


	}
}
