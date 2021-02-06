using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
namespace MainGame.Components {
	public struct BlockPlacer {
		public bool ShouldPlaceBlock;
		[JsonInclude]
		public string BallPrefabPath;
	}
}
