using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct DamageArea : IComponent {
		[JsonInclude] public int Damage;

		public object Clone() => this;
	}
}
