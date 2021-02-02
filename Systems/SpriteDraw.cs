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
			foreach(KeyValuePair<ulong,Component> kvp in entityComponents) {
				Position2D pc = world.GetComponent<Position2D>(kvp.Key);
				spriteBatch.Draw((kvp.Value as Sprite).Texture, pc.Position, Color.White);
			}
			spriteBatch.End();
		}
	}
}
