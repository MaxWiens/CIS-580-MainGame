using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct Grip : IComponent {
		public object Clone() => this;
	}
}
