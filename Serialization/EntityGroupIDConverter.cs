using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using ECS;
namespace MainGame.Serialization {
	class EntityGroupIDConverter : JsonConverter<ID> {
		private readonly List<ID> _idTagList;
		private readonly List<ID> _globalIDTagList;

		public EntityGroupIDConverter(List<ID> idTagList, List<ID> globalIDTagList) {
			_idTagList = idTagList;
			_globalIDTagList = globalIDTagList;
		}

		public override ID Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			string s = reader.GetString();
			ID id;
			switch(s[0]) {
				case '#':
					id = new ID(s);
					_idTagList.Add(id);
					break;
				case '$':
					id = new ID(s[1..]);
					_globalIDTagList.Add(id);
					break;
				default:
					id = (ID)Guid.Parse(s);
					break;
			}
			return id;
		}

		public override void Write(Utf8JsonWriter writer, ID value, JsonSerializerOptions options) {
			// this might be bad :)
			if(value.Tag != string.Empty)
				writer.WriteStringValue(value.Tag);
			else
				writer.WriteStringValue(value.Guid);
		}
	}
}