using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class Grid : UpdateSystem {
		public const int CHUNK_SIZE_FACTOR = 4;
		public const int CHUNK_SIZE = 1<<4;
		public const int WORLD_HEIGHT = CHUNK_SIZE * CHUNK_SIZE;
		public const int TILE_SIZE = 16;
		private bool[,] _filled = new bool[500,500];
		private Guid[,] _entities = new Guid[500, 500];

		public Dictionary<Point, Guid[,]> _chunks = new Dictionary<Point, Guid[,]>();
		private Transform2D _fallbackT2D;
		public Stack<Point> PointsToDestroy = new Stack<Point>();

		public Grid(ZaWarudo world) : base(world) { }

		public override void Update(float deltaTime) {
			var gridElementMap = world.GetEntitiesWithComponent<GridElement>();
			var chunkLoaderMap = world.GetEntitiesWithComponent<ChunkLoading>();
			var transMap = world.GetEntitiesWithComponent<Transform2D>();
			Transform2D trans;
			Point position;
			Point chunkPosition;
			foreach(var eid in chunkLoaderMap.Keys) {
				trans = transMap[eid];
				position = ToGridPosition(trans.Position);
				chunkPosition = GetChunk(position);
			}

			while(PointsToDestroy.TryPop(out Point p)) {
				if(p.X >= 0 && p.X < 500 && p.Y >= 0 && p.Y < 500 && _filled[p.X, p.Y]) {
					world.DestroyEntity(_entities[p.X, p.Y]);
				}
			}

			var entities = world.GetEntitiesWithComponent<GridElement>();

			if(entities!= null) {
				var eids = entities.Keys;
				_filled = new bool[500, 500];
				_entities = new Guid[500, 500];

				foreach(var eid in eids) {
					ref GridElement g = ref world.GetComponent<GridElement>(eid);
					Point p;
					Transform2D transform = world.TryGetComponent(eid, ref _fallbackT2D, out bool isSuccessful);
					if(isSuccessful) {
						p = g.Position = ToGridPosition(transform.Position);
					} else {
						p = g.Position;
					}

					if(p.X >= 0 && p.X < 500 && p.Y >= 0 && p.Y < 500 && !_filled[p.X,p.Y]) {
						_filled[p.X, p.Y] = true;
						_entities[p.X, p.Y] = eid;
					}
						
				}
			}
		}
		public bool GetEntityAt(int x, int y, out Guid eid) {
			if(x >= 0 && x < 500 && y >= 0 && y < 500 && _filled[x, y]) {
				eid = _entities[x, y];
				return true;
			}
			eid = Guid.Empty;
			return false;
		}
		public bool GetEntityAt(Point p, out Guid eid) => GetEntityAt(p.X ,p.Y, out eid);

		public bool IsCellFilled(int x, int y) {
			if(x >= 0 && x < 500 && y >= 0 && y < 500)
				return _filled[x, y];
			return true;
		}
		public bool IsCellFilled(Point p) {
			if(p.X >= 0 && p.X < 500 && p.Y >= 0 && p.Y < 500)
				return _filled[p.X, p.Y];
			return true;
		}

		public static Point ToGridPosition(Vector2 vector)
			=> new Point((int)vector.X / TILE_SIZE, (int)vector.Y / TILE_SIZE);

		public static Point GetChunk(Point gridPosition)
			=> new Point(gridPosition.X>>CHUNK_SIZE_FACTOR, gridPosition.Y>>CHUNK_SIZE_FACTOR);

		public static Point GetChunk(Vector2 realPosition)
			=> new Point(((int)realPosition.X/TILE_SIZE) >> CHUNK_SIZE_FACTOR, ((int)realPosition.Y/TILE_SIZE)>> CHUNK_SIZE_FACTOR);

		public static Vector2 NearestGridPosition(Vector2 vector)
			=> new Vector2(((int)vector.X/TILE_SIZE)* TILE_SIZE, ((int)vector.Y/TILE_SIZE)*TILE_SIZE);
	}
}
