using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public struct TileAnimation : IComponent{
		[JsonInclude] public Assets.Asset Asset;
		[JsonInclude] public int FrameIdx;
		[JsonInclude] public float FrameDelay;
		public float Timer;

		public object Clone() => this;
	}
}
