using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct Exploding : IComponent {
		[JsonInclude] public float Timer;
		[JsonInclude] public bool ExplodesOnContact;
		[JsonInclude] public string ExplosionPrefabPath;

		public object Clone() => this;
	}
}
