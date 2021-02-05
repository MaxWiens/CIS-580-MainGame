using System.Text.Json.Serialization;

namespace MainGame.Components {
	public struct Health {
		[JsonInclude]
		public int Value;
	}
}
