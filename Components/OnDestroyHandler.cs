using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class OnDestoryHandler : IComponent {
		[JsonInclude] public Script Script;
		public object Clone() => this;
	}
}
