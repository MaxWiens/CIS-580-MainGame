using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using MainGame.Serialization;
using System.Collections.Generic;
using System;
using System.IO;
using MainGame.Input;
using MainGame.Systems;
using MainGame.Assets;
namespace MainGame {
	public class MegaDungeonGame : Game {
		private readonly JsonSerializerOptions _serializerOptions;


		public const string GAME_TITLE = "Enter the Megadungeon";
		public const string CONTROLLS_CONFIG_PATH = @"Assets\Controlls.json";
		public Point Resolution = new Point(256, 144);
		private readonly ECS.World _world;
		private readonly tainicom.Aether.Physics2D.Dynamics.World _physicsWorld;
		private readonly Dictionary<Guid, Assets.Asset> _assets = new Dictionary<Guid, Assets.Asset>();
		private GraphicsDeviceManager _graphics;
		public Guid MainCamera;

		private SpriteBatch _targetBatch;
		public SpriteBatch SpriteBatch;

		private RenderTarget2D _target;

		public MegaDungeonGame() : base() {
			Window.Title = GAME_TITLE;
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			_graphics = new GraphicsDeviceManager(this);
			_physicsWorld = new tainicom.Aether.Physics2D.Dynamics.World();

			InputManager.LoadBindings(CONTROLLS_CONFIG_PATH);
			_serializerOptions = new JsonSerializerOptions() {
				Converters = {
					new BodyConverter(_physicsWorld),
					new LayerConverter(),
					
					// Monogame type converters
					new PointConverter(),
					new RectangleConverter(),
					new Vector2Converter(),
					new Vector3Converter(),
					new ColorConverter(),
					
					// Aether type converters
					new ShapeConverter(),
							
					// content
					new Texture2DConverter(Content),

					// assets
					new AssetConverter(_assets),
					new TileSheetConverter()
				}
			};
			_world = new ECS.World(_serializerOptions);
		}

		protected override void Initialize() {
			base.Initialize();

			_graphics.PreferredBackBufferWidth = 1280;
			_graphics.PreferredBackBufferHeight = 720;
			_graphics.ApplyChanges();

			_targetBatch = new SpriteBatch(GraphicsDevice);
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			_target = new RenderTarget2D(GraphicsDevice, Resolution.X, Resolution.Y);
		}

		protected override void BeginRun() {
			_world.RegisterSystem(new Systems.Physics(_world, _physicsWorld), 0);
			_world.RegisterSystem(new Systems.Physics(_world, _physicsWorld), 1);

			_world.RegisterSystem(new PlayerController(_world, Content));
			_world.RegisterSystem(new SpriteDraw(_world, this));
			_world.RegisterSystem(new Grid(_world));
			_world.RegisterSystem(new Following(_world));
			_world.RegisterSystem(new Destruction(_world));
			_world.RegisterSystem(new Animator(_world));
			_world.RegisterSystem(new MoverSystem(_world));

			// debug
			_world.RegisterSystem(new CollisionDraw(_world, this), 0);
			_world.RegisterSystem(new PositionDraw(_world, this), 0);


			LoadAssets(@"Assets\Assets.json");
			_world.LoadEntityGroupFromFile(@"Assets\TestScene.json", Guid.Empty);

			var eids = _world.GetEntitiesWithComponent<Components.Camera>();
			if(eids.Count == 0) {
				System.Diagnostics.Debug.Fail("no camera in startin scene");
				Exit();
				return;
			} else {
				var enumerator = eids.Keys.GetEnumerator();
				enumerator.MoveNext();
				MainCamera = enumerator.Current;
			}
		}

		protected override void Update(GameTime gameTime) {
			InputManager.Update();
			_world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
		}

		protected override void Draw(GameTime gameTime) {
			base.Draw(gameTime);
			GraphicsDevice.SetRenderTarget(_target);
			GraphicsDevice.Clear(new Color(0x2d, 0x9c, 0x42));
			SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);
			_world.Draw();
			SpriteBatch.End();

			GraphicsDevice.SetRenderTarget(null);
			_targetBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);
			_targetBatch.Draw(_target, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
			_targetBatch.End();
		}

		public void LoadAssets(string filePath) {
			using FileStream stream = File.OpenRead(filePath);
			using JsonDocument doc = JsonDocument.Parse(stream);
			foreach(JsonElement entityElement in doc.RootElement.GetProperty("Assets").EnumerateArray()) {
				LoadAsset(entityElement);
			}
		}
		public void LoadAsset(JsonElement jsonElement) {
			JsonElement data = jsonElement.GetProperty("Data");
			Guid assetID = data.GetProperty("ID").GetGuid();
			if(_assets.ContainsKey(assetID))
				return;
			Type assetType = Type.GetType(jsonElement.GetProperty("Type").GetString());
			if(!(JsonSerializer.Deserialize(data.GetRawText(), assetType, _serializerOptions) is Asset asset))
				throw new Exception("Asset type is not Asset");
			_assets.Add(assetID, asset);
		}
	}
}
