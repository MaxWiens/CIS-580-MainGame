using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct BlockPlacer : IComponent {
		public bool ShouldPlaceBlock;
		[JsonInclude] public string BlockPrefabPath;

		public object Clone() => new BlockPlacer() {
			ShouldPlaceBlock = ShouldPlaceBlock,
			BlockPrefabPath = (string)BlockPrefabPath.Clone()
		};
	}
}
