using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct GridElement : IComponent {
		public Point Position;

		public object Clone() => this;
	}
}
