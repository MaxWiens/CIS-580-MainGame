using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class Grid : UpdateSystem {
		public const int GRID_SIZE = 16;
		private bool[,] _filled = new bool[16,9];
		private Guid[,] _entities = new Guid[16, 9];

		public Grid(ZaWarudo world) : base(world) { }
		private Transform2D _t;

		public Stack<Point> PointsToDestroy = new Stack<Point>();

		public override void Update(float deltaTime) {
			while(PointsToDestroy.TryPop(out Point p)) {
				if(p.X >= 0 && p.X < 16 && p.Y >= 0 && p.Y < 9 && _filled[p.X, p.Y]) {
					world.DestroyEntity(_entities[p.X, p.Y]);
				}
			}

			var entities = world.GetEntitiesWithComponent<GridAligned>();
			ref Transform2D transform2D = ref _t;

			if(entities!= null) {
				var eids = entities.Keys;
				_filled = new bool[16, 9];
				_entities = new Guid[16, 9];



				foreach(var eid in eids) {
					ref GridAligned g = ref world.GetComponent<GridAligned>(eid);
					Point p;
					if(world.TryGetComponent<Transform2D>(eid, ref transform2D)) {
						p = g.GridPosition = ToGridPosition(transform2D.Position);
					} else {
						p = g.GridPosition;
					}

					if(p.X >= 0 && p.X < 16 && p.Y >= 0 && p.Y < 9 && !_filled[p.X,p.Y]) {
						_filled[p.X, p.Y] = true;
						_entities[p.X, p.Y] = eid;
					}
						
				}
			}
		}

		public bool IsCellFilled(int x, int y) {
			if(x >= 0 && x < 16 && y >= 0 && y < 9)
				return _filled[x, y];
			return true;
		}
		public bool IsCellFilled(Point p) {
			if(p.X >= 0 && p.X < 16 && p.Y >= 0 && p.Y < 9)
				return _filled[p.X, p.Y];
			return true;
		}

		public static Point ToGridPosition(Vector2 vector)
			=> new Point((int)vector.X / GRID_SIZE, (int)vector.Y / GRID_SIZE);

		public static Vector2 NearestGridPosition(Vector2 vector)
			=> new Vector2((int)vector.X/ GRID_SIZE, (int)vector.Y / GRID_SIZE)* GRID_SIZE;
	}
}
