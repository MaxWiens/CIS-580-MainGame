using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct Drops : IComponent {
		[JsonInclude] public string[] Items;

		public object Clone() {
			string[] items = new string[Items.Length];
			Items.CopyTo(items, 0);
			return new Drops() {
				Items = items
			};
		} 
	}
}
