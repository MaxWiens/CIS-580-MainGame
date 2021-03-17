using ECS;
using MoonSharp.Interpreter;
namespace MainGame.Components.UI {
	[MoonSharpUserData]
	public struct Volume : IComponent {
		public object Clone() => this;
	}
}
