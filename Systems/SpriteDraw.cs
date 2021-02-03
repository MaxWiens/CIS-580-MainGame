using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class SpriteDraw : DrawSystem {
		SpriteBatch spriteBatch;
		public SpriteDraw(World world) : base(world) { }

		private void OnInit() {
			spriteBatch = new SpriteBatch(world.GraphicsDevice);
		}
		private void OnContentLoad() {
			var entityComponents = world.GetEntitiesWithComponent<Sprite>();
			Sprite sprite;
			foreach(var comp in entityComponents.Values) {
				sprite = (comp as Sprite);
				sprite.Texture = world.Content.Load<Texture2D>(sprite.TextureName);
			}
		}

		public override void Draw() {
			spriteBatch.Begin();
			var entityComponents = world.GetEntitiesWithComponent<Sprite>();
			Position2D pos;
			Color colorComponent;
			Microsoft.Xna.Framework.Color color;
			foreach(KeyValuePair<ulong,Component> kvp in entityComponents) {
				pos = world.GetComponent<Position2D>(kvp.Key);
				;
				color = (colorComponent = world.GetComponent<Color>(kvp.Key)) != null ? colorComponent.Value : Microsoft.Xna.Framework.Color.White;
				spriteBatch.Draw((kvp.Value as Sprite).Texture, pos.Position, color);
			}
			spriteBatch.End();
		}
	}
}
