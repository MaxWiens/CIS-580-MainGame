using MoonSharp.Interpreter;
using System.Text.Json.Serialization;
using ECS;
namespace MainGame.Components.UI {
	[MoonSharpUserData]
	public struct Button : IComponent {
		public bool IsPressed;
		[JsonInclude] public Script ClickEvent;

		public object Clone() => this;
	}
}
