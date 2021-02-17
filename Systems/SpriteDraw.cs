using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class SpriteDraw : DrawSystem {
		public SpriteDraw(ZaWarudo world) : base(world) { }

		public override void Draw() {
			Transform2D cam = world.GetComponent<Transform2D>(world.MainCamera);
			var spriteEntitites = world.GetEntitiesWithComponent<Sprite>();
			var transMap = world.GetEntitiesWithComponent<Transform2D>();
			var eids = spriteEntitites.Keys;
			Sprite s;
			Transform2D trans;
			Vector2 camCenter = cam.Position - (world.Resolution.ToVector2() * 0.5f);
			foreach(var eid in eids) {
				s = spriteEntitites[eid];
				trans = transMap[eid];
				Point pos = (trans.Position - s.Offset - camCenter).ToPoint();
				Vector2 vscale = trans.Scale*s.Scale;
				Point scale = new Point((int)(s.Texture.Width * vscale.X), (int)(s.Texture.Height * vscale.Y));
				world.SpriteBatch.Draw(
					texture: s.Texture, //Texture2D 
					destinationRectangle: new Rectangle(pos, scale), 
					sourceRectangle: null, // draw full texture
					color: s.Albedo, 
					rotation: 0f, 
					origin: Vector2.Zero, 
					effects: SpriteEffects.None, 
					layerDepth: 0f
				);

				//world.SpriteBatch.Draw(
				//	s.Texture, 
				//	world.GetComponent<Transform2D>(eid).Position-s.Offset-camCenter, 
				//	s.Albedo
				//);
			}
		}
	}
}
