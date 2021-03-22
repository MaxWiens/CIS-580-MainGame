namespace ECS {
	public interface ISystem : IPriority {
		World World { get; }
	}
}
