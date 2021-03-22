using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class TileAnimation : Component {
		[JsonInclude] public Assets.Asset Asset;
		[JsonInclude] public int FrameIdx;
		[JsonInclude] public float FrameDelay;
		public float Timer;
		public TileAnimation(Entity entity) : base(entity) {}
		
		public override IComponent Clone(Entity entity) => new TileAnimation(entity) {
			Asset = Asset,
			FrameIdx = FrameIdx,
			FrameDelay = FrameDelay,
			Timer = 0
		};
	}
}
