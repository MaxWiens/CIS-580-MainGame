using System.Collections.Generic;
namespace ECS {
	public class Scene {
		public readonly string Name;
		internal readonly List<Entity> entities = new List<Entity>();
		public Scene(string name) {
			Name = name;
		}
	}
}
