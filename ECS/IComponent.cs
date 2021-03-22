namespace ECS {
	public interface IComponent : IPriority {
		Entity Entity { get; }
		
		IComponent Clone(Entity entity);
	}
}
