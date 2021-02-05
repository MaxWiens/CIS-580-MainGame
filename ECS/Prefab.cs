using System;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace MainGame.ECS {
	public class Prefab {
		public readonly string Json;
		
		public Prefab(string json) {
			Json = json;
		}

		//public IEnumerable<object> GetComponents() {
		//	JsonDocument document = JsonDocument.Parse(Json);
			
		//	document.Dispose();
		//	;
		//}
	}
}
