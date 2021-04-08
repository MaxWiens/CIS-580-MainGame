using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class BlockPlacer : Component {
		public bool ShouldPlaceBlock;
		[JsonInclude] public string BlockPrefabPath;

		public BlockPlacer(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new BlockPlacer(entity) {
			ShouldPlaceBlock = ShouldPlaceBlock,
			BlockPrefabPath = (string)BlockPrefabPath.Clone()
		};
	}
}
