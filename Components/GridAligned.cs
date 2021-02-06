using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;
namespace MainGame.Components {
	public struct GridAligned {
		[JsonInclude] public Point GridPosition;
	}
}
