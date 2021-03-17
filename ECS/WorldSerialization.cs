using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MainGame.Serialization;
namespace ECS {
	public partial class GameWorld {
		private Guid _currentlySerializingEntity;
		public Guid CurrentlySerializingEntity => _currentlySerializingEntity;

		private Dictionary<string, EntityGroup> _loadedEntityGroups = new Dictionary<string, EntityGroup>();

		private readonly JsonSerializerOptions _entitySerializerOptions;
		public IList<Guid> LoadEntityGroupFromFile(string filepath)
			=> LoadEntityGroupFromFile(filepath, Guid.Empty);

		public IList<Guid> LoadEntityGroupFromFile(string filepath, Guid sceneID) {
			using FileStream stream = File.OpenRead(filepath);
			using JsonDocument doc = JsonDocument.Parse(stream);
			JsonElement root = doc.RootElement;
			Dictionary<string, Guid> localTagIDs = new Dictionary<string, Guid>();
			JsonSerializerOptions op = new JsonSerializerOptions(_entitySerializerOptions);
			op.Converters.Add(new LocalTagGuidConverter(localTagIDs));
			List<Guid> eids = new List<Guid>();
			List<object> components;

			if(root.TryGetProperty("SID", out JsonElement SceneIDElm)){
				sceneID = SceneIDElm.GetGuid();
				if(!_scenes.ContainsKey(sceneID)) {
					AddScene(sceneID, string.Empty);
				}
			}

			foreach(JsonElement elm in root.GetProperty("Entities").EnumerateArray()) {
				//load entities
				Guid eid;
				if(elm.TryGetProperty("EID", out JsonElement value)) {
					string s = value.GetString();
					if(s[0] == '#') {
						// local tag identifier
						if(!localTagIDs.TryGetValue(s, out eid)) {
							eid = Guid.NewGuid();
							localTagIDs.Add(s, eid);
						}
					} else {
						eid = Guid.Parse(s);
					}
				} else {
					eid = Guid.NewGuid();
				}
				_currentlySerializingEntity = eid;
				eids.Add(eid);
				bool isEnabled = true;
				if(elm.TryGetProperty("IsEnabled", out value))
					isEnabled = value.GetBoolean();

				string name = null;
				if(elm.TryGetProperty("Name", out value))
					name = value.GetString();

				components = new List<object>();
				if(elm.TryGetProperty("Components", out JsonElement componentsElement)){
					foreach(JsonProperty componentProperty in componentsElement.EnumerateObject()) {
						components.Add(ComponentParser.Parse(componentProperty, op));
					}
				}
				MakeEntity(eid, sceneID, components, isEnabled, name);
			}
			return eids;
		}

		public IEnumerable<Guid> CloneEntityGroup(string path) {
			string fullPath = Path.GetFullPath(path);
			if(!_loadedEntityGroups.TryGetValue(fullPath, out EntityGroup group)) {
				group = EntityGroup.LoadEntityGroup(path, this);
				_loadedEntityGroups.Add(fullPath, group);
			}
			return group.BuildCopy(this);
		}

		public class EntityGroup {
			public readonly string Name;
			public readonly string Path;
			private readonly List<ID> _tagids = new List<ID>();
			private readonly List<ID> _globalTagIDList = new List<ID>();
			private readonly Entity[] _entities;

			private EntityGroup(string groupName, string path, Entity[] entities, List<ID> tagids, List<ID> globalTagIDList) {
				Name = groupName;
				Path = path;
				_entities = entities;
				_tagids = tagids;
				_globalTagIDList = globalTagIDList;
			}

			public struct Entity {
				public ID EID;
				public string Name;
				public bool IsEnabled;
				public List<IComponent> Components;
			}

			public IEnumerable<Guid> BuildCopy(GameWorld world) {
				foreach(ID id in _globalTagIDList)
					id.Guid = world._entityNames[id.Tag];

				Dictionary<string, Guid> tagMap = new Dictionary<string, Guid>();
				foreach(ID id in _tagids) {
					if(!tagMap.TryGetValue(id.Tag, out Guid guid))
						tagMap.Add(id.Tag, guid = Guid.NewGuid());
					id.Guid = guid;
				}

				List<IComponent> components;
				IComponent[] copiedComponents;
				Entity e;
				Guid[] entityGUIDs = new Guid[_entities.Length];
				for(int i = 0; i < _entities.Length; i++) {
					e = _entities[i];
					if(e.EID.Tag == null && !e.EID.GivenValue) e.EID.Guid = Guid.NewGuid();
					entityGUIDs[i] = e.EID;
					copiedComponents = new IComponent[e.Components.Count];
					components = e.Components;
					for(int j = 0; j < components.Count; j++)
						copiedComponents[j] = (IComponent)components[j].Clone();
					world.MakeEntity(e.EID, Guid.Empty, copiedComponents, e.IsEnabled, e.Name);
				}
				return entityGUIDs;
			}

			public static EntityGroup LoadEntityGroup(string filepath, GameWorld world) {
				List<IComponent> components;
				ID id;
				JsonElement entityElement;

				List<ID> tagIDList = new List<ID>();
				List<ID> globalTagIDList = new List<ID>();
				JsonSerializerOptions op = new JsonSerializerOptions(world._entitySerializerOptions);
				op.Converters.Add(new EntityGroupIDConverter(tagIDList, globalTagIDList));

				using FileStream stream = File.OpenRead(filepath);
				using JsonDocument doc = JsonDocument.Parse(stream);
				JsonElement root = doc.RootElement;
				string entityGroupName = root.GetProperty("Name").GetString();
				JsonElement EntitiesElement = root.GetProperty("Entities");
				Entity[] entities = new Entity[EntitiesElement.GetArrayLength()];
				for(int i = 0; i < entities.Length; i++) {
					entityElement = EntitiesElement[i];
					if(entityElement.TryGetProperty("EID", out JsonElement value)) {
						id = JsonSerializer.Deserialize<ID>(value.GetRawText(), op);
					} else {
						id = new ID();
					}
					bool isEnabled = true;
					if(entityElement.TryGetProperty("IsEnabled", out value))
						isEnabled = value.GetBoolean();

					string name = null;
					if(entityElement.TryGetProperty("Name", out value))
						name = value.GetString();

					components = new List<IComponent>();
					if(entityElement.TryGetProperty("Components", out JsonElement componentsElement)) {
						foreach(JsonProperty componentProp in componentsElement.EnumerateObject()) {
							components.Add(ComponentParser.Parse(componentProp, op));
						}
					}
					entities[i] = new Entity() {
						EID = id,
						Name = name,
						IsEnabled = isEnabled,
						Components = components
					};
				}
				return new EntityGroup(entityGroupName, filepath, entities, tagIDList, globalTagIDList);
			}
		}
	}
}
