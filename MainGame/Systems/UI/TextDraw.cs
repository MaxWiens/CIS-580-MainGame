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
	public class TextDraw : BaseSystem, IDrawable {
		private MainGame _game;
		public TextDraw(World world, MainGame game) : base(world) {
			_game = game;
		}
		public void Draw() {
			Body body;
			foreach(UI.TextBox tb in World.GetEntitiesWith<UI.TextBox>().Values) {
				body = tb.Entity.GetComponent<Body>();
				//Point pos = (body.Position - tb.Offset).ToPoint();
				
				_game.SpriteBatch.DrawString(
					tb.Font,
					tb.Text,
					tb.IsCentered ? body.Position - tb.Font.MeasureString(tb.Text) : body.Position,
					tb.Color,
					0,
					Vector2.Zero,
					tb.Scale,
					SpriteEffects.None,
					layerDepth: 1f
				);
			}
		}
	}
}
