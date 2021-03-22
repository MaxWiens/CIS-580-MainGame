namespace ECS {
	public abstract class BaseSystem : ISystem {
		private readonly World _world;
		public World World => _world;
		private readonly uint _priority;
		public virtual uint Priority => uint.MaxValue;
		public BaseSystem(World world) {
			_world = world;
		}
	}
}
