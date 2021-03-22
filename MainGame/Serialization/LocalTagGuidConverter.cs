using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MainGame.Serialization {
	class LocalTagGuidConverter : JsonConverter<Guid> {
		private readonly IDictionary<string, Guid> _tagToGuidMap;
		private readonly IDictionary<Guid, string> _guidToTagMap;
		public LocalTagGuidConverter(IDictionary<string,Guid> localTagMap) {
			_tagToGuidMap = localTagMap;
		}

		public LocalTagGuidConverter(IDictionary<Guid, string> guidToTagMap) {
			_guidToTagMap = guidToTagMap;
		}

		public LocalTagGuidConverter(IDictionary<string, Guid> localTagMap, IDictionary<Guid, string> guidToTagMap) {
			_tagToGuidMap = localTagMap;
			_guidToTagMap = guidToTagMap;
		}

		public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			string s = reader.GetString();
			if(s.Length > 0 && s[0] == '#') {
				if(_tagToGuidMap.TryGetValue(s, out Guid id))
					return id;
				id = Guid.NewGuid();
				_tagToGuidMap.Add(s, id);
				return id;
			}
			return Guid.Parse(s);
		}
			

		public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options) {
			// this might be bad :)
			if(_guidToTagMap.TryGetValue(value, out string tag))
				writer.WriteStringValue(tag);
			else
				writer.WriteStringValue(value);
		}
			
	}
}