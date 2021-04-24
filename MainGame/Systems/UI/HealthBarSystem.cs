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
		private Texture2D _skull;
		private SpriteFont _font;

		private int _killCount = 0;
		private int _skeletonKillCount = 0;
		public void AddKill(bool isSkeleton) {
			_killCount++;
			if(isSkeleton)
				_skeletonKillCount++;
		}

		public void ResetCount() {
			_killCount = 0;
			_skeletonKillCount = 0;
		}

		bool CreatedBoss = false;

		public HealthBarSystem(World world, MainGame game) : base(world) {
			_game = game;
			_hearts = _game.Content.Load<Texture2D>("Textures/hearts");
			_font = _game.Content.Load<SpriteFont>("MainFont");
			_skull = _game.Content.Load<Texture2D>("Textures/skull");
			world.Reset += OnReset;
		}

		private void OnReset() {
			CreatedBoss = false;
		}

		public void Draw() {
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

				if(!CreatedBoss && _skeletonKillCount >= 1) {
					World.CloneEntityGroup("Assets/Prefabs/Entities/BigSkull.json");
					CreatedBoss = true;
				}
			}

			if(_killCount != 0) {
				foreach(Entity e in World.GetEntitiesWith<UI.KillCounter>().Keys) {
					Body b = e.GetComponent<Body>();
					string killstring = $"Kills: {_killCount}";
					var stringSize = _font.MeasureString(killstring);
					stringSize.Y = 0;
					_game.UISpriteBatch.DrawString(_font, killstring, b.Position - stringSize, Color.Crimson);
				}
			}
			

			foreach(Entity e in World.GetEntitiesWith<UI.SkullCounter>().Keys) {
				Body b = e.GetComponent<Body>();
				string killstring = $"x{_skeletonKillCount}";
				var stringSize = _font.MeasureString(killstring);
				stringSize.Y = 0;
				_game.UISpriteBatch.DrawString(_font, killstring, b.Position - stringSize, Color.Crimson);
				_game.UISpriteBatch.Draw(_skull, (b.Position - stringSize) + new Vector2(-_skull.Width, _skull.Height/4), Color.White);
			}
		}
	}
}
