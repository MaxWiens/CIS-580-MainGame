using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;
namespace MainGame.Systems.UI {
	using ECS;
	using Components;
	using UI = Components.UI;
	[MoonSharpUserData]
	public class SpriteDraw : BaseSystem, IDrawable {
		private MainGame _game;
		public SpriteDraw(World world, MainGame game) : base(world) {
			_game = game;
		}
		public void Draw() {
			Body body;
			foreach(UI.Sprite s in World.GetEntitiesWith<UI.Sprite>().Values) {
				body = s.Entity.GetComponent<Body>();
				Point pos = (body.Position - s.Offset).ToPoint();
				Point scale = new Point((int)(s.SourceRectangle.Width * s.Scale.X), (int)(s.SourceRectangle.Height * s.Scale.Y));
				_game.UISpriteBatch.Draw(
					texture: s.Texture,
					destinationRectangle: new Rectangle(pos, scale),
					sourceRectangle: s.SourceRectangle,
					color: s.Albedo,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: s.SpriteEffect,
					layerDepth: 1f
				);
			}
		}
	}
}
