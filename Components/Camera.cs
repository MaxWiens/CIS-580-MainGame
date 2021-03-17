using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct Camera : IComponent {
		public object Clone() => this;
	}
}
