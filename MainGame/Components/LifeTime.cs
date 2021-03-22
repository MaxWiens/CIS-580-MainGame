using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class LifeTime : Component {
		[JsonInclude] public float Time;

		public LifeTime(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new LifeTime(entity) { Time = Time};
	}
}
