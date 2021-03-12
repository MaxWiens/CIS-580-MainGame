using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
namespace Util {
	[Flags]
	public enum Directions : byte{
		None = 0,
		Up    = 0b0001,
		Down  = 0b0010,
		Left  = 0b0100,
		Right = 0b1000,
		All = Up | Down | Left | Right
	}
	
	public static class Direction {
		public static Vector2 ToVector2(this Directions direction)
			=> new Vector2(
				((direction&Directions.Down)!= Directions.None? 1:0) - ((direction & Directions.Up) != Directions.None ? 1 : 0),
				((direction & Directions.Right) != Directions.None ? 1 : 0) - ((direction & Directions.Left) != Directions.None ? 1 : 0)
			);
	}
}
