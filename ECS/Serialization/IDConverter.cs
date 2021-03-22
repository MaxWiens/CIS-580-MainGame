using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace ECS.Serialization {
	class IDConverter : JsonConverter<ID> {
		private readonly Dictionary<string, Guid> _localTagMap;
		private readonly Dictionary<string, Entity> _globalTagMap;

		public IDConverter(Dictionary<string, Guid> localTagMap, Dictionary<string, Entity> globalTagMap) {
			_localTagMap = localTagMap;
			_globalTagMap = globalTagMap;
		}

		public override ID Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			string s = reader.GetString();
			ID id;
			switch(s[0]) {
				case '#':
					if(!_localTagMap.TryGetValue(s, out Guid guid)){
						guid = Guid.NewGuid();
						_localTagMap.Add(s, guid);
					}
					id = new ID(s) { Guid = guid };
					break;
				case '$':
					string tag = s.Substring(1);
					id = new ID(tag) { Guid = _globalTagMap[tag].EID };
					break;
				default:
					id = new ID(Guid.Parse(s));
					break;
			}
			return id;
		}

		public override void Write(Utf8JsonWriter writer, ID value, JsonSerializerOptions options) {
			// this might be bad :)
			if(value.Tag != string.Empty)
				writer.WriteStringValue(value.Tag);
			else if(value.GivenValue)
				writer.WriteStringValue(value.Guid);
			throw new NotImplementedException();
		}
	}
}