using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;

namespace MainGame.Systems {
	using ECS;
	using Components;
	using Util;
	[MoonSharpUserData]
	public class MoverSystem : BaseSystem, IUpdateable {
		public MoverSystem(World world) : base(world) { }

		public void Update(float deltaTime) {
			if(deltaTime != 0f) {
				Mover mover;
				Body body;
				Sprite sprite;
				TileAnimation tileAnimation;
				foreach(Entity entity in World.GetEntitiesWith<Mover>().Keys) {
					mover = entity.GetComponent<Mover>();
					sprite = entity.GetComponent<Sprite>();
					body = entity.GetComponent<Body>();
					tileAnimation = entity.GetComponent<TileAnimation>();

					if(body.LinearVelocity.LengthSquared() > 0.25f) {
						mover.Direction = Directions.None;
						if(body.LinearVelocity.X > 0) {
							mover.Direction |= Directions.Right;
							sprite.SpriteEffect = SpriteEffects.None;
						} else {
							mover.Direction |= Directions.Left;
							sprite.SpriteEffect = SpriteEffects.FlipHorizontally;
						}

						if(body.LinearVelocity.Y > 0) {
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
		}
	}
}
