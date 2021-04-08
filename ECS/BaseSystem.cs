namespace ECS {
	public abstract class BaseSystem : ISystem {
		private readonly World _world;
		public World World => _world;
		public BaseSystem(World world) {
			_world = world;
		}
	}
}
