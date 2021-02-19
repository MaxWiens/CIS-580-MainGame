using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class Following : UpdateSystem {
		public Following(ZaWarudo world) : base(world) { }

		public override void Update(float deltaTime) {
			var followerMap = world.GetEntitiesWithComponent<Follower>();
			var transMap = world.GetEntitiesWithComponent<Transform2D>();
			var eids = followerMap.Keys;
			Vector2 dif;
			foreach(var eid in eids) {
				ref Follower f = ref followerMap[eid];
				ref Transform2D t = ref world.GetComponent<Transform2D>(eid);
				Transform2D target = world.GetComponent<Transform2D>(f.Target);
				if(t.Position != target.Position) {
					dif = target.Position - t.Position;
					if(dif.Length() < f.SnapDistance) {
						t.Position = target.Position;
					} else {
						t.Position += dif * Math.Clamp(f.Strength * deltaTime, 0f, 1f);
					}
				}
			}
		}
	}
}
