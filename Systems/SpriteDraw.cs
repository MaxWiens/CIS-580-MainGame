using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class SpriteDraw : DrawSystem {
		public SpriteDraw(ZaWarudo world) : base(world) { }

		private void OnInit() {

		}

		private void OnContentLoad() {
			var spriteEntitites = world.GetEntitiesWithComponent<Sprite>();
			var entitites = spriteEntitites.Keys;
			foreach(var eid in entitites){
				ref Sprite s = ref spriteEntitites[eid];
				s.Texture = world.Content.Load<Texture2D>(@"Textures\"+s.TextureName);
			}
		}

		public override void Draw() {
			var spriteEntitites = world.GetEntitiesWithComponent<Sprite>();
			var eids = spriteEntitites.Keys;
			Sprite s2;
			foreach(var eid in eids) {
				ref Sprite s = ref spriteEntitites[eid];
				s2 = spriteEntitites[eid];
				world.SpriteBatch.Draw(s.Texture, world.GetComponent<Transform2D>(eid).Position, s.Albedo);
			}
		}
	}
}
