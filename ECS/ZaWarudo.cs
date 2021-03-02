using System;
using System.Collections.Generic;
using System.Text;
using MainGame.Util;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace MainGame.ECS {
	using Systems;
	using Input;
	using Collections;
	using Assets;
	public class ZaWarudo : Game {
		public SpriteBatch SpriteBatch;
		private SpriteBatch _targetBatch;
		public RenderTarget2D Target;
		public Point Resolution = new Point(256, 144);
		private readonly Dictionary<Type, IKeyRefMap<Guid>> _componentStore;
		private readonly Dictionary<Guid, HashSet<Type>> _entities;
		private readonly Dictionary<Guid, Asset> _assets;
		private readonly Dictionary<Type, UpdateSystem> _updateSystems;

		private readonly Dictionary<Type, UpdateSystem> _enabledUpdateSystems;
		private readonly Dictionary<Type, DrawSystem> _drawSystems;
		private readonly Dictionary<Type, DrawSystem> _enabledDrawSystems;
		private readonly IEnumerable<System> _systems;
		private readonly Dictionary<Type, System> _systemDictionary;
		private const BindingFlags MESSAGE_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance;

		private GraphicsDeviceManager _graphics;

		public readonly InputManager InputManager;

		public Guid MainCamera;

		public readonly ComponentParser ComponentParser;
		private readonly JsonSerializerOptions _jsonSerializerOptions;
		public ZaWarudo() {

			_entities = new Dictionary<Guid, HashSet<Type>>();
			_componentStore = new Dictionary<Type, IKeyRefMap<Guid>>();

			_updateSystems = new Dictionary<Type, UpdateSystem>();
			_enabledUpdateSystems = new Dictionary<Type, UpdateSystem>();
			_drawSystems = new Dictionary<Type, DrawSystem>();
			_enabledDrawSystems = new Dictionary<Type, DrawSystem>();

			_systemDictionary = new Dictionary<Type, System>();
			_assets = new Dictionary<Guid, Asset>();

			_jsonSerializerOptions = new JsonSerializerOptions() {
				IgnoreNullValues = false,
				Converters = {
						new Serialization.PointConverter(),
						new Serialization.RectangleConverter(),
						new Serialization.Vector2Converter(),
						new Serialization.Vector3Converter(),
						new Serialization.ColorConverter(),
						new Serialization.SoloBindingConverter(),
						new Serialization.CompositeBindingConverter(),
						new Serialization.Texture2DConverter(Content),
						new Serialization.LayerConverter(),
						
						// Assets
						new Serialization.AssetConverter(_assets),
						new Serialization.TileSheetConverter(),
					}
			};
			_graphics = new GraphicsDeviceManager(this);
			ComponentParser = new ComponentParser(_jsonSerializerOptions);
			InputManager = new InputManager(_jsonSerializerOptions);
			
			
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.Title = "Block Frog";

			_systems = GetSystems();
			foreach(System system in _systems)
				RegisterSystem(system);
		}

		private System[] GetSystems() => new System[]{
			new PlayerController(this),
			new SpriteDraw(this),
			new Physics(this),
			new Grid(this),
			new Following(this),
			new Destruction(this),
			new Animator(this),
			new MoverSystem(this),

			// debug
			new CollisionDraw(this),
			//new PositionDraw(this),
		};

		public T GetSystem<T>() where T : System
			=> _systemDictionary.TryGetValue(typeof(T), out System s) && s is T t ? t : null;

		protected override void Initialize() {
			_graphics.PreferredBackBufferWidth = 1280;
			_graphics.PreferredBackBufferHeight = 720;
			_graphics.ApplyChanges();

			LoadAssets(@"Assets\Assets.json");

			LoadEntities(@"Assets\TestScene.json");

			var eids = GetEntitiesWithComponent<Components.Camera>();
			if(eids.Count == 0) {
				global::System.Diagnostics.Debug.Fail("no camera in startin scene");
				Exit();
				return;
			} else {
				var enumerator = eids.Keys.GetEnumerator();
				enumerator.MoveNext();
				MainCamera = enumerator.Current;
			}


			foreach(System system in _systems) {
				system.GetType().GetMethod("OnInit", MESSAGE_FLAGS)?.Invoke(system, null);
			}

			_targetBatch = new SpriteBatch(GraphicsDevice);
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			Target = new RenderTarget2D(GraphicsDevice, Resolution.X, Resolution.Y);
			base.Initialize();
		}

		protected override void LoadContent() {
			foreach(System system in _systems) {
				system.GetType().GetMethod("OnContentLoad", MESSAGE_FLAGS)?.Invoke(system, null);
			}
		}

		public Guid MakeEntity() => MakeEntity(Guid.NewGuid());

		public Guid MakeEntity(params object[] components) => MakeEntity(Guid.NewGuid(), components);

		public Guid MakeEntity(Guid eid) {
			_entities.Add(eid, new HashSet<Type>());
			return eid;
		}

		public Guid MakeEntity(IEnumerable<object> components) => MakeEntity(Guid.NewGuid(), components);

		public Guid MakeEntity(Guid eid, IEnumerable<object> components) {
			if(!_entities.TryAdd(eid, new HashSet<Type>()))
				return MakeEntity(Guid.NewGuid(), components);
			foreach(object c in components) {
				AddComponent(eid, c);
			}
			return eid;
		}
		
		public void DestroyEntity(Guid entityID) {
		if(_entities.Remove(entityID, out HashSet<Type> componentTypes)) {
				foreach(Type t in componentTypes) {
					_componentStore[t].Remove(entityID);
				}
			}
		}

		public RefMap<Guid,T> GetEntitiesWithComponent<T>() where T : struct {
			if(_componentStore.TryGetValue(typeof(T), out var entities)) {
				return entities as RefMap<Guid, T>;
			}
			var map = new RefMap<Guid, T>();
			_componentStore.Add(typeof(T), map);
			return map;
		}

		public void AddComponent(Guid entityID, object component) {
			Type componentType = component.GetType();
			if(_entities.TryGetValue(entityID, out var components)) {
				components.Add(componentType);
			} else {
				// entity does'nt exist
				return;
			}
			if(_componentStore.TryGetValue(componentType, out IKeyRefMap<Guid> entities)) {
				entities.Add(entityID, component);
			} else {
				Type bagtype = typeof(RefMap<,>).MakeGenericType(typeof(Guid), componentType);
				IKeyRefMap<Guid> bag = Activator.CreateInstance(bagtype) as IKeyRefMap<Guid>;
				bag.Add(entityID, component);
				_componentStore.Add(componentType, bag);
			}
		}

		public void AddComponent<T>(Guid entityID, T component) where T : struct{
			Type componentType = typeof(T);
			if(_entities.TryGetValue(entityID, out var components)) {
				components.Add(componentType);
			} else {
				// entity does'nt exist
				return;
			}

			if(_componentStore.TryGetValue(componentType, out var entities)) {
				entities.Add(entityID, component);
			} else {
				entities = new RefMap<Guid, T>();
				entities.Add(entityID, component);
				_componentStore.Add(componentType, entities);
			}
		}

		public void RemoveComponent<T>(Guid entityID) {
			if(_entities.TryGetValue(entityID, out var componentTypes)) {
				componentTypes.Remove(typeof(T));
				_componentStore[typeof(T)].Remove(entityID);
			}
		}

		public ref T GetComponent<T>(Guid entityID) where T : struct {
			return ref (_componentStore[typeof(T)] as RefMap<Guid, T>)[entityID];
		}

		public ref T TryGetComponent<T>(Guid entityID, ref T fallbackValue, out bool isSuccessful) where T : struct {
			if(_componentStore.TryGetValue(typeof(T), out var bag)) {
				return ref (bag as RefMap<Guid, T>).TryGetValue(entityID, ref fallbackValue, out isSuccessful);
			}
			isSuccessful = false;
			return ref fallbackValue;
		}

		public void EnableSystem<T>() where T : System {
			if(typeof(T).IsAssignableFrom(typeof(UpdateSystem)) && _updateSystems.TryGetValue(typeof(T), out UpdateSystem updateSystem)) {
				_enabledUpdateSystems.Add(typeof(T), updateSystem);
				typeof(T).GetMethod("OnEnable", MESSAGE_FLAGS)?.Invoke(updateSystem, null);
			} else if(typeof(T).IsAssignableFrom(typeof(DrawSystem)) && _drawSystems.TryGetValue(typeof(T), out DrawSystem drawSystem)) {
				_enabledDrawSystems.Add(typeof(T), drawSystem);
				typeof(T).GetMethod("OnEnable", MESSAGE_FLAGS)?.Invoke(drawSystem, null);
			}
		}

		public void DisableSystem<T>() where T : System {
			if(typeof(T).IsAssignableFrom(typeof(UpdateSystem)) && _enabledUpdateSystems.TryGetValue(typeof(T), out UpdateSystem updateSystem)) {
				_enabledUpdateSystems.Add(typeof(T), updateSystem);
				typeof(T).GetMethod("OnDisable", MESSAGE_FLAGS)?.Invoke(updateSystem, null);
			} else if(typeof(T).IsAssignableFrom(typeof(DrawSystem)) && _enabledDrawSystems.TryGetValue(typeof(T), out DrawSystem drawSystem)) {
				_enabledDrawSystems.Add(typeof(T), drawSystem);
				typeof(T).GetMethod("OnDisable", MESSAGE_FLAGS)?.Invoke(drawSystem, null);
			}
		}

		protected override void Update(GameTime gameTime) {
			float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			InputManager.Update(deltaTime);
			foreach(UpdateSystem system in _enabledUpdateSystems.Values) {
				system.Update(deltaTime);
			}
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.SetRenderTarget(Target);
			GraphicsDevice.Clear(new Color(0x2d,0x9c,0x42));// new Color(0x40, 0x9c, 0x2b) newer // new Color(0x30, 0x9d, 0x6c) old
			SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
			float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			foreach(DrawSystem system in _enabledDrawSystems.Values) {
				system.Draw();
			}
			SpriteBatch.End();

			GraphicsDevice.SetRenderTarget(null);
			_targetBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
			_targetBatch.Draw(Target, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
			_targetBatch.End();
			base.Draw(gameTime);
		}

		protected override void OnExiting(object sender, EventArgs args) {
			End();
		}

		private void End() {
			foreach(System system in _enabledUpdateSystems.Values) {
				system.GetType().GetMethod("OnDisable", MESSAGE_FLAGS)?.Invoke(system, null);
			}
			_enabledUpdateSystems.Clear();
			foreach(System system in _enabledDrawSystems.Values) {
				system.GetType().GetMethod("OnDisable", MESSAGE_FLAGS)?.Invoke(system, null);
			}
			_enabledDrawSystems.Clear();

			foreach(System system in _updateSystems.Values) {
				system.GetType().GetMethod("OnEnd", MESSAGE_FLAGS)?.Invoke(system, null);
			}
			foreach(System system in _drawSystems.Values) {
				system.GetType().GetMethod("OnEnd", MESSAGE_FLAGS)?.Invoke(system, null);
			}
		}

		private void RegisterSystem(System system) {
			Type t = system.GetType();
			_systemDictionary.Add(t, system);
			if(system is UpdateSystem updateSystem) {
				_updateSystems.Add(t, updateSystem);
				_enabledUpdateSystems.Add(t, updateSystem);
				system.GetType().GetMethod("OnEnable", MESSAGE_FLAGS)?.Invoke(system, null);
			} else if(system is DrawSystem drawSystem) {
				_drawSystems.Add(t, drawSystem);
				_enabledDrawSystems.Add(t, drawSystem);
				system.GetType().GetMethod("OnEnable", MESSAGE_FLAGS)?.Invoke(system, null);
			}
		}

		public List<Guid> LoadEntities(string filePath) {
			using FileStream stream = File.OpenRead(filePath);
			JsonDocument doc = JsonDocument.Parse(stream);
			List<Guid> eids = new List<Guid>();
			foreach(JsonElement entityElement in doc.RootElement.GetProperty("Entitites").EnumerateArray()) {
				eids.Add(LoadEntity(entityElement));
			}
			return eids;
		}

		public Guid LoadEntity(string entityJson) {
			using(JsonDocument document = JsonDocument.Parse(entityJson))
				return LoadEntity(document.RootElement);
		}

		public Guid LoadEntity(JsonElement entityJson) {
			List<object> components = new List<object>();
			Guid eid;
			if(entityJson.TryGetProperty("ID", out JsonElement idElement)){
				eid = idElement.GetGuid();
			} else {
				eid = Guid.NewGuid();
			}

			if(entityJson.TryGetProperty("Components", out JsonElement componentsElement)){
				foreach(JsonProperty componentJson in componentsElement.EnumerateObject()) {
					components.Add(ComponentParser.Parse(componentJson));
				}
			}
			MakeEntity(eid, components);
			return eid;
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
			Asset asset = JsonSerializer.Deserialize(data.GetRawText(), assetType, _jsonSerializerOptions) as Asset;
			if(asset == null)
				throw new Exception("Asset type is not Asset");
			_assets.Add(assetID, asset);
		}
		
		private void LoadSystems(string filePath) {

		}
	}
}
