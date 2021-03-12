using System;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECS {
	public class Prefab {
		private static Dictionary<string, Prefab> _prefabs = new Dictionary<string, Prefab>();

		public readonly string Json;
		
		//public static Prefab LoadPrefab(string filename) {
		//	public Prefab(string json) {
		//		Json = json;
		//	}
		//}

		

		//public IEnumerable<object> GetComponents() {
		//	JsonDocument document = JsonDocument.Parse(Json);
			
		//	document.Dispose();
		//	;
		//}
	}
}
