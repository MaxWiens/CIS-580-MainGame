using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
namespace MainGame.Systems {
	using ECS;
	using Components;
	[MoonSharpUserData]
	public class TileSystem : BaseSystem, IFixedUpdateable {
		public const int CHUNK_SIZE_FACTOR = 4;
		public const int CHUNK_SIZE = 1<<CHUNK_SIZE_FACTOR;
		public const int TILE_SIZE = 16;
		private readonly Dictionary<Guid, (int,int,int,int)> _tiles = new Dictionary<Guid, (int, int, int, int)>();
		private Dictionary<Point, Chunk> _chunks = new Dictionary<Point, Chunk>();
		Random r;
		private readonly MainGame _game;
		private readonly tainicom.Aether.Physics2D.Dynamics.World _physicsWorld;

		public TileSystem(World world, MainGame game, tainicom.Aether.Physics2D.Dynamics.World physicsWorld) : base(world) {
			r = new Random((int)DateTime.Now.Ticks);
			_game = game;
			_physicsWorld = physicsWorld;
			World.Reset += OnReset;
		}

		public void FixedUpdate(float deltaTime) {
			//var gridElementMap = world.GetEntitiesWithComponent<GridElement>();
			var chunkLoaderMap = World.GetEntitiesWith<ChunkLoading>();
			var bodyMap = World.GetEntitiesWith<Body>();
			Body body;
			Point chunkPosition;

			foreach(KeyValuePair<Point, Chunk> posChunk in _chunks) {
				posChunk.Value.IsActive = false;
			}

			foreach(Entity entity in chunkLoaderMap.Keys) {
				body = entity.GetComponent<Body>();
				Point p = ToTilePosition(body.Position);
				chunkPosition = GetChunk(p);
				//global::System.Diagnostics.Debug.WriteLine($"tile:{p} chunk:{chunkPosition} real:{body.Position}");
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
		}

		public void LoadChunk(Point chunkPosition) {
			if(!_chunks.TryGetValue(chunkPosition, out Chunk chunk)) {
				// load chunk
				Dictionary<Point, Guid> tiles = new Dictionary<Point, Guid>();
				_chunks.Add(chunkPosition, new Chunk(tiles, true));
				int topSize = r.Next(0, 6);
				int bottomSize = r.Next(CHUNK_SIZE-6, CHUNK_SIZE+1);

				int leftSize =  r.Next(0, 6);
				int rightSize = r.Next(CHUNK_SIZE-6, CHUNK_SIZE+1);
				
				if(chunkPosition != Point.Zero) {
					
					
					Entity e;
					
					if(Util.Rand.Float() >= 0.5f) {
						e = World.CloneEntityGroup("Assets\\Prefabs\\Entities\\Ghost.json")[0];
					} else {
						e = World.CloneEntityGroup("Assets\\Prefabs\\Entities\\Skeleton.json")[0];
					}
					e.GetComponent<Body>().Position = chunkPosition.ToVector2() * (16f * 16f) + new Vector2(128, 128);

					if(Util.Rand.Float() <= 0.1f) {
						if(Util.Rand.Float() >= 0.5f) {
							e = World.CloneEntityGroup("Assets\\Prefabs\\Entities\\Ghost.json")[0];
						} else {
							e = World.CloneEntityGroup("Assets\\Prefabs\\Entities\\Skeleton.json")[0];
						}
						e.GetComponent<Body>().Position = chunkPosition.ToVector2() * (16f * 16f) + new Vector2(128, 128);
					}
				}
					

				

				for(int y =0; y < topSize; y++) {
					for(int x = 0; x < CHUNK_SIZE; x++) {
						if(x == 7 || x == 8) continue;
						CreateBrick(x, y, chunkPosition.X, chunkPosition.Y, tiles);
					}
				}

				for(int y = bottomSize; y < CHUNK_SIZE; y++) {
					for(int x = 0; x < CHUNK_SIZE; x++) {
						if(x == 7 || x == 8) continue;
						CreateBrick(x, y, chunkPosition.X, chunkPosition.Y, tiles);
					}
				}

				for(int y = topSize; y < bottomSize; y++) {
					if(y == 7 || y == 8) continue;
					for(int x = 0; x < leftSize; x++) {
						CreateBrick(x, y, chunkPosition.X, chunkPosition.Y, tiles);
					}
				}

				for(int y = topSize; y < bottomSize; y++) {
					if(y == 7 || y == 8) continue;
					for(int x = rightSize; x < CHUNK_SIZE; x++) {
						CreateBrick(x, y, chunkPosition.X, chunkPosition.Y, tiles);
					}
				}
				
			} else {
				chunk.IsActive = true;
			}
		}

		private void UnloadChunk(Point p) {
			Chunk chunk = _chunks[p];
			foreach(KeyValuePair<Point, Guid> kvp in chunk.Tiles) {
				World.RemoveEntity(kvp.Value);
			}
			_chunks.Remove(p);
		}

		private void CreateBrick(int x, int y, int chunkX, int chunkY, Dictionary<Point,Guid> chunk) {
			Entity block = World.CloneEntityGroup("Assets/Prefabs/Wall.json")[0];
			Body b = block.GetComponent<Body>();
			b.Position = new Vector2(
				((chunkX * CHUNK_SIZE) + x) * TILE_SIZE,
				((chunkY * CHUNK_SIZE) + y) * TILE_SIZE
			);
			//_tiles.Add(block.EID, (x,y,chunkX,chunkY));
			block.Enable();
			//chunk.Add(new Point(x,y), block.EID);
		}

		public bool RemoveTile(Point tilePosition) {
			Point chunkPos = GetChunk(tilePosition);
			if(_chunks.TryGetValue(chunkPos, out Chunk chunk)) {
				chunk.Tiles.Remove(GlobalTilePositionToChunkTilePosition(tilePosition), out Guid tileEID);
				_tiles.Remove(tileEID);
				return true;
			} else {
				// load chunk to delete entity
				//throw new NotImplementedException();
			}
			return false;
		}

		private bool RemoveTile(int tileX, int tileY, int chunkX, int chunkY) {
			if(_chunks.TryGetValue(new Point(chunkX,chunkY), out Chunk chunk)) {
				chunk.Tiles.Remove(new Point(tileX, tileY), out Guid tileEID);
				_tiles.Remove(tileEID);
				return true;
			} else {
				// load chunk to delete entity
				throw new NotImplementedException();
			}
			return false;
		}

		public bool RemoveTile(Guid tileEID)
			=> _tiles.TryGetValue(tileEID, out (int, int, int, int) position) && RemoveTile(position.Item1, position.Item2, position.Item3, position.Item4);

		public bool AddTile(Guid eid, Point globalTilePosition) {
			Point chunkPos = GetChunk(globalTilePosition);
			if(_chunks.TryGetValue(chunkPos, out Chunk chunk)) {
				Point tilePosition = GlobalTilePositionToChunkTilePosition(globalTilePosition);
				if(chunk.Tiles.TryAdd(tilePosition, eid)) {
					_tiles.Add(eid, (tilePosition.X, tilePosition.Y, chunkPos.X, chunkPos.Y));
				}
				return false;
			} else {
				// load chunck at position
				
				throw new NotImplementedException();
			}
		}

		public bool GetEntityAt(Point p, out Guid eid) {
			if(_chunks.TryGetValue(GetChunk(p), out Chunk chunk) && chunk.Tiles.TryGetValue(GlobalTilePositionToChunkTilePosition(p), out eid))
				return true;
			eid = Guid.Empty;
			return false;
		}
		public bool GetEntityAt(int x, int y, out Guid eid) => GetEntityAt(new Point(x,y), out eid);

		public bool IsCellFilled(Point tilePosition) {
			if(_chunks.TryGetValue(GetChunk(tilePosition), out Chunk chunk)) {
				return chunk.Tiles.ContainsKey(GlobalTilePositionToChunkTilePosition(tilePosition));
			}
			return false;
		}
		public bool IsCellFilled(int x, int y) => IsCellFilled(new Point(x,y));

		public static Point ToTilePosition(Vector2 vector)
			=> new Point(Util.Math.IntDivRoundDown((int)vector.X, TILE_SIZE), Util.Math.IntDivRoundDown((int)vector.Y, TILE_SIZE));

		public static Point GetChunk(Point gridPosition)
			=> new Point(Util.Math.IntDivRoundDown(gridPosition.X, CHUNK_SIZE), Util.Math.IntDivRoundDown(gridPosition.Y, CHUNK_SIZE));

		public static Point GetChunk(Vector2 realPosition) => GetChunk(ToTilePosition(realPosition));

		public static Vector2 NearestTilePosition(Vector2 vector) => ToTilePosition(vector).ToVector2();

		public Point GlobalTilePositionToChunkTilePosition(Point globalTilePosition)
			=> new Point(Util.Math.Mod(globalTilePosition.X, CHUNK_SIZE), Util.Math.Mod(globalTilePosition.Y, CHUNK_SIZE));

		private class Chunk {
			public Chunk(Dictionary<Point,Guid> tiles, bool isActive) {
				Tiles = tiles;
				IsActive = isActive;
			}
			public Dictionary<Point,Guid> Tiles;
			public bool IsActive;
		}
	}
}
