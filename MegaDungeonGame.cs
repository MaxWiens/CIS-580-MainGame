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
using UI = MainGame.Systems.UI;
using MainGame.Assets;
namespace MainGame {
	public class MegaDungeonGame : Game {
		private readonly JsonSerializerOptions _serializerOptions;
		public const string GAME_TITLE = "Enter the Megadungeon";
		public const string CONTROLLS_CONFIG_PATH = @"Assets\Controlls.json";
		public Point Resolution = new Point(256, 144);
		private readonly ECS.GameWorld _world;
		private readonly tainicom.Aether.Physics2D.Dynamics.World _physicsWorld;
		private readonly Dictionary<Guid, Assets.Asset> _assets = new Dictionary<Guid, Assets.Asset>();
		public GraphicsDeviceManager Graphics;
		public Guid MainCamera;

		public const int SCALE = 16;

		public const float aspectRatio = 16f/9f;

		private SpriteBatch _targetBatch;
		public SpriteBatch SpriteBatch;

		private RenderTarget2D _target;

		public MegaDungeonGame() : base() {
			Window.Title = GAME_TITLE;
			Window.AllowUserResizing = true;
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Graphics = new GraphicsDeviceManager(this);
			_physicsWorld = new tainicom.Aether.Physics2D.Dynamics.World();

			InputManager.LoadBindings(CONTROLLS_CONFIG_PATH);
			_serializerOptions = new JsonSerializerOptions() {
				Converters = {
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
			_world = new ECS.GameWorld(_serializerOptions, _physicsWorld);
			_serializerOptions.Converters.Add(new BodyConverter(_physicsWorld, _world));
			_serializerOptions.Converters.Add(new ScriptConverter(this, _physicsWorld, _world));
		}

		protected override void Initialize() {
			base.Initialize();

			Graphics.PreferredBackBufferWidth = 1280;
			Graphics.PreferredBackBufferHeight = 720;
			Graphics.ApplyChanges();

			_targetBatch = new SpriteBatch(GraphicsDevice);
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			_target = new RenderTarget2D(GraphicsDevice, Resolution.X, Resolution.Y);
		}

		protected override void BeginRun() {
			_world.RegisterSystem(new Systems.Physics(_world, _physicsWorld), 0);
			_world.RegisterSystem(new ButtonClickSystem(_world, _physicsWorld, this));
			
			_world.RegisterSystem(new PlayerController(_world, Content));
			_world.RegisterSystem(new Grid(_world, this, _physicsWorld));
			_world.RegisterSystem(new Following(_world));
			_world.RegisterSystem(new Destruction(_world));
			_world.RegisterSystem(new Animator(_world));
			_world.RegisterSystem(new MoverSystem(_world));

			_world.RegisterSystem(new UI.SpriteDraw(_world, this));
			_world.RegisterSystem(new UI.VolumeDraw(_world, this));
			_world.RegisterSystem(new SpriteDraw(_world, this));

			// debug
			//_world.RegisterSystem(new CollisionDraw(_world, this), 0);
			_world.RegisterSystem(new PositionDraw(_world, this), 0);

			LoadAssets(@"Assets\Assets.json");
			_world.LoadEntityGroupFromFile("Assets/Scenes/MainMenuScene.json", Guid.Empty);

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
			int w = Graphics.PreferredBackBufferWidth;
			int h = Graphics.PreferredBackBufferHeight;
			float scaleValue;
			float viewx;
			float viewy;
			
			if((float)w/h > aspectRatio) { //w > h) {// && 
				//boarders on Sides
				scaleValue = h / 144f;
				System.Diagnostics.Debug.WriteLine($"A {w},{h}, {scaleValue}, {(float)w/h}");
				viewx = (w-(256f*scaleValue))/2f;
				viewy = 0f;
			} else {
				//boarders on 
				scaleValue = w / 256f;
				System.Diagnostics.Debug.WriteLine($"B {w},{h}, {scaleValue}, {(float)w / h}");
				viewx = 0f;
				viewy = (h - (144f * scaleValue)) / 2f;
			}
			Matrix m = Matrix.CreateScale(scaleValue, scaleValue, 1f) * Matrix.CreateTranslation(new Vector3(viewx, viewy, 0f));
			
			// Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up) * Matrix.CreateOrthographicOffCenter(0f, 1280f, 720f, 0, 0, -100) *  * Matrix.CreateTranslation(0f, 0f, 0);
			//GraphicsDevice.SetRenderTarget(_target);
			GraphicsDevice.Clear(new Color(0x2d, 0x9c, 0x42));
			SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack, transformMatrix: m);
			//SpriteBatch.Draw(Content.Load<Texture2D>("Textures/pixel"), new Rectangle(0, 0, 256, 144), Color.White);
			_world.Draw();
			SpriteBatch.End();
			

			//GraphicsDevice.SetRenderTarget(null);
			
			//_targetBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);
			//_targetBatch.Draw(_target, new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), Color.White);
			//_targetBatch.End();
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
