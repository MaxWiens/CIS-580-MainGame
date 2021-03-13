using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;
namespace MainGame.Systems.UI {
	using ECS;
	using ECS.S;
	using Components;
	using UI = Components.UI;
	public class VolumeDraw : System, IDrawable {
		private MegaDungeonGame _game;
		private readonly SpriteFont _font;
		public VolumeDraw(World world, MegaDungeonGame game) : base(world) {
			_font = game.Content.Load<SpriteFont>("MainFont");
			_game = game;
		}
		public void Draw() {
			//Body camBody = world.GetComponent<Body>(_game.MainCamera);
			var volumeMap = world.GetEntitiesWithComponent<UI.Volume>();
			var transMap = world.GetEntitiesWithComponent<Body>();
			var eids = volumeMap.Keys;
			Body body;
			//Vector2 camCenter = camBody.Position - (_game.Resolution.ToVector2() * 0.5f);
			string s = MediaPlayer.Volume.ToString("p0");
			Vector2 offset = _font.MeasureString(s)/2;
			foreach(var eid in eids) {
				body = transMap[eid];
				_game.SpriteBatch.DrawString(_font, s, body.Position-offset, Color.White);
			}
		}
	}
}
