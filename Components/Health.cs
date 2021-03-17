﻿using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	public struct Health : IComponent {
		[JsonInclude] public int Value;

		public object Clone() => this;
	}
}
