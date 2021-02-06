using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MainGame.Systems {
	using ECS;
	public class Grid : System {
		public const int GRID_SIZE = 16;
		//private Guid[,] = new Grid[,];
		public Grid(ZaWarudo world) : base(world) {

		}

		public static Point ToGridPosition(Vector2 vector)
			=> new Point((int)vector.X / GRID_SIZE, (int)vector.Y / GRID_SIZE);

		public static Vector2 NearestGridPosition(Vector2 vector)
			=> new Vector2((int)vector.X/ GRID_SIZE, (int)vector.Y / GRID_SIZE)* GRID_SIZE;

	}
}
