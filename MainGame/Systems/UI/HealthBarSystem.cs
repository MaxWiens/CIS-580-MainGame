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
	public class HealthBarSystem : BaseSystem, IDrawable {
		private MainGame _game;
		private Texture2D _hearts;
		private SpriteFont _font;

		public int KillCount = 0;
		public HealthBarSystem(World world, MainGame game) : base(world) {
			_game = game;
			_hearts = _game.Content.Load<Texture2D>("Textures/hearts");
			_font = _game.Content.Load<SpriteFont>("MainFont");
		}

		public void Draw() {
			Body body;
			Entity player = World.GetEntity("PlayerCharacter");
			if(player != null) {
				Health playerHealth = player.GetComponent<Health>();
				foreach(Entity e in World.GetEntitiesWith<UI.HealthBar>().Keys) {
					Body b = e.GetComponent<Body>();
					for(int i = 0; i < playerHealth.Value; i++) {
						_game.UISpriteBatch.Draw(
							texture: _hearts,
							destinationRectangle: new Rectangle((int)b.Position.X + (i*13), (int)b.Position.Y, 12,12),
							sourceRectangle: new Rectangle(0,0,12,12),
							color: Color.White,
							rotation: 0f,
							origin: Vector2.Zero,
							effects: SpriteEffects.None,
							layerDepth: 1f
						);
					}
				}
			}

			foreach(Entity e in World.GetEntitiesWith<UI.KillCounter>().Keys) {
				Body b = e.GetComponent<Body>();
				string killstring = $"Kills: {KillCount}";
				var stringSize = _font.MeasureString(killstring);
				stringSize.Y = 0;
				_game.UISpriteBatch.DrawString(_font, killstring, b.Position - stringSize, Color.Crimson);




				//	texture: _hearts,
				//	destinationRectangle: new Rectangle((int)b.Position.X + (i * 13), (int)b.Position.Y, 12, 12),
				//	sourceRectangle: new Rectangle(0, 0, 12, 12),
				//	color: Color.White,
				//	rotation: 0f,
				//	origin: Vector2.Zero,
				//	effects: SpriteEffects.None,
				//	layerDepth: 1f
				//);
			}
		}
	}
}
