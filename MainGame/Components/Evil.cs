using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Evil : Component {
		
		public Evil(Entity entity) : base(entity) { }

		[MessageHandler(1)]
		public bool OnDamage(Message message) {
			if(message.Content.TryGetValue("IsEvil", out object value) && (bool)value) {
				return false;
			}
			return true;
		}

		public override IComponent Clone(Entity entity) => new Evil(entity);
			
	}
}
