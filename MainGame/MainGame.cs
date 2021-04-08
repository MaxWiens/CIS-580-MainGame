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
		public const string GAME_TITLE = "Frogs of Destruction";
		public const string CONTROLLS_CONFIG_PATH = @"Assets\Controlls.json";
		private readonly ECS.World _world;
		private readonly PhysicsWorld _physicsWorld;
		private readonly Dictionary<Guid, Asset> _assets = new Dictionary<Guid, Asset>();
		public GraphicsDeviceManager Graphics;
		public readonly Camera MainCamera = new Camera() { Position = Vector2.Zero, Resolution = new Point(_minTargetDimention, _minTargetDimention) };

		public SpriteBatch SpriteBatch;
		public SpriteBatch UISpriteBatch;

		private RenderTarget2D _target;

		public MainGame() : base() {
			
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
			
			Window.AllowUserResizing = true;
			Window.ClientSizeChanged += OnWindowSizeChange;
		}

		protected override void Initialize() {
			base.Initialize();
			Window.Title = GAME_TITLE;

			Graphics.PreferredBackBufferWidth = 1280;
			Graphics.PreferredBackBufferHeight = 720;
			Graphics.ApplyChanges();
			OnWindowSizeChange(null, null);
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			UISpriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void BeginRun() {
			PhysicsSystem physicsSystem = new PhysicsSystem(_world, _physicsWorld);
			_world.AddSystem(physicsSystem);
			_world.AddSystem(new UI.ButtonClickSystem(_world, this, _physicsWorld));

			_world.AddSystem(new PlayerController(_world, Content, this, physicsSystem));
			_world.AddSystem(new TileSystem(_world, this, _physicsWorld));
			_world.AddSystem(new Following(_world));
			_world.AddSystem(new CameraFollow(_world, this));
			_world.AddSystem(new Animator(_world));
			_world.AddSystem(new MoverSystem(_world));
			_world.AddSystem(new Systems.AI.EnemyAISystem(_world));

			_world.AddSystem(new UI.SpriteDraw(_world, this));
			_world.AddSystem(new UI.VolumeDraw(_world, this));
			_world.AddSystem(new SpriteDraw(_world, this));
			_world.AddSystem(new LifetimeSystem(_world));

			_world.AddSystem(new ParticleSystem(_world, this));

			_world.AddSystem(new UI.ElementRepositionSystem(_world, this));
			

			// debug Systems
#if DEBUG
			//_world.RegisterSystem(new CollisionDraw(_world, this), 0);
			//_world.AddSystem(new PositionDraw(_world, this));
#endif

			LoadAssets(@"Assets\Assets.json");
			_world.LoadEntityGroupFromFile("Assets/Scenes/MainMenuScene.json");
			//_world.LoadEntityGroupFromFile("Assets/Scenes/MainMenuScene.json");
		}

		protected override void Update(GameTime gameTime) {	
			InputManager.Update();
			_world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
		}
		
		protected override void Draw(GameTime gameTime) {
			base.Draw(gameTime);

			Matrix m = Matrix.CreateTranslation((MainCamera.Resolution.X*0.5f) - MainCamera.Position.X, (MainCamera.Resolution.Y * 0.5f) - MainCamera.Position.Y, 0f);
			Matrix scaleMatrix = Matrix.CreateScale(_scale, _scale, 1f);
			GraphicsDevice.Clear(new Color(0x2d, 0x9c, 0x42));
			SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack, transformMatrix: m * scaleMatrix);
			UISpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: scaleMatrix);
			_world.Draw();
			SpriteBatch.End();
			UISpriteBatch.End();
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
			return ScreenPos/_scale;
		}

		public Vector2 WorldToScreen(Vector2 worldPosition) {
			return worldPosition*_scale; // not correct?
		}
		
		private float _scale;
		private const int _minTargetDimention = 256;
		
		private void OnWindowSizeChange(object sender, EventArgs e) {
			// on resize, change the the target resolution so the smallest

			int windowWidth = Window.ClientBounds.Width;
			int windowHeight = Window.ClientBounds.Height;

			if(windowWidth < _minTargetDimention) {
				Graphics.PreferredBackBufferWidth = _minTargetDimention;
				windowWidth = _minTargetDimention;
			}
			if(windowHeight < _minTargetDimention) {
				Graphics.PreferredBackBufferHeight = _minTargetDimention;
				windowHeight = _minTargetDimention;
			}
			Graphics.ApplyChanges();

			if(windowWidth < windowHeight) {
				// w is smallest dimention
				MainCamera.Resolution.X = _minTargetDimention;
				MainCamera.Resolution.Y = (int)(_minTargetDimention * ((float)windowHeight / windowWidth));
				_scale = (float)windowWidth / _minTargetDimention;
			} else {
				// h is smallest dimention
				MainCamera.Resolution.X = (int)(_minTargetDimention * ((float)windowWidth / windowHeight));
				MainCamera.Resolution.Y = _minTargetDimention;
				_scale = (float)windowHeight / _minTargetDimention;
			}
			WindowResized?.Invoke(MainCamera.Resolution);
		}

		public event Action<Point> WindowResized;
	}
}
