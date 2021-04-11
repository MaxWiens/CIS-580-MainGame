using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision;
using ECS;
using System;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class DamageArea : Component {
		[JsonInclude] public int Damage = 1;
		[JsonInclude] public bool DamageTiles;
		[JsonInclude] public bool IsEvil;

		public DamageArea(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new DamageArea(entity) { Damage = Damage, DamageTiles = DamageTiles, IsEvil = IsEvil};

		[MessageHandler]
		public bool OnCollision(Message message) {
			if(DamageTiles) {
				int i = 0;
				if(((Entity)((Fixture)message.Content["Other"]).Body.Tag).TryGetComponent(out Tile t)) {
					i = 0;
				}
			}
			

			return ((Entity)((Fixture)message.Content["Other"]).Body.Tag).SendMessage(new Message("OnDamage") { Content = { { "Total", Damage }, { "DamageTiles", DamageTiles }, {"IsEvil", IsEvil } } });
		}
	}
}
