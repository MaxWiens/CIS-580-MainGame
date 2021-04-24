using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using tainicom.Aether.Physics2D.Dynamics;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class PlayerDamageArea : Component {
		[JsonInclude] public int Damage = 1;
		
		public PlayerDamageArea(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new PlayerDamageArea(entity) { Damage = Damage };

		[MessageHandler]
		public bool OnCollision(Message message) {
			Entity e = ((Entity)((Fixture)message.Content["Other"]).Body.Tag);
			if(e.TryGetComponent(out Components.AI.BasicEnemyAI _) || e.TryGetComponent(out Components.AI.Boss _)) {
				e.SendMessage(new Message("OnDamage") { Content = { { "Total", Damage } } });
			}
			return true;
		}
	}
}
