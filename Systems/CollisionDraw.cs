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
		private void OnInit() {
			spriteBatch = new SpriteBatch(world.GraphicsDevice);
		}
		private void OnContentLoad() {
			_pixelTexture = world.Content.Load<Texture2D>(@"Textures\pixel");
		}

		public override void Draw() {
			var entitites = world.GetEntitiesWithComponent<RectCollider>();
			var eids = entitites.Keys;
			foreach(var eid in eids) {
				ref RectCollider s = ref entitites[eid];
				Vector2 pos = world.GetComponent<Transform2D>(eid).Position+s.Offset;
				Color c;
				if(s.collisions != null && s.collisions.Count > 0){
					c = new Color(255, 100, 100, 150);
				} else {
					c = new Color(30, 30, 30, 2);
				}
				world.SpriteBatch.Draw(_pixelTexture, new Rectangle(pos.ToPoint(), s.Dimentions.ToPoint()), new Rectangle(0,0,1,1), c);
			}
		}
	}
}
