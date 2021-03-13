using MoonSharp.Interpreter;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace MainGame.Components.UI {
	public struct Button {
		public bool IsPressed;
		[JsonInclude] public Script ClickEvent;
	}
}
