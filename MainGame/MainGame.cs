using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using MainGame.Serialization;
using System.Collections.Generic;
using System;
using System.IO;
using MainGame.Systems;
using UI = MainGame.Systems.UI;
using tainicom.Aether.Physics2D.Dynamics;
using PhysicsWorld = tainicom.Aether.Physics2D.Dynamics.World;
using InputSystem;
using Serialization;
using Serialization.Physics;
using Serialization.Content;
using Serialization.Assets;
using Assets;
using System.Reflection;
using ECS;

using MainGame.Serialization.MoonSharp;
using MainGame.Serialization.Components;


namespace MainGame {
	public class MainGame : Game {
		private readonly JsonSerializerOptions _entitySerializerOptions;
		private readonly JsonSerializerOptions _entityGroupSerializerOptions;
		public const string GAME_TITLE = "Enter the Megadungeon";
		public const string CONTROLLS_CONFIG_PATH = @"Assets\Controlls.json";
		public Point Resolution = new Point(256, 144);
		private readonly ECS.World _world;
		private readonly PhysicsWorld _physicsWorld;
		private readonly Dictionary<Guid, Asset> _assets = new Dictionary<Guid, Asset>();
		public GraphicsDeviceManager Graphics;
		public readonly Camera MainCamera = new Camera() { Position = Vector2.Zero, Resolution = new Point(256, 144) };

		public const int SCALE = 16;

		public float aspectRatio = 16f / 9f;

		private SpriteBatch _targetBatch;
		public SpriteBatch SpriteBatch;

		private RenderTarget2D _target;

		public MainGame() : base() {
			Window.Title = GAME_TITLE;
			//Window.AllowUserResizing = true;
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Graphics = new GraphicsDeviceManager(this);
			_physicsWorld = new tainicom.Aether.Physics2D.Dynamics.World();
			InputManager.LoadBindings(CONTROLLS_CONFIG_PATH);
			_entitySerializerOptions = new JsonSerializerOptions() {
				Converters = {
					
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
					new SoundEffectConverter(Content),

					// assets
					new AssetConverter(_assets),
					new TileSheetConverter()
				}
			};
			_entityGroupSerializerOptions = new JsonSerializerOptions(_entitySerializerOptions);
			_world = new ECS.World(_entitySerializerOptions, _entityGroupSerializerOptions);
			_entitySerializerOptions.Converters.Add(new ScriptConverter(this, _physicsWorld, _world));
			_entitySerializerOptions.Converters.Add(new BodyConverter());
			_entityGroupSerializerOptions.Converters.Add(new BodyConverter());
			_entityGroupSerializerOptions.Converters.Add(new ScriptConverter(this, _physicsWorld, _world));
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
			_world.AddSystem(new Systems.PhysicsSystem(_world, _physicsWorld));
			_world.AddSystem(new UI.ButtonClickSystem(_world, this, _physicsWorld));

			_world.AddSystem(new PlayerController(_world, Content, this, _physicsWorld));
			_world.AddSystem(new TileSystem(_world, this, _physicsWorld));
			_world.AddSystem(new Following(_world));
			_world.AddSystem(new CameraFollow(_world, this));
			_world.AddSystem(new Animator(_world));
			_world.AddSystem(new MoverSystem(_world));

			_world.AddSystem(new UI.SpriteDraw(_world, this));
			_world.AddSystem(new UI.VolumeDraw(_world, this));
			_world.AddSystem(new SpriteDraw(_world, this));

			// debug
			//_world.RegisterSystem(new CollisionDraw(_world, this), 0);
			#if DEBUG
			_world.AddSystem(new PositionDraw(_world, this));
			#endif

			LoadAssets(@"Assets\Assets.json");
			_world.LoadEntityGroupFromFile("Assets/TestScene.json");
			//_world.LoadEntityGroupFromFile("Assets/Scenes/MainMenuScene.json");
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

			float maxtargetresolution = 256f;
			aspectRatio = (float)w / h;
			float targetResolutionX;
			float targetResolutionY;

			if(w > h) {
				scaleValue = w / 255f;
				targetResolutionX = maxtargetresolution;
				targetResolutionY = targetResolutionX / aspectRatio;
			} else {
				scaleValue = h / 255f;
				targetResolutionY = maxtargetresolution;
				targetResolutionX = targetResolutionY * aspectRatio;
			}

			Matrix m = Matrix.CreateScale(scaleValue, scaleValue, 1f); // * Matrix.CreateTranslation(new Vector3(viewx, viewy, 0f));

			// Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up) * Matrix.CreateOrthographicOffCenter(0f, 1280f, 720f, 0, 0, -100) *  * Matrix.CreateTranslation(0f, 0f, 0);
			//GraphicsDevice.SetRenderTarget(_target);
			GraphicsDevice.Clear(new Color(0x2d, 0x9c, 0x42));
			SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack, transformMatrix: m);
			//SpriteBatch.Draw(Content.Load<Texture2D>("Textures/pixel"), new Rectangle(0, 0, (int)targetResolutionX, (int)targetResolutionY), Color.White);
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
			Type assetType = Type.GetType(Assembly.CreateQualifiedName("Assets", jsonElement.GetProperty("Type").GetString()));
			
			if(!(JsonSerializer.Deserialize(data.GetRawText(), assetType, _entitySerializerOptions) is Asset asset))
				throw new Exception("Asset type is not Asset");
			_assets.Add(assetID, asset);
		}

		public Vector2 ScreenToWorld(Vector2 ScreenPos) {
			return ScreenPos/5f;
		}

		public float PixelScale => 5f;

		public Vector2 WorldToScreen(Vector2 worldPosition) {
			return Vector2.Zero;
		}



	}
}
