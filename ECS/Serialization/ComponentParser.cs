using System;
using System.Text.Json;
using System.Reflection;

namespace ECS.Serialization {
	public static class ComponentParser {
		public const string COMPONENT_ASSEMBLY_NAME = "MainGame";
		public const string COMPONENT_NAMESPACE = "MainGame.Components.";

		public static IComponent Parse(Type componentType, JsonElement json, JsonSerializerOptions options)
			=> (IComponent)JsonSerializer.Deserialize(json.GetRawText(), componentType, options);
		

		public static IComponent Parse(Type componentType, string json, JsonSerializerOptions options) {
			JsonDocument doc = JsonDocument.Parse(json);
			IComponent t = Parse(componentType, doc.RootElement, options);
			doc.Dispose();
			return t;
		}

		public static IComponent Parse(JsonProperty json, JsonSerializerOptions options) {
			string s = Assembly.CreateQualifiedName(COMPONENT_ASSEMBLY_NAME, COMPONENT_NAMESPACE + json.Name);
			return Parse(Type.GetType(s), json.Value, options);
		}
			
		public static T Parse<T>(string json, JsonSerializerOptions options)
			=> (T)JsonSerializer.Deserialize(json, typeof(T), options);

		public static T Parse<T>(JsonElement json, JsonSerializerOptions options)
			=> (T)JsonSerializer.Deserialize(json.GetRawText(), typeof(T), options);
	}

}
