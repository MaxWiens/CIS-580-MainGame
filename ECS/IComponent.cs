namespace ECS {
	public interface IComponent {
		Entity Entity { get; }
		
		IComponent Clone(Entity entity);
	}
}
