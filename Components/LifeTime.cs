using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct LifeTime : IComponent{
		public float Time;

		public object Clone() => this;
	}
}
