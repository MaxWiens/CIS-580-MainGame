using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Systems {
	using ECS;
	using ECS.S;
	using Components;
	public class Grid : System, IUpdateable {
		public const int CHUNK_SIZE_FACTOR = 4;
		public const int CHUNK_SIZE = 1<<CHUNK_SIZE_FACTOR;
		public const int WORLD_HEIGHT = CHUNK_SIZE * CHUNK_SIZE;
		public const int TILE_SIZE = 16;
		private bool[,] _filled = new bool[500,500];
		private Guid[,] _entities = new Guid[500, 500];

		private Dictionary<Point, Guid[,]> _chunks = new Dictionary<Point, Guid[,]>();
		private Body _fallbackBody;
		public Stack<Point> PointsToDestroy = new Stack<Point>();
		Random r;
		private readonly MegaDungeonGame _game;
		private readonly tainicom.Aether.Physics2D.Dynamics.World _physicsWorld;

		public Grid(GameWorld world, MegaDungeonGame game, tainicom.Aether.Physics2D.Dynamics.World physicsWorld) : base(world) {
			r = new Random((int)DateTime.Now.Ticks);
			_game = game;
			_physicsWorld = physicsWorld;
			world.Reset += OnReset;
		}

		private void OnReset() {
			_chunks = new Dictionary<Point, Guid[,]>();
			PointsToDestroy = new Stack<Point>();
			_entities = new Guid[500, 500];
			_filled = new bool[500, 500];
		}

		public void LoadChunk(Point chunkPosition) {
			if(!_chunks.ContainsKey(chunkPosition)) {
				// load chunk
				Guid[,] chunk = new Guid[CHUNK_SIZE, CHUNK_SIZE];
				/*
				object[] components = new object[] {
					new Sprite() {
						Texture = _game.Content.Load<Texture2D>("Textures\\wood_block"),
						Albedo = Color.White,
						Scale = new Vector2(1),
						SourceRectangle = new Rectangle(0,0,16,16)
					},
					new Health(){ Value = 10},
					new Drops(){ Items = new string[]{"Assets\\Prefabs\\WoodBlockItem.json"}},
					new GridElement()
				};
				for(int i = 0; i < CHUNK_SIZE; i++) {
					for(int j = 0; j < CHUNK_SIZE; j++) {
						if(r.Next(0, 100) > 50) {
							Guid blockEID = Guid.NewGuid();
							world.MakeEntity(blockEID, Guid.Empty, components);
							//Guid blockEid = world.LoadEntityGroupFromFile(@"Assets\Prefabs\WoodBlock.json", Guid.Empty)[0];
							Body b = new Body() {
								Position = new Vector2(
									((chunkPosition.X << CHUNK_SIZE_FACTOR) + i) * TILE_SIZE,
									((chunkPosition.Y << CHUNK_SIZE_FACTOR) + j) * TILE_SIZE
								)
							};
							b.CreateRectangle(16, 16, 1, new Vector2(8, 8));
							_physicsWorld.Add(b);
							world.AddComponent(blockEID, b);
							//world.GetComponent<Body>(blockEid).Position = 
							chunk[i, j] = blockEID;
						}
					}
				}*/

				_chunks.Add(chunkPosition, chunk);
			}
		}

		public void Update(float deltaTime) {
			//var gridElementMap = world.GetEntitiesWithComponent<GridElement>();
			var chunkLoaderMap = world.GetEntitiesWithComponent<ChunkLoading>();
			var bodyMap = world.GetEntitiesWithComponent<Body>();
			Body body;
			Point chunkPosition;
			foreach(var eid in chunkLoaderMap.Keys) {
				body = bodyMap[eid];
				Point p = ToGridPosition(body.Position);
				chunkPosition = GetChunk(p);
				LoadChunk(chunkPosition + new Point(1));
				LoadChunk(chunkPosition + new Point(1,0));
				LoadChunk(chunkPosition + new Point(0, 1));
				LoadChunk(chunkPosition);
				LoadChunk(chunkPosition + new Point(0, -1));
				LoadChunk(chunkPosition + new Point(-1, 0));
				LoadChunk(chunkPosition + new Point(-1));
				LoadChunk(chunkPosition + new Point(1, -1));
				LoadChunk(chunkPosition + new Point(-1, 1));
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
					Body transform = world.TryGetComponent(eid, ref _fallbackBody, out bool isSuccessful);
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
