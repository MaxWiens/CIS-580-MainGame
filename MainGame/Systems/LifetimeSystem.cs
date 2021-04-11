using Microsoft.Xna.Framework;
using System;
using MoonSharp.Interpreter;
using System.Collections.Generic;
namespace MainGame.Systems {
	using ECS;
	using Components;
	public class LifetimeSystem : BaseSystem , IUpdateable {
		public LifetimeSystem(World world) : base(world) { }
		List<Entity> entitiesToRemove = new List<Entity>();
		public void Update(float deltaTime) {
			entitiesToRemove.Clear();
			foreach(LifeTime t in World.GetEntitiesWith<LifeTime>().Values) {
				t.Time -= deltaTime;
				if(t.Time <= 0f) {
					entitiesToRemove.Add(t.Entity);
				}
			}
			foreach(Entity e in entitiesToRemove) {
				if(e.TryGetComponent(out Health h)) {
					h.Value = 0;
				}
				e.SendMessage(new Message("OnDeath"));
				World.RemoveEntity(e);
			}
		}
	}
}
