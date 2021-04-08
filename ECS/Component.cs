using System.Reflection;
namespace ECS {
	public abstract class Component : IComponent {
		private const BindingFlags MESSAGE_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
		private readonly Entity _entity;
		public Entity Entity => _entity;

		public Component(Entity entity) {
			_entity = entity;
		}
		
		public bool HandleMessage(Message message)
			=> (bool)GetType().InvokeMember(message.HandlerName, MESSAGE_FLAGS, null, this, new object[] { this, message });

		public abstract IComponent Clone(Entity entity);
		public IComponent Clone() => Clone(null);
	}
}
