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
