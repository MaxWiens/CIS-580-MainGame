using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MainGame.ECS {
	public class ComponentParser {
		public ComponentParser(JsonSerializerOptions options) {
			_options = options;
		}

		private readonly JsonSerializerOptions _options;

		public object Parse(Type componentType, JsonElement json)
			=> JsonSerializer.Deserialize(json.GetRawText(), componentType, _options);

		public object Parse(Type componentType, string json) {
			JsonDocument doc = JsonDocument.Parse(json);
			object t = Parse(componentType, doc.RootElement);
			doc.Dispose();
			return t;
		}

		public object Parse(JsonProperty json)
			=> Parse(Type.GetType("MainGame.Components." + json.Name), json.Value);
			
		public T Parse<T>(string json) where T : struct 
			=> (T)JsonSerializer.Deserialize(json, typeof(T), _options);

		public T Parse<T>(JsonElement json) where T : struct
			=> (T)JsonSerializer.Deserialize(json.GetRawText(), typeof(T), _options);
	}

}
