using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Systems {
	using ECS;
	using ECS.S;
	using Components;
	public class TileSystem : System, IUpdateable {
		public const int CHUNK_SIZE_FACTOR = 4;
		public const int CHUNK_SIZE = 1<<CHUNK_SIZE_FACTOR;
		public const int WORLD_HEIGHT = CHUNK_SIZE * CHUNK_SIZE;
		public const int TILE_SIZE = 16;
		private Dictionary<Point, Chunk> _chunks = new Dictionary<Point, Chunk>();
		public Stack<Point> PointsToDestroy = new Stack<Point>();
		Random r;
		private readonly MegaDungeonGame _game;
		private readonly tainicom.Aether.Physics2D.Dynamics.World _physicsWorld;

		public TileSystem(GameWorld world, MegaDungeonGame game, tainicom.Aether.Physics2D.Dynamics.World physicsWorld) : base(world) {
			r = new Random((int)DateTime.Now.Ticks);
			_game = game;
			_physicsWorld = physicsWorld;
			world.Reset += OnReset;
		}

		public void Update(float deltaTime) {
			//var gridElementMap = world.GetEntitiesWithComponent<GridElement>();
			var chunkLoaderMap = world.GetEntitiesWithComponent<ChunkLoading>();
			var bodyMap = world.GetEntitiesWithComponent<Body>();
			Body body;
			Point chunkPosition;

			foreach(var posChunk in _chunks) {
				posChunk.Value.IsActive = false;
			}

			foreach(var eid in chunkLoaderMap.Keys) {
				body = bodyMap[eid];
				Point p = ToTilePosition(body.Position);
				chunkPosition = GetChunk(p);
				LoadChunk(chunkPosition + new Point(1));
				LoadChunk(chunkPosition + new Point(1, 0));
				LoadChunk(chunkPosition + new Point(0, 1));
				LoadChunk(chunkPosition);
				LoadChunk(chunkPosition + new Point(0, -1));
				LoadChunk(chunkPosition + new Point(-1, 0));
				LoadChunk(chunkPosition + new Point(-1));
				LoadChunk(chunkPosition + new Point(1, -1));
				LoadChunk(chunkPosition + new Point(-1, 1));
			}

			foreach(var posChunk in _chunks) {
				if(!posChunk.Value.IsActive) {
					UnloadChunk(posChunk.Key);
				}
			}
		}

		private void OnReset() {
			_chunks = new Dictionary<Point, Chunk>();
			PointsToDestroy = new Stack<Point>();
		}

		public void LoadChunk(Point chunkPosition) {
			if(!_chunks.TryGetValue(chunkPosition, out Chunk chunk)) {
				// load chunk
				Guid[,] tiles = new Guid[CHUNK_SIZE, CHUNK_SIZE];
				
				object[] components = new object[] {
					new Sprite() {
						Texture = _game.Content.Load<Texture2D>("Textures\\wood_block"),
						Albedo = Color.White,
						Scale = new Vector2(1),
						SourceRectangle = new Rectangle(0,0,16,16)
					},
					new GridElement(){},
					new Health(){ Value = 10},
					new Drops(){ Items = new string[]{"Assets\\Prefabs\\WoodBlockItem.json"}},
				};
				
				int topSize = r.Next(0, 6);
				int bottomSize = r.Next(CHUNK_SIZE-6, CHUNK_SIZE+1);

				int leftSize = r.Next(0, 6);
				int rightSize = r.Next(CHUNK_SIZE-6, CHUNK_SIZE+1);

				for(int y =0; y < topSize; y++) {
					for(int x = 0; x < CHUNK_SIZE; x++) {
						if(x == 7 || x == 8) continue;
						CreateBrick(x, y, chunkPosition.X, chunkPosition.Y, tiles, components);
					}
				}

				for(int y = bottomSize; y < CHUNK_SIZE; y++) {
					for(int x = 0; x < CHUNK_SIZE; x++) {
						if(x == 7 || x == 8) continue;
						CreateBrick(x, y, chunkPosition.X, chunkPosition.Y, tiles, components);
					}
				}

				for(int y = topSize; y < bottomSize; y++) {
					if(y == 7 || y == 8) continue;
					for(int x = 0; x < leftSize; x++) {
						CreateBrick(x, y, chunkPosition.X, chunkPosition.Y, tiles, components);
					}
				}

				for(int y = topSize; y < bottomSize; y++) {
					if(y == 7 || y == 8) continue;
					for(int x = rightSize; x < CHUNK_SIZE; x++) {
						CreateBrick(x, y, chunkPosition.X, chunkPosition.Y, tiles, components);
					}
				}
				_chunks.Add(chunkPosition, new Chunk(tiles, true));
			} else {
				chunk.IsActive = true;
			}
		}

		private void UnloadChunk(Point p) {
			Chunk chunk = _chunks[p];
			foreach(Guid eid in chunk.Tiles) {
				if(eid != Guid.Empty)
					world.DestroyEntity(eid);
			}
			_chunks.Remove(p);
		}

		private void CreateBrick(int x, int y, int chunkX, int chunkY, Guid[,] chunk, object[] components) {
			Guid blockEID = Guid.NewGuid();
			world.MakeEntity(blockEID, Guid.Empty, components);
			Body b = new Body() {
				Position = new Vector2(
					((chunkX << CHUNK_SIZE_FACTOR) + x) * TILE_SIZE,
					((chunkY << CHUNK_SIZE_FACTOR) + y) * TILE_SIZE
				)
			};
			b.CreateRectangle(TILE_SIZE, TILE_SIZE, 1, new Vector2(TILE_SIZE / 2, TILE_SIZE/2));
			_physicsWorld.Add(b);
			b.Tag = blockEID;
			world.AddComponent(blockEID, b);
			//world.GetComponent<Body>(blockEid).Position = 
			chunk[y, x] = blockEID;
		}

		public bool GetEntityAt(int x, int y, out Guid eid) {
			if(_chunks.TryGetValue(new Point(y / CHUNK_SIZE, x / CHUNK_SIZE), out Chunk chunk) && (eid = chunk.Tiles[y % CHUNK_SIZE, x % CHUNK_SIZE]) != Guid.Empty) {
				return true;
			}
			eid = Guid.Empty;
			return false;
		}
		public bool GetEntityAt(Point p, out Guid eid) => GetEntityAt(p.X ,p.Y, out eid);

		public bool IsCellFilled(int x, int y) {
			if(_chunks.TryGetValue(new Point(y / CHUNK_SIZE, x / CHUNK_SIZE), out Chunk chunk)) {
				return chunk.Tiles[y % CHUNK_SIZE, x % CHUNK_SIZE] != Guid.Empty;
			}
			return false;
		}
		public bool IsCellFilled(Point p) => IsCellFilled(p.X, p.Y);

		public static Point ToTilePosition(Vector2 vector)
			=> new Point((int)vector.X / TILE_SIZE, (int)vector.Y / TILE_SIZE);

		public static Point GetChunk(Point gridPosition)
			=> new Point(gridPosition.X>>CHUNK_SIZE_FACTOR, gridPosition.Y>>CHUNK_SIZE_FACTOR);

		public static Point GetChunk(Vector2 realPosition)
			=> new Point(((int)realPosition.X/TILE_SIZE) >> CHUNK_SIZE_FACTOR, ((int)realPosition.Y/TILE_SIZE)>> CHUNK_SIZE_FACTOR);

		public static Vector2 NearestTilePosition(Vector2 vector)
			=> new Vector2(((int)vector.X/TILE_SIZE)* TILE_SIZE, ((int)vector.Y/TILE_SIZE)*TILE_SIZE);

		private class Chunk {
			public Chunk(Guid[,] tiles, bool isActive) {
				Tiles = tiles;
				IsActive = isActive;
			}
			public Guid[,] Tiles;
			public bool IsActive;
		}
	}
}
