using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
namespace MainGame.Components {
	public struct DamageArea {
		[JsonInclude] public int Damage;
	}
}
