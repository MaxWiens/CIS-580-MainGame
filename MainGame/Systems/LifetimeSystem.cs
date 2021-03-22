using Microsoft.Xna.Framework;
using System;
using MoonSharp.Interpreter;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class LifetimeSystem : BaseSystem , IUpdateable {
		public LifetimeSystem(World world) : base(world) { }

		public void Update(float deltaTime) {
			foreach(LifeTime t in World.GetEntitiesWith<LifeTime>().Values) {
				t.Time -= deltaTime;
				if(t.Time <= 0f) {
					World.RemoveEntity(t.Entity);
				}
			}
		}
	}
}
