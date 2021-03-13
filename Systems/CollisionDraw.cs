using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
#if false
namespace MainGame.Systems {
	using ECS;
	using ECS.S;
	using Components;
	using Collections;
	public class CollisionDraw : System, IDrawable {
		private readonly MegaDungeonGame _game;
		public CollisionDraw(World world, MegaDungeonGame game) : base(world) {
			_game = game;
			_pixelTexture = game.Content.Load<Texture2D>(@"Textures\pixel");
			_circleTexture = game.Content.Load<Texture2D>(@"Textures\circle");
		}
		private Texture2D _pixelTexture;
		private Texture2D _circleTexture;

		private RectBounds _fallbackRectBounds;
		private CircleBounds _fallbackCircleBounds;
		public void Draw() {
			var rbMap = world.GetEntitiesWithComponent<RigidBody>();
			var sbMap = world.GetEntitiesWithComponent<StaticBody>();
			var rectMap = world.GetEntitiesWithComponent<RectBounds>();
			var circMap = world.GetEntitiesWithComponent<CircleBounds>();
			var transMap = world.GetEntitiesWithComponent<Body>();
			Transform2D cam = world.GetComponent<Transform2D>(_game.MainCamera);
			Color c;
			Vector2 camCenter = (cam.Position - (_game.Resolution.ToVector2() * 0.5f));
			if(rbMap != null) {
				foreach(Guid eid in rbMap.Keys) {
					RigidBody rb = rbMap[eid];
					if(rb.Collisions != null && rb.Collisions.Count > 0)
						c = new Color(255, 100, 100, 150);
					else
						c = new Color(30, 30, 30, 2);
					Helper(eid, camCenter, c, rectMap, transMap, circMap);
				}
			}

			if(sbMap != null) {
				foreach(Guid eid in sbMap.Keys) {
					StaticBody sb = sbMap[eid];
					if(sb.Collisions != null && sb.Collisions.Count > 0)
						c = new Color(255, 100, 100, 150);
					else
						c = new Color(30, 30, 30, 2);
					Helper(eid, camCenter, c, rectMap, transMap, circMap);
				}
			}
		}

		private void Helper(Guid eid, Vector2 camCenter, Color c, IRefMap<Guid,RectBounds> rectMap, IRefMap<Guid, Transform2D> transMap, IRefMap<Guid, CircleBounds> circMap) {
			bool isSuccessful;
			RectBounds rect = rectMap.TryGetValue(eid, ref _fallbackRectBounds, out isSuccessful);
			if(isSuccessful) {
				Transform2D trans = transMap[eid];
				Vector2 adjustedpos = trans.Position - rect.Offset;
				_game.SpriteBatch.Draw(
					_pixelTexture,
					new Rectangle((adjustedpos - camCenter).ToPoint(),
					rect.Dimentions.ToPoint()),
					new Rectangle(0, 0, 1, 1), c,
					0f,
					Vector2.Zero,
					SpriteEffects.None,
					1f
				);
				return;
			}
			CircleBounds circ = circMap.TryGetValue(eid, ref _fallbackCircleBounds, out isSuccessful);
			if(isSuccessful) {
				Transform2D trans = transMap[eid];
				Vector2 adjustedpos = trans.Position - circ.Offset - camCenter;
				int scale = (int)(circ.Radius * 2f);
				_game.SpriteBatch.Draw(
					texture: _circleTexture,
					destinationRectangle: new Rectangle(adjustedpos.ToPoint(), new Point(scale, scale)),
					sourceRectangle: null,
					color: c,
					rotation: 0,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f
				);
				return;
			}
		}
	}
}
#endif