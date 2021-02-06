using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MainGame.ECS {
	public static class Component {
		
		public static object Parse(Type componentType, JsonElement json)
			=> JsonSerializer.Deserialize(json.GetRawText(), componentType, Serialization.JsonDefaults.Options);

		public static object Parse(Type componentType, string json) {
			JsonDocument doc = JsonDocument.Parse(json);
			object t = Parse(componentType, doc.RootElement);
			doc.Dispose();
			return t;
		}

		public static object Parse(JsonProperty json)
			=> Parse(Type.GetType("MainGame.Components." + json.Name), json.Value);
			
		public static T Parse<T>(string json) where T : struct 
			=> (T)JsonSerializer.Deserialize(json, typeof(T), Serialization.JsonDefaults.Options);

		public static T Parse<T>(JsonElement json) where T : struct
			=> (T)JsonSerializer.Deserialize(json.GetRawText(), typeof(T), Serialization.JsonDefaults.Options);
	}

}
