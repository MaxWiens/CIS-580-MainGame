using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class ChunkLoading : Component {
		public ChunkLoading(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new ChunkLoading(Entity);
	}
}
