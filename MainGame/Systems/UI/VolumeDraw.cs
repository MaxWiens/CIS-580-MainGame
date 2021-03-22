using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MoonSharp.Interpreter;
namespace MainGame.Systems.UI {
	using ECS;
	using Components;
	using UI = Components.UI;
	[MoonSharpUserData]
	public class VolumeDraw : BaseSystem, IDrawable {
		private readonly MainGame _game;
		private readonly SpriteFont _font;
		public VolumeDraw(World world, MainGame game) : base(world) {
			_font = game.Content.Load<SpriteFont>("MainFont");
			_game = game;
		}
		public void Draw() {
			Body body;
			string s = MediaPlayer.Volume.ToString("p0");
			Vector2 offset = _font.MeasureString(s)/2;
			foreach(UI.Volume volume in World.GetEntitiesWith<UI.Volume>().Values){
				body = volume.Entity.GetComponent<Body>();
				_game.SpriteBatch.DrawString(_font, s, body.Position-offset, Color.White);
			}
		}
	}
}
