using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using System;
namespace MainGame.Systems.AI {
	using ECS;
	using Components;
	using Components.AI;
	[MoonSharpUserData]
	public class BossAISystem : BaseSystem, IUpdateable {
		private readonly Random _r;
		public BossAISystem(World world) : base(world) {
			_r = new Random((int)DateTime.Now.Ticks);
		}
		Vector2 prev;
		public void Update(float deltaTime) {
			var enemyMap = World.GetEntitiesWith<Boss>();
			Entity player;
			Vector2 dif;
			if(deltaTime != 0) {
				foreach(Boss enemy in enemyMap.Values) {
					Body body = enemy.Entity.GetComponent<Body>();

					player = World.GetEntity("PlayerCharacter");

					enemy.GrowlTimer -= deltaTime;
					if(enemy.GrowlTimer <= 0) {
						enemy.GrowlTimer += Util.Rand.Float()*3f+3f;
					}
					dif = player.GetComponent<Body>().Position - body.Position;
					var difNorm = Vector2.Normalize((Vector2.Normalize(dif)+ (prev*2.5f))/2f);
					if(float.IsNaN(difNorm.X) || float.IsNaN(difNorm.Y)) continue;
					body.LinearVelocity = difNorm*100f;
					prev = difNorm;
				}
			}
		}
	}
}
