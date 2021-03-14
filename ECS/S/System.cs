namespace ECS.S {
	public abstract class System {
		protected readonly GameWorld world;
	
		public System(GameWorld world) {
			this.world = world;
		}
	}
}
