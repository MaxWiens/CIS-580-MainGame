using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class CollisionDraw : DrawSystem {
		SpriteBatch spriteBatch;
		public CollisionDraw(ZaWarudo world) : base(world) { }
		private Texture2D _pixelTexture;
		private Texture2D _circleTexture;
		private void OnInit() {
			spriteBatch = new SpriteBatch(world.GraphicsDevice);
		}
		private void OnContentLoad() {
			_pixelTexture = world.Content.Load<Texture2D>(@"Textures\pixel");
			_circleTexture = world.Content.Load<Texture2D>(@"Textures\circle");
		}

		private RectBounds _fallbackRectBounds;
		private CircleBounds _fallbackCircleBounds;
		public override void Draw() {
			bool isSuccessful;
			var rbMap = world.GetEntitiesWithComponent<RigidBody>();
			var sbMap = world.GetEntitiesWithComponent<StaticBody>();
			var rectMap = world.GetEntitiesWithComponent<RectBounds>();
			var circMap = world.GetEntitiesWithComponent<CircleBounds>();
			var transMap = world.GetEntitiesWithComponent<Transform2D>();
			Transform2D cam = world.GetComponent<Transform2D>(world.MainCamera);
			Color c;
			Vector2 camCenter = (cam.Position - (world.Resolution.ToVector2() * 0.5f));
			if(rbMap != null) {
				foreach(Guid eid in rbMap.Keys) {
					RigidBody rb = rbMap[eid];
				
					if(rb.Collisions != null && rb.Collisions.Count > 0)
						c = new Color(255, 100, 100, 150);
					else
						c = new Color(30, 30, 30, 2);

					RectBounds rect = rectMap.TryGetValue(eid, ref _fallbackRectBounds, out isSuccessful);
					if(isSuccessful) {
						Vector2 adjustedpos = transMap[eid].Position - rect.Offset;
						world.SpriteBatch.Draw(_pixelTexture, new Rectangle((adjustedpos - camCenter).ToPoint(), rect.Dimentions.ToPoint()), new Rectangle(0, 0, 1, 1), c);
						continue;
					}
					CircleBounds circ = circMap.TryGetValue(eid, ref _fallbackCircleBounds, out isSuccessful);
					if(isSuccessful) {
						Transform2D trans  = transMap[eid];
						Vector2 adjustedpos = trans.Position - circ.Offset - camCenter;

						Vector2 vscale = trans.Scale * circ.Radius * 2;
						Point scale = new Point((int)(vscale.X), (int)(vscale.Y));
						world.SpriteBatch.Draw(
							texture: _circleTexture,
							destinationRectangle: new Rectangle(adjustedpos.ToPoint(), scale),
							sourceRectangle: null,
							color: c
						);
						continue;
					}
				}
			}

			if(sbMap != null) {
				foreach(Guid eid in sbMap.Keys) {
					StaticBody sb = sbMap[eid];

					if(sb.Collisions != null && sb.Collisions.Count > 0)
						c = new Color(255, 100, 100, 150);
					else
						c = new Color(30, 30, 30, 2);

					RectBounds rect = rectMap.TryGetValue(eid, ref _fallbackRectBounds, out isSuccessful);
					if(isSuccessful) {
						Vector2 adjustedpos = world.GetComponent<Transform2D>(eid).Position - rect.Offset;
						world.SpriteBatch.Draw(_pixelTexture, new Rectangle((adjustedpos - camCenter).ToPoint(), rect.Dimentions.ToPoint()), new Rectangle(0, 0, 1, 1), c);
						continue;
					}
					CircleBounds circ = circMap.TryGetValue(eid, ref _fallbackCircleBounds, out isSuccessful);
					if(isSuccessful) {
						continue;
					}
				}
			}

			/*
			var rectMap = world.GetEntitiesWithComponent<RectBounds>();
			var circleMap = world.GetEntitiesWithComponent<RectBounds>();
			var rbMap = world.GetEntitiesWithComponent<RigidBody>();
			Transform2D cam = world.GetComponent<Transform2D>(world.MainCamera);
			var entitites = world.GetEntitiesWithComponent<RectBounds>();
			var eids = entitites.Keys;
			Vector2 camCenter = (cam.Position - (world.Resolution.ToVector2() * 0.5f));
			foreach(var eid in eids) {
				ref RectBounds s = ref entitites[eid];
				Vector2 adjustedpos = world.GetComponent<Transform2D>(eid).Position-s.Offset;
				Color c;
				if(s.collisions != null && s.collisions.Count > 0){
					c = new Color(255, 100, 100, 150);
				} else {
					c = new Color(30, 30, 30, 2);
				}

				world.SpriteBatch.Draw(_pixelTexture, new Rectangle((adjustedpos - camCenter).ToPoint(), s.Dimentions.ToPoint()), new Rectangle(0,0,1,1), c);
			}
			*/

		}
	}
}
