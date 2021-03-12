namespace ECS.S {
	public abstract class System {
		protected readonly World world;
	
		public System(World world) {
			this.world = world;
		}
	}
}
