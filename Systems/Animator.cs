using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class Animator : UpdateSystem {
		public Animator(ZaWarudo world) : base(world) { }

		public override void Update(float deltaTime) {
			var aniMap = world.GetEntitiesWithComponent<TileAnimation>();
			var spriteMap = world.GetEntitiesWithComponent<Sprite>();
			var eids = aniMap.Keys;
			foreach(var eid in eids) {
				ref TileAnimation animation = ref aniMap[eid];
				if((animation.Timer += deltaTime) >= animation.FrameDelay) {
					animation.Timer -= animation.FrameDelay;
					++animation.FrameIdx;
					ref Sprite sprite = ref spriteMap[eid];
					var asset = animation.Asset as Assets.TileSheet;
					sprite.Texture = asset.Texture;
					int x = asset.NumTiles.X;
					if(animation.FrameIdx >= x*asset.NumTiles.Y) {
						 animation.FrameIdx = 0;
					}
					sprite.SourceRectangle = new Rectangle(new Point(animation.FrameIdx % x, animation.FrameIdx / x)*asset.TileDimentions, asset.TileDimentions);
				}
			}
		}
	}
}
