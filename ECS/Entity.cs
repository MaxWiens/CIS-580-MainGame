using System;
using System.Collections.Generic;
using System.Collections;
using MoonSharp.Interpreter;
using System.Reflection;
using System.Linq;
namespace ECS {
	[MoonSharpUserData]
	public class Entity {

		private static readonly Dictionary<Type, MethodInfo[]> _cachedComponentData = new Dictionary<Type, MethodInfo[]>();

		public readonly Guid EID;
		public readonly string Name;
		public readonly World World;
		private readonly Scene _scene;
		public Scene Scene => _scene;
		public object EntityGroup;
		
		private readonly Dictionary<string, SortedDictionary<PriorityHandler, MessageHandler>> _messageHandles = new Dictionary<string, SortedDictionary<PriorityHandler, MessageHandler>>();
		private readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

		private bool _isEnabled = false;
		public bool IsEnabled => _isEnabled;
		internal bool IsAlive = false;
		
		internal Entity(World world, Guid eid, Scene scene, string name) {
			EID = eid;
			World = world;
			Name = name;
			_scene = scene;
			_scene.entities.Add(this);
		}

		public IComponent GetComponent(Type componentType) => _components[componentType];
		public bool TryGetComponent(Type componentType, out IComponent component) => _components.TryGetValue(componentType, out component);
		public T GetComponent<T>() where T : IComponent => (T)_components[typeof(T)];
		public bool TryGetComponent<T>(out T component) {
			if(_components.TryGetValue(typeof(T), out IComponent com)) {
				component = (T)com;
				return true; 
			}
			component = default;
			return false;	
		}

		public bool AddComponent(IComponent component) {
			Type componentType = component.GetType();
			if(!_components.ContainsKey(componentType)) {
				if(!_cachedComponentData.TryGetValue(componentType, out MethodInfo[] messageHandlers)) {
					messageHandlers = componentType.GetMethods().Where(m => Attribute.IsDefined(m, typeof(MessageHandlerAttribute))).ToArray();
					_cachedComponentData.Add(componentType, messageHandlers);
				}
				component = component.Clone(this);

				foreach(var v in messageHandlers) {
					MessageHandler handler = (MessageHandler)v.CreateDelegate(typeof(MessageHandler), component);
					if(!_messageHandles.TryGetValue(v.Name, out var handlers)) {
						handlers = new SortedDictionary<PriorityHandler, MessageHandler>();// (IPriorityComparer.Comparer);
						_messageHandles.Add(v.Name, handlers);
					}
					handlers.Add(new PriorityHandler(component, v), handler);
				}
				_components.Add(componentType, component);
				if(_isEnabled) {
					if(!World.enabledComponents.TryGetValue(componentType, out var componentStore)) {
						componentStore = new Dictionary<Entity, IComponent>();
						World.enabledComponents.Add(componentType, componentStore);
					}
					componentStore.Add(this, component);
				}
				return true;
			}
			return false;
		}

		public bool RemoveComponent(Type componentType) {
			if(_components.TryGetValue(componentType, out IComponent value)) {
				_components.Remove(componentType);
				if(_isEnabled)
					World.enabledComponents[componentType].Remove(this);
				PriorityHandler p = new PriorityHandler(value);
				foreach(var v in _cachedComponentData[componentType])
					_messageHandles[v.Name].Remove(p);

				return true;
			}
			return false;
		}

		public void RemoveAllComponents() {
			while(_components.Count > 0) {
				RemoveComponent(_components.Keys.First());
			}
		}

		public void Enable() {
			if(IsAlive && !_isEnabled) {
				_isEnabled = true;
				foreach(var kvp in _components) {
					if(!World.enabledComponents.TryGetValue(kvp.Key, out var map)) {
						map = new Dictionary<Entity, IComponent>();
						World.enabledComponents.Add(kvp.Key, map);
					}
					map.Add(this, kvp.Value);
				}
					
				SendMessage(Message.EnabledMessage);
			}
		}
		
		public void Disable() {
			if(_isEnabled) {
				SendMessage(Message.DisabledMessage);
				_isEnabled = false;

				foreach(var kvp in _components)
					World.enabledComponents[kvp.Key].Remove(this);
			}
		}
		
		public bool SendMessage(Message message) {
			if(IsAlive && _isEnabled && _messageHandles.TryGetValue(message.HandlerName, out SortedDictionary<PriorityHandler, MessageHandler> handlers)){
				foreach(var handler in handlers.Values) {
					if(!handler(message)) return false;
				}
			}
			return true;
		}

		private struct PriorityHandler : IComparable<PriorityHandler> {
			private readonly IComponent _component;
			private readonly ulong _priority;
			public PriorityHandler(IComponent component) {
				_component = component;
				_priority = 0;
			}
			public PriorityHandler(IComponent component, MethodInfo method) {
				MessageHandlerAttribute a = method.GetCustomAttribute<MessageHandlerAttribute>();
				_component = component;
				ulong pri = ((ulong)a.Priority) << 32;
				ulong pri2 = (ulong)component.GetHashCode();
				_priority = pri | pri2;
			}

			public int CompareTo(PriorityHandler other) => _component==other._component ? 0 : _priority.CompareTo(other._priority);
		}

		public void StartCoroutine(IEnumerator coroutine) => World.StartCoroutine(this, coroutine);
	}
}
