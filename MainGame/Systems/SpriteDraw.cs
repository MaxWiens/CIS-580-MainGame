using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;

namespace MainGame.Systems {
	using ECS;
	using Components;
	[MoonSharpUserData]
	public class SpriteDraw : BaseSystem, IDrawable {
		private readonly MainGame _game;
		public SpriteDraw(World world, MainGame game) : base(world) {
			_game = game;
		}

		public void Draw() {
			Body body;
			Vector2 camCenter = _game.MainCamera.Center;
			foreach(Sprite s in World.GetEntitiesWith<Sprite>().Values) {
				body = s.Entity.GetComponent<Body>();
				Point pos = (body.Position - s.Offset - camCenter).ToPoint();
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
