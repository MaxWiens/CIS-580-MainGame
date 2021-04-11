using System;
using System.IO;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECS {
	[MoonSharpUserData]
	public partial class World {
		private readonly Scene _defaultScene = new Scene("Default");
		private readonly Dictionary<string, Scene> _sceneNames = new Dictionary<string, Scene>();
		private readonly List<Scene> _scenes = new List<Scene>();
		private readonly Dictionary<string, Entity> _entityNames = new Dictionary<string, Entity>();
		private readonly Dictionary<Guid, Entity> _entities = new Dictionary<Guid, Entity>();
		private readonly Dictionary<object,HashSet<Entity>> _entityGroups = new Dictionary<object, HashSet<Entity>>();

		private readonly Dictionary<Entity, IComponent> _emptyCompList = new Dictionary<Entity, IComponent>(0);
		internal readonly Dictionary<Type, Dictionary<Entity, IComponent>> enabledComponents = new Dictionary<Type, Dictionary<Entity, IComponent>>();
		private readonly Dictionary<Entity,List<IEnumerator>> _coroutines = new Dictionary<Entity, List<IEnumerator>>();
		public event Action Reset;

		private readonly Dictionary<Type, ISystem> _systems = new Dictionary<Type, ISystem>();
		internal readonly Dictionary<Type, IDrawable> _drawSystems = new Dictionary<Type, IDrawable>();// (IPriorityComparer.Comparer);
		internal readonly Dictionary<Type, IUpdateable> _updateSystems = new Dictionary<Type, IUpdateable>();// (IPriorityComparer.Comparer);
		internal readonly Dictionary<Type, IFixedUpdateable> _fixedUpdateSystems = new Dictionary<Type, IFixedUpdateable>();// (IPriorityComparer.Comparer);

		public float DeltaTimeScale = 1f;
		private readonly float _fixedUpdateTime;

		public World(JsonSerializerOptions entitySerializerOptions, JsonSerializerOptions entityGroupSerializerOptions, float fixedUpdateTime = 1f/60f) {
			_fixedUpdateTime = fixedUpdateTime;
			_entitySerializerOptions = entitySerializerOptions;
			_entityGroupSerializerOptions = entityGroupSerializerOptions;
			AddScene(_defaultScene);
		}

		private float _fixedUpdateTimer = 0f;
		[MoonSharpHidden]
		public void Update(float deltaTime) {
			deltaTime *= DeltaTimeScale;
			foreach(IUpdateable u in _updateSystems.Values) {
				u.Update(deltaTime);
				ActuallyRemoveEntities();
			}
			_fixedUpdateTimer += deltaTime;
			if(_fixedUpdateTimer >= _fixedUpdateTime) {
				FixedUpdate(_fixedUpdateTime);
				_fixedUpdateTimer -= _fixedUpdateTime;
			}
			foreach(var kvp in _coroutines) {
				for(int i = kvp.Value.Count-1; i >= 0; i--) {
					if(!kvp.Value[i].MoveNext())
						StopCoroutine(kvp.Key, kvp.Value[i]);
				}
			}
		}

		[MoonSharpHidden]
		private void FixedUpdate(float fixedDeltaTime) {
			foreach(IFixedUpdateable fu in _fixedUpdateSystems.Values) {
				fu.FixedUpdate(fixedDeltaTime);
				ActuallyRemoveEntities();
			}
				
			
		}

		[MoonSharpHidden]
		public void Draw() {
			foreach(IDrawable d in _drawSystems.Values)
				d.Draw();
		}

		#region Systems
		public void AddSystem(ISystem system) {
			Type systemType = system.GetType();
			if(!_systems.ContainsKey(systemType)) {
				if(system is IUpdateable u)
					_updateSystems.Add(systemType, u);

				if(system is IFixedUpdateable fu)
					_fixedUpdateSystems.Add(systemType,fu);

				if(system is IDrawable d)
					_drawSystems.Add(systemType, d);
				_systems.Add(systemType, system);
			}
		}

		public object GetSystem(Type systemType) => _systems[systemType];
		public T GetSystem<T>() => (T)_systems[typeof(T)];

		#endregion

		#region Entity
		public Entity GetEntity(Guid eid) => _entities.TryGetValue(eid, out Entity entity) ? entity : null;
		public Entity GetEntity(string name) => _entityNames.TryGetValue(name, out Entity entity) ? entity : null;
		public Dictionary<Entity, IComponent> GetEntitiesWith(Type componentType) => enabledComponents[componentType];
		public Dictionary<Entity, IComponent> GetEntitiesWith<T>()
			=> enabledComponents.TryGetValue(typeof(T), out var result) ? result : _emptyCompList;
		
		public Entity MakeEntity(string name = null, Scene scene = null, object group = null, bool isEnabled = false) {
			if(scene == null)
				scene = _defaultScene;
			
			Entity e = new Entity(this, Guid.NewGuid(), scene, name);
			e.IsAlive = true;
			_entities.Add(e.EID, e);
			if(name != null)
				_entityNames.Add(name, e);
			scene.entities.Add(e);
			if(group != null) e.EntityGroup = group;
			if(isEnabled)
				e.Enable();
			return e;
		}

		public Entity MakeEntity(IEnumerable<IComponent> components, string name = null, Scene scene = null, object group = null, bool isEnabled = false) {
			if(scene == null)
				scene = _defaultScene;
			Entity e = new Entity(this, Guid.NewGuid(), scene, name);
			e.IsAlive = true;
			_entities.Add(e.EID, e);
			if(name != null)
				_entityNames.Add(name, e);

			scene.entities.Add(e);
			foreach(IComponent component in components) {
				e.AddComponent(component);
			}
			if(group != null) e.EntityGroup = group;
			if(isEnabled) e.Enable();
			return e;
		}

		public bool RemoveEntity(Guid eid) {
			if(_entities.TryGetValue(eid, out Entity e)) {
				RemoveEntity(e);
				return true;
			}
			return false;
		}

		public bool RemoveEntity(Entity e) {
			e.Disable();
			e.IsAlive = false;
			_entitiesToRemove.Enqueue(e);
			return true;
		}

		private Queue<Entity> _entitiesToRemove = new Queue<Entity>();
		private void ActuallyRemoveEntities() {
			while(_entitiesToRemove.Count > 0) {
				Entity e = _entitiesToRemove.Dequeue();
				e.Disable();
				if(e.EntityGroup != null) {
					_entityGroups[e.EntityGroup].Remove(e);
				}
				e.RemoveAllComponents();
				e.Scene.entities.Remove(e);
				e.SendMessage(Message.DestroyMessage);
				if(e.Name != null)
					_entityNames.Remove(e.Name);
				_entities.Remove(e.EID);

				_coroutines.Remove(e);
			}
		}

		#endregion

		#region Scenes
		public Scene TopScene => _scenes[_scenes.Count - 1];
		public bool PopScene() {
			if(_scenes.Count == 1) {
				for(int i = _defaultScene.entities.Count - 1; i >= 0; i--) {
					RemoveEntity(_defaultScene.entities[i]);
				}
				_defaultScene.entities.Clear();
				return false;
			} else {
				Scene s = _scenes[_scenes.Count - 1];
				for(int i = s.entities.Count - 1; i >= 0; i--) {
					if(s.entities[i].Name == "PlayerCharacter") {
						int l = 0;
					}
					RemoveEntity(s.entities[i]);
				}
				_scenes.RemoveAt(_scenes.Count - 1);
				_sceneNames.Remove(s.Name);
				return true;
			}
		}

		public Scene MakeScene(string Name) {
			Scene s = new Scene(Name);
			AddScene(s);
			return s;
		}

		private void AddScene(Scene s) {
			_scenes.Add(s);
			_sceneNames.Add(s.Name, s);
		}



		public void RemoveAllScenes() {
			while(PopScene());
			Reset?.Invoke();
		}
		#endregion


		public void StartCoroutine(Entity entity, IEnumerator coroutine) {
			if(coroutine.MoveNext()) {
				if(!_coroutines.TryGetValue(entity, out var coroutines)) {
					coroutines = new List<IEnumerator>();
					_coroutines.Add(entity, coroutines);
				}
				coroutines.Add(coroutine);
			}
		}

		public void StopCoroutine(Entity entity, IEnumerator coroutine) {
			if(_coroutines.TryGetValue(entity, out var coroutines))
				coroutines.Remove(coroutine);
		}
	}
}
