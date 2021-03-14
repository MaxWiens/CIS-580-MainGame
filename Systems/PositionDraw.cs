using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using ECS.S;
	using Components;
	public class PositionDraw : System, IDrawable {
		private readonly Texture2D _pixelTexture;
		private readonly MegaDungeonGame _game;
		public PositionDraw(GameWorld world, MegaDungeonGame game) : base(world) {
			_game = game;
			_pixelTexture = game.Content.Load<Texture2D>(@"Textures\pixel");
		}

		public void Draw() {
			if(!world.TryGetEID("MainCamera", out Guid cameraEid))
				return;
			Body camBody = world.GetComponent<Body>(cameraEid);
			var entitites = world.GetEntitiesWithComponent<Body>();
			if(entitites != null) {
				var eids = entitites.Keys;
				Body body;
				foreach(var eid in eids) {
					body = entitites[eid];
					_game.SpriteBatch.Draw(
						_pixelTexture, 
						new Rectangle((body.Position - (camBody.Position - (_game.Resolution.ToVector2() * 0.5f))).ToPoint(), new Point(1,1)), 
						new Rectangle(0, 0, 1, 1), 
						Color.White,
						0f,
						Vector2.Zero,
						SpriteEffects.None,
						1f);
				}
			}
		}
	}
}
