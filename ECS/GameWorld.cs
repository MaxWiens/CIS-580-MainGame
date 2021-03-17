using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MainGame.Collections;
using System.Text.Json;
using System.Linq;
using MainGame.Components;
namespace ECS {
	using S;
	public partial class GameWorld {
		private const BindingFlags MESSAGE_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance;
		
		public readonly float FixedDeltaTime;
		public const string DEFAULT_SCENE_NAME = "Default";

		private readonly List<SceneData> _sceneList = new List<SceneData>();
		public Guid TopScene => _sceneList[_sceneList.Count-1].SID;
		public Guid[] Scenes => (from SceneData d in _sceneList select d.SID).ToArray();
		private readonly Dictionary<Guid, SceneData> _scenes = new Dictionary<Guid, SceneData>();
		private readonly Dictionary<string, Guid> _entityNames = new Dictionary<string, Guid>();
		private readonly Dictionary<Guid, EntityData> _entities = new Dictionary<Guid, EntityData>();
		
		/// <summary> Holds components in a structure that can easily re-enable and access the components in a disabled entity </summary>
		private readonly Dictionary<Guid, Dictionary<Type, object>> _disabledComponentStore = new Dictionary<Guid, Dictionary<Type, object>>();
		private readonly Dictionary<Type, IKeyRefMap<Guid>> _enabledComponentStore = new Dictionary<Type, IKeyRefMap<Guid>>();

		private readonly Dictionary<Type, SystemData> _systems = new Dictionary<Type, SystemData>();
		private readonly SortedDictionary<uint, IUpdateable> _enabledUpdateSystems = new SortedDictionary<uint, IUpdateable>();
		private readonly SortedDictionary<uint, IFixedUpdatable> _enabledFixedUpdateSystems = new SortedDictionary<uint, IFixedUpdatable>();
		private readonly SortedDictionary<uint, IDrawable> _enabledDrawSystems = new SortedDictionary<uint, IDrawable>();
		public event Action Reset;
		public float DeltaTimeScale = 1f;
		private float _fixedTimeTimer = 0f;
		private readonly tainicom.Aether.Physics2D.Dynamics.World _physicsWorld;

		public GameWorld(JsonSerializerOptions entitySerializerOptions, tainicom.Aether.Physics2D.Dynamics.World physicsWorld, float fixedDeltaTime = 1f/60f) {
			_physicsWorld = physicsWorld;
			FixedDeltaTime = fixedDeltaTime;
			_entitySerializerOptions = entitySerializerOptions;
			// add default scene
			AddScene(Guid.Empty, DEFAULT_SCENE_NAME);
		}

		public void Update(float deltaTime) {
			deltaTime *= DeltaTimeScale;
			_fixedTimeTimer += deltaTime;
			if(_fixedTimeTimer >= FixedDeltaTime) {
				_fixedTimeTimer -= FixedDeltaTime;
				foreach(IFixedUpdatable fixedUpdatable in _enabledFixedUpdateSystems.Values)
					fixedUpdatable.FixedUpdate(FixedDeltaTime);
			}

			foreach(IUpdateable updatable in _enabledUpdateSystems.Values)
				updatable.Update(deltaTime);
		}

		public void Draw() {
			foreach(IDrawable drawable in _enabledDrawSystems.Values)
				drawable.Draw();
		}

		#region Entity
		public Guid MakeEntity(Guid sceneID, bool isEnabled = true) => MakeEntity(Guid.NewGuid(), sceneID, isEnabled);

		public Guid MakeEntity(Guid eid, Guid sceneID, bool isEnabled = true, string name = null) {
			EntityData data = new EntityData(eid, sceneID, new HashSet<Type>(), isEnabled, name);
			_entities.Add(eid, data);
			if(name != null)
				_entityNames.Add(name, eid);
			if(!isEnabled) {
				_disabledComponentStore.Add(eid, new Dictionary<Type, object>());
			}
			_scenes[sceneID].Entities.Add(eid);
			return eid;
		}

		public Guid MakeEntity(Guid eid, Guid sceneID, IEnumerable<object> components, bool isEnabled = true, string name = null) {
			MakeEntity(eid, sceneID, isEnabled, name);
			foreach(object component in components)
				AddComponent(eid, component);
			return eid;
		}

		public void DestroyEntity(Guid eid) {
			EntityData data = _entities[eid];
			DestroyEntityNotScene(data);
			_scenes[data.SceneID].Entities.Remove(eid);			
		}

		

		private void DestroyEntityNotScene(EntityData data) {
			OnDestoryHandler value = default;
			value = TryGetComponent(data.EID , ref value, out bool isSuccessful);
			if(isSuccessful) {
				value.Script.Call("OnDestroy", data.EID);
			}


			foreach(var componentType in data.Components) {
				if(data.IsEnabled) {
					if(_enabledComponentStore.TryGetValue(componentType, out var componentMap)) {
						if(componentType == typeof(MainGame.Components.Body))
							_physicsWorld.Remove((MainGame.Components.Body)componentMap[data.EID]);
						componentMap.Remove(data.EID);
					}
				} else {
					_disabledComponentStore[data.EID].Remove(componentType);
				}
			}
			_entities.Remove(data.EID);
			if(data.Name != null)
				_entityNames.Remove(data.Name);
		}
		
		public Guid GetEID(string name) {
			return _entityNames[name];
		}
		public bool TryGetEID(string name, out Guid eid)
			=> _entityNames.TryGetValue(name, out eid);

		public ref T GetComponent<T>(Guid eid)
			=> ref (_enabledComponentStore[typeof(T)] as RefMap<Guid, T>)[eid];
		
		public ref T TryGetComponent<T>(Guid entityID, ref T fallbackValue, out bool isSuccessful) {
			if(_enabledComponentStore.TryGetValue(typeof(T), out var bag)) {
				return ref (bag as RefMap<Guid, T>).TryGetValue(entityID, ref fallbackValue, out isSuccessful);
			}
			isSuccessful = false;
			return ref fallbackValue;
		}

		public IRefMap<Guid,T> GetEntitiesWithComponent<T>() {
			IKeyRefMap<Guid> map;
			if(_enabledComponentStore.TryGetValue(typeof(T), out map)){
			} else {
				map = new RefMap<Guid, T>();
				_enabledComponentStore.Add(typeof(T), map);
			}
			return map as IRefMap<Guid, T>;
		}

		public void AddComponent<T>(Guid eid, T component) {
			EntityData data = _entities[eid];
			Type componentType = typeof(T);
			if(data.IsEnabled) {
				if(!_enabledComponentStore.TryGetValue(componentType, out IKeyRefMap<Guid> map)) {
					map = typeof(RefMap<,>).MakeGenericType(typeof(Guid), componentType).GetConstructor(Type.EmptyTypes).Invoke(null) as IKeyRefMap<Guid>;
					_enabledComponentStore.Add(componentType, map);
				}
				map.Add(eid, component);
			} else {
				_disabledComponentStore[eid].Add(componentType, componentType);
			}
			data.Components.Add(componentType);
		}
		public void AddComponent(Guid eid, object component) {
			EntityData data = _entities[eid];
			Type componentType = component.GetType();
			if(data.IsEnabled) {
				if(!_enabledComponentStore.TryGetValue(componentType, out IKeyRefMap<Guid> map)) {
					map = typeof(RefMap<,>).MakeGenericType(typeof(Guid), componentType).GetConstructor(Type.EmptyTypes).Invoke(null) as IKeyRefMap<Guid>;
					_enabledComponentStore.Add(componentType, map);
				}
				map.Add(eid, component);
			} else {
				_disabledComponentStore[eid].Add(componentType, componentType);
			}
			data.Components.Add(componentType);
		}

		public void RemoveComponent(Guid eid, Type componentType) {
			EntityData data = _entities[eid];
			if(data.IsEnabled) {
				if(_enabledComponentStore.TryGetValue(componentType, out var componentMap))
					componentMap.Remove(eid);
			} else {
				_disabledComponentStore[eid].Remove(componentType);
			}
			data.Components.Remove(componentType);
		}
		public void RemoveComponent<T>(Guid eid) => RemoveComponent(eid, typeof(T));

		public void Enable(Guid eid) {
			EntityData data = _entities[eid];
			if(_disabledComponentStore.Remove(eid, out var disabledComponentMap)) {
				foreach(var componentTypeValue in disabledComponentMap) {
					if(!_enabledComponentStore.TryGetValue(componentTypeValue.Key, out var map)) {
						// if componet store doesn't have refmap for this type of component, add it
						map = typeof(RefMap<,>).MakeGenericType(typeof(Guid), componentTypeValue.Key).GetConstructor(Type.EmptyTypes).Invoke(null) as IKeyRefMap<Guid>;
						_enabledComponentStore.Add(componentTypeValue.Key, map);
					}
					map.Add(eid, componentTypeValue.Value);
				}
				data.IsEnabled = true;
			}
		}

		public void Disable(Guid eid) {
			EntityData data = _entities[eid];
			if(data.IsEnabled) {
				var disabledDict = new Dictionary<Type, object>();
				_disabledComponentStore.Add(eid, disabledDict);
				foreach(Type componentType in data.Components) {
					_enabledComponentStore[componentType].Remove(eid, out object value);
					disabledDict.Add(componentType, value);
				}
				data.IsEnabled = false;
			}
		}

		public bool IsEnabled(Guid eid) => _entities[eid].IsEnabled;

		private class EntityData {
			public EntityData(Guid eid, Guid sceneID, HashSet<Type> components, bool isEnabled, string name = null) {
				EID = eid;
				SceneID = sceneID;
				Components = components;
				IsEnabled = isEnabled;
				Name = name;
			}
			public readonly string Name;
			public readonly Guid EID;
			public readonly HashSet<Type> Components;
			public Guid SceneID;
			public bool IsEnabled;
		}
		#endregion

		#region System
		public System GetSystem(Type type) => _systems.TryGetValue(type, out SystemData data) ? data.System : null;
		public T GetSystem<T>() where T : System => _systems.TryGetValue(typeof(T), out SystemData data) ? (T)data.System : null;

		public void SetEnabled(Type type, bool enabled) {
			if(_systems.TryGetValue(type, out SystemData data)) {
				data.IsEnabled = enabled;
				System system = data.System;
				if(enabled) {
					if(system is IUpdateable updatable) _enabledUpdateSystems.Add(data.Priority, updatable);
					if(system is IFixedUpdatable fixedUpdatable) _enabledFixedUpdateSystems.Add(data.Priority, fixedUpdatable);
					if(system is IDrawable drawable) _enabledDrawSystems.Add(data.Priority, drawable);
					
					if(system is IEnableHandler enableHandler) enableHandler.OnEnable();
				} else {
					if(system is IUpdateable) _enabledUpdateSystems.Remove(data.Priority);
					if(system is IFixedUpdatable) _enabledFixedUpdateSystems.Remove(data.Priority);
					if(system is IDrawable) _enabledDrawSystems.Remove(data.Priority);

					if(system is IDisableHandler disableHandler) disableHandler.OnDisable();
				}
			}
		}
		public void SetEnabled<T>(bool enabled) where T : System => SetEnabled(typeof(T), enabled);

		public bool IsEnabled(Type systemType) => _systems.TryGetValue(systemType, out SystemData data) && data.IsEnabled;
		public bool IsEnabled<T>() where T : System => _systems.TryGetValue(typeof(T), out SystemData data) && data.IsEnabled;

		public bool RegisterSystem(System system, ushort priority = ushort.MaxValue, bool beginEnabled = true) {
			Type systemType = system.GetType();
			int systemCount = _systems.Count;
			if(_systems.ContainsKey(systemType) || systemCount > ushort.MaxValue) return false;
			SystemData data = new SystemData(system, ((uint)priority << 16) | (ushort)systemCount, beginEnabled);
			_systems.Add(systemType, data);
			if(beginEnabled) {
				if(system is IUpdateable updatable) _enabledUpdateSystems.Add(data.Priority, updatable);
				if(system is IFixedUpdatable fixedUpdatable) _enabledFixedUpdateSystems.Add(data.Priority, fixedUpdatable);
				if(system is IDrawable drawable) _enabledDrawSystems.Add(data.Priority, drawable);

				if(system is IEnableHandler enableHandler) enableHandler.OnEnable();
			}
			return true;
		}

		private class SystemData {
			public SystemData(System system, uint priority, bool isEnabled) {
				Priority = priority; ;
				System = system;
				IsEnabled = isEnabled;
			}
			public readonly System System;
			public readonly uint Priority;
			public bool IsEnabled;
		}
		#endregion

		#region Scene
		public void AddScene(Guid sid, string name) {
			SceneData data = new SceneData(sid, name);
			_scenes.Add(sid, data);
			_sceneList.Add(data);
		}
		
		public void RemoveScene(Guid sid) {
			if(_scenes.TryGetValue(sid, out SceneData sceneData)){
				foreach(Guid eid in sceneData.Entities) {
					EntityData data = _entities[eid];
					DestroyEntityNotScene(data);
				}
				if(sid == Guid.Empty) {
					// reset default scene's timescale
					_scenes[sid].Entities.Clear();
					sceneData.TimeScale = 1f;
				} else {
					// remove scene from list
					_sceneList.Remove(sceneData);
					_scenes.Remove(sid);
				}
			}
		}

		public void PopScene() {
			SceneData sid = _sceneList[_sceneList.Count - 1];
			RemoveScene(sid.SID);
		}

		public void RemoveAllScenes() {
			while(_sceneList.Count > 1)
				PopScene();
			PopScene();
			Reset?.Invoke();
		}

		public void SetTimeScale(Guid sid, float value) => _scenes[sid].TimeScale = value;
		public float GetSceneTimeScale(Guid sid) => _scenes[sid].TimeScale;
		public float GetEntityTimeScale(Guid eid) => _scenes[_entities[eid].SceneID].TimeScale;

		private class SceneData {
			public SceneData(Guid sid, string name, float timeScale = 1f) {
				SID = sid;
				Name = name;
				TimeScale = timeScale;
			}
			public readonly Guid SID;
			public readonly string Name;
			public float TimeScale;
			public HashSet<Guid> Entities = new HashSet<Guid>();
		}
		#endregion
	}
}
