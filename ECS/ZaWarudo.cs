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
	public class ZaWarudo : Game {
		private readonly Dictionary<Type, IKeyBag<Guid>> _componentStore;
		private readonly Dictionary<Guid, HashSet<Type>> _entities;
		private readonly Dictionary<Type, UpdateSystem> _updateSystems;

		private readonly Dictionary<Type, UpdateSystem> _enabledUpdateSystems;
		private readonly Dictionary<Type, DrawSystem> _drawSystems;
		private readonly Dictionary<Type, DrawSystem> _enabledDrawSystems;
		private readonly IEnumerable<System> _systems;
		private const BindingFlags MESSAGE_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance;

		private GraphicsDeviceManager _graphics;

		public readonly InputManager InputManager;

		public ZaWarudo() {
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.Title = "Minecraft 2";
			InputManager = new InputManager();

			_entities = new Dictionary<Guid, HashSet<Type>>();
			_componentStore = new Dictionary<Type, IKeyBag<Guid>>();
			
			_updateSystems = new Dictionary<Type, UpdateSystem>();
			_enabledUpdateSystems = new Dictionary<Type, UpdateSystem>();
			_drawSystems = new Dictionary<Type, DrawSystem>();
			_enabledDrawSystems = new Dictionary<Type, DrawSystem>();

			_systems = GetSystems();
			foreach(System system in _systems)
				RegisterSystem(system);
		}

		private System[] GetSystems() => new System[]{
			new PlayerController(this),
			new SpriteDraw(this), 
		};

		protected override void Initialize() {
			LoadEntities(@"Content\TestScene.json");

			foreach(System system in _systems) {
				system.GetType().GetMethod("OnInit", MESSAGE_FLAGS)?.Invoke(system, null);
			}
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
			_entities.Add(eid, new HashSet<Type>());
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

		public Bag<Guid,T> GetEntitiesWithComponent<T>() where T : struct {
			_componentStore.TryGetValue(typeof(T), out var entities);
			return entities as Bag<Guid, T>;
		}

		public void AddComponent(Guid entityID, object component) {
			Type componentType = component.GetType();
			if(_entities.TryGetValue(entityID, out var components)) {
				components.Add(componentType);
			} else {
				// entity does'nt exist
				return;
			}
			if(_componentStore.TryGetValue(componentType, out IKeyBag<Guid> entities)) {
				entities.Add(entityID, component);
			} else {
				Type bagtype = typeof(Bag<,>).MakeGenericType(typeof(Guid), componentType);
				IKeyBag<Guid> bag = Activator.CreateInstance(bagtype) as IKeyBag<Guid>;
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
				entities = new Bag<Guid, T>();
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
			return ref (_componentStore[typeof(T)] as Bag<Guid, T>)[entityID];
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
			GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.DarkGreen);
			float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			foreach(DrawSystem system in _enabledDrawSystems.Values) {
				system.Draw();
			}
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

		public void LoadEntities(string filePath) {
			using(FileStream stream = File.OpenRead(filePath)) {
				JsonDocument doc = JsonDocument.Parse(stream);

				foreach(JsonElement entityJson in doc.RootElement.GetProperty("Entitites").EnumerateArray()) {
					LoadEntity(entityJson.GetRawText());
				}
			}
		}

		public void LoadEntity(string entityJson) {
			using(JsonDocument document = JsonDocument.Parse(entityJson))
				LoadEntity(document.RootElement);
		}

		public void LoadEntity(JsonElement entityJson) {
			List<object> components = new List<object>();
			Guid eid;
			if(entityJson.TryGetProperty("ID", out JsonElement idElement)){
				eid = idElement.GetGuid();
			} else {
				eid = Guid.NewGuid();
			}

			if(entityJson.TryGetProperty("Components", out JsonElement componentsElement)){
				foreach(JsonProperty componentJson in componentsElement.EnumerateObject()) {
					Type componentType = Type.GetType("MainGame.Components." + componentJson.Name);
					object c = JsonSerializer.Deserialize(componentJson.Value.GetRawText(), componentType, Serialization.JsonDefaults.Options);
					components.Add(c);
				}
			}
			MakeEntity(eid, components);
		}
		
		private void LoadSystems(string filePath) {

		}
	}
}
