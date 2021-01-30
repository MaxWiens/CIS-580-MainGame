using System;
using System.Collections.Generic;
using System.Text;
using MainGame.Util;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace MainGame.ECS {
	class World {
		private readonly Dictionary<Type,Dictionary<ulong,List<ulong>>> _componentStore;
		private readonly Dictionary<ulong, HashSet<Type>> _entityComponentTypes;
		private readonly Dictionary<ulong, Component> _components;
		//private readonly Dictionary<Type, >

		public World(IEnumerable<System> systems) {
			_componentStore = new Dictionary<Type, Dictionary<ulong,List<ulong>>>();
			_entityComponentTypes = new Dictionary<ulong, HashSet<Type>>();
			_components = new Dictionary<ulong, Component>();

			foreach(System system in systems) {
				RegisterSystem(system);
			}
		}

		public void EnableSystem<S>() {
			string s = typeof(S).Name;
		}

		public ulong MakeEntity() {
			ulong eid = IDManager.NextID();
			_entityComponentTypes.Add(eid, new HashSet<Type>());
			return eid;
		}

		public ulong MakeEntity(IEnumerable<Component> components) {
			ulong eid = IDManager.NextID();
			foreach(Component c in components) {
				AddComponent(eid, c);
			}
			return eid;
		}

		public void DestroyEntity(ulong entityID) {
			if(_entityComponentTypes.TryGetValue(entityID, out HashSet<Type> components)) {
				Dictionary<ulong, List<ulong>> eidcids;
				foreach(Type t in components) {
					eidcids = _componentStore[t];
					foreach(ulong cid in eidcids[entityID]) {
					}
					eidcids.Remove(entityID);
				}
				_entityComponentTypes.Remove(entityID);
			}
			//_eidManager.ReturnID(entityID);
		}

		public void AddComponent(ulong entityID, Component component) {
			Type componentType = component.GetType();
			if(!_componentStore.TryGetValue(componentType, out Dictionary<ulong, List<ulong>> eidcid)){
				_componentStore.Add(componentType, new Dictionary<ulong, List<ulong>>() { { entityID, new List<ulong>() { component.ID } } });
			} else if(eidcid.TryGetValue(entityID, out List<ulong> cids)) {
				cids.Add(component.ID);
			} else {
				cids = new List<ulong>() { component.ID };
				eidcid.Add(entityID, cids);
			}
			if(_entityComponentTypes.TryGetValue(entityID, out HashSet<Type> components)) {
				components.Add(component.GetType());
			}
		}
		
		public void RemoveComponent(ulong entityID, ulong componentID) {

		}

		public void Simulate(float deltaTime) {

		}
		
		private void RegisterSystem(System system) {
			Type t = system.GetType();
			if(Attribute.GetCustomAttribute(t, typeof(ModifiesComponentsAttribute)) is ModifiesComponentsAttribute ma) {
				foreach(Type component in ma.Modifies) {

				}
			}
			if(Attribute.GetCustomAttribute(t, typeof(ReadsComponentsAttribute)) is ReadsComponentsAttribute ra) {
				foreach(Type component in ra.Reads) {

				}
			}
		}
	}
}
