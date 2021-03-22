using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class DamageArea : Component {
		[JsonInclude] public int Damage;
		
		public DamageArea(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new DamageArea(entity) { Damage = Damage };
	}
}
