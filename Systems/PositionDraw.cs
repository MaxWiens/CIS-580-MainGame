using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class PositionDraw : DrawSystem {
		SpriteBatch spriteBatch;
		public PositionDraw(ZaWarudo world) : base(world) { }
		private Texture2D _pixelTexture;
		private void OnInit() {
			spriteBatch = new SpriteBatch(world.GraphicsDevice);
		}
		private void OnContentLoad() {
			_pixelTexture = world.Content.Load<Texture2D>(@"Textures\pixel");
		}

		public override void Draw() {
			Transform2D cam = world.GetComponent<Transform2D>(world.MainCamera);
			var entitites = world.GetEntitiesWithComponent<Transform2D>();
			if(entitites != null) {
				var eids = entitites.Keys;
				Transform2D pos;
				foreach(var eid in eids) {
					pos = entitites[eid];
					world.SpriteBatch.Draw(
						_pixelTexture, 
						new Rectangle((pos.Position - (cam.Position - (world.Resolution.ToVector2() * 0.5f))).ToPoint(), new Point(1,1)), 
						new Rectangle(0, 0, 1, 1), 
						Color.White);
				}
			}
		}
	}
}
