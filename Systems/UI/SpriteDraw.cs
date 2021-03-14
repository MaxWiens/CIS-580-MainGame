using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
namespace MainGame.Systems.UI {
	using ECS;
	using ECS.S;
	using Components;
	using UI = Components.UI;
	public class SpriteDraw : System, IDrawable {
		private MegaDungeonGame _game;
		public SpriteDraw(GameWorld world, MegaDungeonGame game) : base(world) {
			_game = game;
		}
		private UI.Button _fallbackButton = default;
		public void Draw() {
			//Body camBody = world.GetComponent<Body>(_game.MainCamera);
			var spriteMap = world.GetEntitiesWithComponent<UI.Sprite>();
			var transMap = world.GetEntitiesWithComponent<Body>();
			var eids = spriteMap.Keys;
			Color c;
			UI.Sprite s;
			Body body;
			//Vector2 camCenter = camBody.Position - (_game.Resolution.ToVector2() * 0.5f);
			
			foreach(var eid in eids) {
				s = spriteMap[eid];
				UI.Button b = world.TryGetComponent(eid, ref _fallbackButton, out bool isSuccessful);
				c = isSuccessful && b.IsPressed ? Color.LightGray : s.Albedo;
				
				
				body = transMap[eid];
				Point pos = (body.Position - s.Offset).ToPoint();
				Point scale = new Point((int)(s.SourceRectangle.Width * s.Scale.X), (int)(s.SourceRectangle.Height * s.Scale.Y));
				_game.SpriteBatch.Draw(
					texture: s.Texture, //Texture2D 
					destinationRectangle: new Rectangle(pos, scale),
					sourceRectangle: s.SourceRectangle,
					color: c, //s.Albedo,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: s.SpriteEffect,
					layerDepth: 1f
				);
			}
		}
	}
}
