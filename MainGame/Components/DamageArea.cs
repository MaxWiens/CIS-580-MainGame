using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using tainicom.Aether.Physics2D.Dynamics;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class DamageArea : Component {
		[JsonInclude] public int Damage = 1;
		
		public DamageArea(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new DamageArea(entity) { Damage = Damage };

		[MessageHandler]
		public bool OnCollision(Message message) {
			return ((Entity)((Fixture)message.Content["Other"]).Body.Tag).SendMessage(new Message("OnDamage") { Content = { {"Total", Damage} } });
		}
	}
}
