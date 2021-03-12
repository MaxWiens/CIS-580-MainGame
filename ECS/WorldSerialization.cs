using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MainGame.Serialization;
namespace ECS {
	public partial class World {

		private readonly JsonSerializerOptions _entitySerializerOptions;

		public IList<Guid> LoadEntityGroupFromFile(string filepath, Guid sceneID) {
			using FileStream stream = File.OpenRead(filepath);
			using JsonDocument doc = JsonDocument.Parse(stream);
			JsonElement root = doc.RootElement;
			Dictionary<string, Guid> localTagIDs = new Dictionary<string, Guid>();
			JsonSerializerOptions op = new JsonSerializerOptions(_entitySerializerOptions);
			op.Converters.Add(new LocalTagGuidConverter(localTagIDs));
			List<Guid> eids = new List<Guid>();
			List<object> components;
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
				eids.Add(eid);
				bool isEnabled = true;
				if(elm.TryGetProperty("IsEnabled", out value))
					isEnabled = value.GetBoolean();

				components = new List<object>();
				if(elm.TryGetProperty("Components", out JsonElement componentsElement)){
					foreach(JsonProperty componentProperty in componentsElement.EnumerateObject()) {
						components.Add(ComponentParser.Parse(componentProperty, op));
					}
				}
				MakeEntity(eid, sceneID, components, isEnabled);
			}
			return eids;
		}
	}
}
