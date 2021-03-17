using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECS {
	public static class ComponentParser {
		public const string COMPONENT_NAMESPACE = "MainGame.Components.";

		public static IComponent Parse(Type componentType, JsonElement json, JsonSerializerOptions options)
			=> (IComponent)JsonSerializer.Deserialize(json.GetRawText(), componentType, options);

		public static IComponent Parse(Type componentType, string json, JsonSerializerOptions options) {
			JsonDocument doc = JsonDocument.Parse(json);
			IComponent t = Parse(componentType, doc.RootElement, options);
			doc.Dispose();
			return t;
		}

		public static IComponent Parse(JsonProperty json, JsonSerializerOptions options)
			=> Parse(Type.GetType(COMPONENT_NAMESPACE + json.Name), json.Value, options);
			
		public static T Parse<T>(string json, JsonSerializerOptions options)
			=> (T)JsonSerializer.Deserialize(json, typeof(T), options);

		public static T Parse<T>(JsonElement json, JsonSerializerOptions options)
			=> (T)JsonSerializer.Deserialize(json.GetRawText(), typeof(T), options);
	}

}
