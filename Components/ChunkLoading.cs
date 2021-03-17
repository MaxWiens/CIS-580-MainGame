using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct ChunkLoading : IComponent {
		public object Clone() => this;
	}
}
