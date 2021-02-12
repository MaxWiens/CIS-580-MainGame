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
			//var spriteEntitites = world.GetEntitiesWithComponent<Sprite>();
			//var entitites = spriteEntitites.Keys;
			//foreach(var eid in entitites){
			//	ref Sprite s = ref spriteEntitites[eid];
			//	s.Texture = world.Content.Load<Texture2D>(@"Textures\"+s.TextureName);
			//}
		}

		public override void Draw() {
			Transform2D cam = world.GetComponent<Transform2D>(world.MainCamera);
			var spriteEntitites = world.GetEntitiesWithComponent<Sprite>();
			var eids = spriteEntitites.Keys;
			Sprite s;
			Vector2 camCenter = cam.Position - (world.Resolution.ToVector2() * 0.5f);
			foreach(var eid in eids) {
				s = spriteEntitites[eid];
				world.SpriteBatch.Draw(s.Texture, world.GetComponent<Transform2D>(eid).Position-s.Offset-camCenter, s.Albedo);
			}
		}
	}
}
