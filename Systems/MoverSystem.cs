using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using ECS.S;
	using Components;
	using Util;
	public class MoverSystem : System, IUpdateable {
		public MoverSystem(World world) : base(world) { }

		public void Update(float deltaTime) {
			var moverMap = world.GetEntitiesWithComponent<Mover>();
			var rbMap = world.GetEntitiesWithComponent<Body>();
			var tileAnimationMap = world.GetEntitiesWithComponent<TileAnimation>();
			var spriteMap = world.GetEntitiesWithComponent<Sprite>();
			var eids = moverMap.Keys;
			foreach(var eid in eids) {
				Body rb = rbMap[eid];
				ref Mover mover = ref moverMap[eid];
				ref Sprite sprite = ref spriteMap[eid];
				ref TileAnimation tileAnimation = ref tileAnimationMap[eid];

				if(rb.LinearVelocity.LengthSquared() > 0.25f) {
					mover.Direction = Directions.None;
					if(rb.LinearVelocity.X > 0) {
						mover.Direction |= Directions.Right;
						sprite.SpriteEffect = SpriteEffects.None;
					} else {
						mover.Direction |= Directions.Left;
						sprite.SpriteEffect = SpriteEffects.FlipHorizontally;
					}

					if(rb.LinearVelocity.Y > 0) {
						mover.Direction |= Directions.Down;
						tileAnimation.Asset = mover.FrontWalkAnimation;
						//tileAnimation.FrameIdx = 0;
					} else {
						mover.Direction |= Directions.Up;
						tileAnimation.Asset = mover.BackWalkAnimation;
						//tileAnimation.FrameIdx = 0;
					}
				} else {
					if((mover.Direction & Directions.Up) != Directions.None) {
						tileAnimation.Asset = mover.Back;
						//tileAnimation.FrameIdx = 0;
					} else {
						tileAnimation.Asset = mover.Front;
						//tileAnimation.FrameIdx = 0;
					}

					if((mover.Direction & Directions.Left) != Directions.None)
						sprite.SpriteEffect = SpriteEffects.FlipHorizontally;
					else
						sprite.SpriteEffect = SpriteEffects.None;

				}

			}
		}

		public void Helper(ref TileAnimation ta, Assets.Asset asset) {
			ta.Asset = asset;
			ta.Timer = 0;
			ta.FrameIdx = 0;
		}
	}
}
