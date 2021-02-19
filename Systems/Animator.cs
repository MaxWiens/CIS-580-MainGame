using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class Animator : UpdateSystem {
		public Animator(ZaWarudo world) : base(world) { }

		public override void Update(float deltaTime) {
			var aniMap = world.GetEntitiesWithComponent<TileAnimation>();
			var eids = aniMap.Keys;
			TileAnimation animation;
			foreach(var eid in eids) {
				animation = aniMap[eid];
				animation.Timer += deltaTime;
				//if(animation.)
			}
		}
	}
}
