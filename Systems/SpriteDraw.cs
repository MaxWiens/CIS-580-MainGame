using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using ECS.S;
	using Components;
	public class SpriteDraw : System, IDrawable {
		private MegaDungeonGame _game;
		public SpriteDraw(World world, MegaDungeonGame game) : base(world) {
			_game = game;
		}

		public void Draw() {
			Transform2D cam = world.GetComponent<Transform2D>(_game.MainCamera);
			var spriteMap = world.GetEntitiesWithComponent<Sprite>();
			var transMap = world.GetEntitiesWithComponent<Transform2D>();
			var eids = spriteMap.Keys;
			Sprite s;
			Transform2D trans;
			Vector2 camCenter = cam.Position - (_game.Resolution.ToVector2() * 0.5f);
			foreach(var eid in eids) {
				s = spriteMap[eid];
				trans = transMap[eid];
				Point pos = (trans.Position - s.Offset - camCenter).ToPoint();
				Point scale = new Point((int)(s.SourceRectangle.Width * s.Scale.X), (int)(s.SourceRectangle.Height * s.Scale.Y));
				_game.SpriteBatch.Draw(
					texture: s.Texture, //Texture2D 
					destinationRectangle: new Rectangle(pos, scale),
					sourceRectangle: s.SourceRectangle,
					color: s.Albedo,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: s.SpriteEffect,
					layerDepth: (float)(pos.Y+s.SourceRectangle.Height % _game.Resolution.Y) / _game.Resolution.Y
				);
			}
		}
	}
}
