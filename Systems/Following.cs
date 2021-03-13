using Microsoft.Xna.Framework;
using System;

namespace MainGame.Systems {
	using ECS;
	using ECS.S;
	using Components;
	public class Following : System, IUpdateable {
		public Following(World world) : base(world) { }

		public void Update(float deltaTime) {
			var followerMap = world.GetEntitiesWithComponent<Follower>();
			var bodyMap = world.GetEntitiesWithComponent<Body>();
			var eids = followerMap.Keys;
			Vector2 dif;
			foreach(var eid in eids) {
				ref Follower f = ref followerMap[eid];
				ref Body body = ref bodyMap[eid];
				Body target = bodyMap[f.Target];
				if(body.Position != target.Position) {
					dif = target.Position - body.Position;
					if(dif.Length() < f.SnapDistance) {
						body.Position = target.Position;
					} else {
						body.Position += dif * Math.Clamp(f.Strength * deltaTime, 0f, 1f);
					}
				}
			}
		}
	}
}
