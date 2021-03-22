using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ECS.Serialization;

namespace ECS {
	public partial class World {
		private Entity _currentlySerializingEntity;
		public Entity CurrentlySerializingEntity => _currentlySerializingEntity;

		private Dictionary<string, EntityGroupData> _loadedEntityGroups = new Dictionary<string, EntityGroupData>();

		private readonly JsonSerializerOptions _entitySerializerOptions;
		private readonly JsonSerializerOptions _entityGroupSerializerOptions;

		public IList<Entity> LoadEntityGroupFromFile(string filepath)
			=> LoadEntityGroupFromFile(filepath, _defaultScene.Name);

		public IList<Entity> LoadEntityGroupFromFile(string filepath, string sceneName) {
			using(FileStream stream = File.OpenRead(filepath))
			using(JsonDocument doc = JsonDocument.Parse(stream)) {
				JsonElement root = doc.RootElement;
				Dictionary<string, Guid> localTagIDs = new Dictionary<string, Guid>();
				JsonSerializerOptions op = new JsonSerializerOptions(_entitySerializerOptions);
				op.Converters.Add(new IDConverter(localTagIDs, _entityNames));
				List<Entity> loadedEntities = new List<Entity>();
				List<object> components;

				Scene scene = null;

				if(root.TryGetProperty("Scene", out JsonElement SceneIDElm)) {
					sceneName = SceneIDElm.GetString();
					if(!_sceneNames.TryGetValue(sceneName, out scene)) {
						scene =  MakeScene(sceneName);
					}
				}

				JsonElement prop;

				foreach(JsonElement entityElement in root.GetProperty("Entities").EnumerateArray()) {
					//load entities
					bool isEnabled = true;
					if(entityElement.TryGetProperty("IsEnabled", out prop))
						isEnabled = prop.GetBoolean();

					string name = null;
					if(entityElement.TryGetProperty("Name", out prop))
						name = prop.GetString();

					Entity entity = MakeEntity(name, scene, null, false);
					loadedEntities.Add(entity);
					_currentlySerializingEntity = entity;
					components = new List<object>();
					if(entityElement.TryGetProperty("Components", out JsonElement componentsElement)) {
						foreach(JsonProperty componentProperty in componentsElement.EnumerateObject()) {
							entity.AddComponent(ComponentParser.Parse(componentProperty, op));
						}
					}
					if(isEnabled)
						entity.Enable();
				}
				return loadedEntities;
			}
		}

		public IList<Entity> CloneEntityGroup(string path) {
			string fullPath = Path.GetFullPath(path);
			if(!_loadedEntityGroups.TryGetValue(fullPath, out EntityGroupData group)) {
				group = EntityGroupData.LoadEntityGroup(path, this);
				_loadedEntityGroups.Add(fullPath, group);
			}
			return group.BuildCopy(this);
		}

		public bool SaveEntityGroup(object GroupKey) {
			if(_entityGroups.TryGetValue(GroupKey, out var entities)) {
				foreach(Entity e in entities) {
					// save to file
				}
				return true;
			}
			return false;
		}

		public class EntityGroupData {
			public readonly string Path;
			private readonly List<ID> _tagids = new List<ID>();
			private readonly List<ID> _globalTagIDList = new List<ID>();
			private readonly EntityData[] _entities;

			private EntityGroupData(string path, EntityData[] entities, List<ID> tagids, List<ID> globalTagIDList) {
				Path = path;
				_entities = entities;
				_tagids = tagids;
				_globalTagIDList = globalTagIDList;
			}

			public struct EntityData {
				public ID EID;
				public string Name;
				public bool IsEnabled;
				public List<IComponent> Components;
			}

			public IList<Entity> BuildCopy(World world) {
				foreach(ID id in _globalTagIDList)
					id.Guid = world._entityNames[id.Tag].EID;

				Dictionary<string, Guid> tagMap = new Dictionary<string, Guid>();
				foreach(ID id in _tagids) {
					if(!tagMap.TryGetValue(id.Tag, out Guid guid))
						tagMap.Add(id.Tag, guid = Guid.NewGuid());
					id.Guid = guid;
				}

				List<IComponent> components;
				IComponent[] copiedComponents;
				EntityData e;
				Entity[] clonedEntities = new Entity[_entities.Length];
				for(int i = 0; i < _entities.Length; i++) {
					e = _entities[i];
					//if(e.EID.Tag == null && !e.EID.GivenValue)
					//	e.EID.Guid = Guid.NewGuid();
					//entities[i] = e.EID;
					clonedEntities[i] = world.MakeEntity(e.Components, name: e.Name, isEnabled: e.IsEnabled);
				}
				return clonedEntities;
			}

			public static EntityGroupData LoadEntityGroup(string filepath, World world) {
				List<IComponent> components;
				ID id;
				JsonElement entityElement;

				List<ID> tagIDList = new List<ID>();
				List<ID> globalTagIDList = new List<ID>();
				JsonSerializerOptions op = new JsonSerializerOptions(world._entityGroupSerializerOptions);
				op.Converters.Add(new EntityGroupIDConverter(tagIDList, globalTagIDList));

				using(FileStream stream = File.OpenRead(filepath))
				using(JsonDocument doc = JsonDocument.Parse(stream)) {
					JsonElement root = doc.RootElement;
					JsonElement EntitiesElement = root.GetProperty("Entities");
					EntityData[] entities = new EntityData[EntitiesElement.GetArrayLength()];
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
						entities[i] = new EntityData() {
							EID = id,
							Name = name,
							IsEnabled = isEnabled,
							Components = components
						};
					}
					return new EntityGroupData(filepath, entities, tagIDList, globalTagIDList);
				}
			}
		}
	}
}
