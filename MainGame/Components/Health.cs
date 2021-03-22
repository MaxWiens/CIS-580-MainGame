using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Health : Component {
		[JsonInclude] public int Value = 1;
		[JsonInclude] public int Max = 1;

		public Health(Entity entity) : base(entity) { }

		[MessageHandler]
		public bool OnDamage(Message message) {
			Value -= (int)message.Content["Total"];
			if(Value <= 0) {
				Entity.SendMessage(new Message("OnDeath"));
				Entity.World.RemoveEntity(Entity);
			}
			return false;
		}

		[MessageHandler]
		public bool OnHeal(Message message) {
			Value += (int)message.Content["Total"];
			return true;
		}

		public override IComponent Clone(Entity entity) => new Health(entity) {
			Value = Value,
			Max = Max
		};
	}
}
