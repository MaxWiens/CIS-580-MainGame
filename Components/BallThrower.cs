using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
namespace MainGame.Components {
	public struct BallThrower {
		public bool ShouldThrowBall;
		[JsonInclude]
		public string BallPrefab;
	}
}
