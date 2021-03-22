using Microsoft.Xna.Framework;
using System;
using MoonSharp.Interpreter;

namespace MainGame.Systems {
	using ECS;
	using Components;
	[MoonSharpUserData]
	public class Following : BaseSystem, IUpdateable {
		public Following(World world) : base(world) { }

		public void Update(float deltaTime) {
			Vector2 dif;
			Body thisBody;
			Body targetBody;
			foreach(Follower f in World.GetEntitiesWith<Follower>().Values) {
				thisBody = f.Entity.GetComponent<Body>();
				targetBody = World.GetEntity(f.Target).GetComponent<Body>();
				if(thisBody.Position != targetBody.Position) {
					dif = targetBody.Position - thisBody.Position;
					if(dif.Length() < f.SnapDistance) {
						thisBody.Position = targetBody.Position;
					} else {
						thisBody.Position += dif * Math.Clamp(f.Strength * deltaTime, 0f, 1f);
					}
				}
			}
		}
	}
}
