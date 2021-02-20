using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using Components;
	using Util;
	public class MoverSystem : UpdateSystem {
		public MoverSystem(ZaWarudo world) : base(world) { }

		public override void Update(float deltaTime) {
			var moverMap = world.GetEntitiesWithComponent<Mover>();
			var rbMap = world.GetEntitiesWithComponent<RigidBody>();
			var tileAnimationMap = world.GetEntitiesWithComponent<TileAnimation>();
			var spriteMap = world.GetEntitiesWithComponent<Sprite>();
			var eids = moverMap.Keys;
			foreach(var eid in eids) {
				RigidBody rb = rbMap[eid];
				ref Mover mover = ref moverMap[eid];
				ref Sprite sprite = ref spriteMap[eid];
				ref TileAnimation tileAnimation = ref tileAnimationMap[eid];

				if(rb.Velocity.LengthSquared() > 0.25f) {
					mover.Direction = Direction.None;
					if(rb.Velocity.X > 0) {
						mover.Direction |= Direction.Right;
						sprite.SpriteEffect = SpriteEffects.None;
					} else {
						mover.Direction |= Direction.Left;
						sprite.SpriteEffect = SpriteEffects.FlipHorizontally;
					}

					if(rb.Velocity.Y > 0) {
						mover.Direction |= Direction.Down;
						tileAnimation.Asset = mover.FrontWalkAnimation;
						//tileAnimation.FrameIdx = 0;
					} else {
						mover.Direction |= Direction.Up;
						tileAnimation.Asset = mover.BackWalkAnimation;
						//tileAnimation.FrameIdx = 0;
					}
				} else {
					if((mover.Direction & Direction.Up) != Direction.None) {
						tileAnimation.Asset = mover.Back;
						//tileAnimation.FrameIdx = 0;
					} else {
						tileAnimation.Asset = mover.Front;
						//tileAnimation.FrameIdx = 0;
					}

					if((mover.Direction & Direction.Left) != Direction.None)
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
