using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;
namespace MainGame.Systems {
	using ECS;
	using Components;
	[MoonSharpUserData]
	public class PositionDraw : BaseSystem, IDrawable {
		private readonly Texture2D _pixelTexture;
		private readonly MainGame _game;
		public override uint Priority => (uint)DefaultPriority.VeryImportant; 
		public PositionDraw(World world, MainGame game) : base(world) {
			_game = game;
			_pixelTexture = game.Content.Load<Texture2D>(@"Textures\pixel");
		}

		public void Draw() {
			Vector2 camCenter = _game.MainCamera.Center;
			foreach(Body body in World.GetEntitiesWith<Body>().Values) {
				_game.SpriteBatch.Draw(
					_pixelTexture, 
					new Rectangle((body.Position - camCenter).ToPoint(), new Point(1,1)), 
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
