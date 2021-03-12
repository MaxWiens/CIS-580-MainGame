using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
namespace MainGame.Components {
	public struct Exploding {
		[JsonInclude] public float Timer;
		[JsonInclude] public bool ExplodesOnContact;
		[JsonInclude] public string ExplosionPrefabPath;
	}
}
