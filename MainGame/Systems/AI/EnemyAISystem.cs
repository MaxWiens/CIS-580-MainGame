using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using System;
namespace MainGame.Systems.AI {
	using ECS;
	using Components;
	using Components.AI;
	[MoonSharpUserData]
	public class EnemyAISystem : BaseSystem, IUpdateable {
		private readonly Random _r;
		public EnemyAISystem(World world) : base(world) {
			_r = new Random((int)DateTime.Now.Ticks);
		}

		public void Update(float deltaTime) {
			var enemyMap = World.GetEntitiesWith<BasicEnemyAI>();
			Entity player;
			Vector2 dif = Vector2.Zero;
			foreach(BasicEnemyAI enemy in enemyMap.Values) {
				Body body = enemy.Entity.GetComponent<Body>();

				if((player = World.GetEntity("PlayerCharacter")) != null && (dif = player.GetComponent<Body>().Position - body.Position).Length() <= enemy.Range) {
					enemy.State = EnemyState.Follow;
				} else {
					enemy.State = EnemyState.Wonder;
				}

				switch(enemy.State) {
					case EnemyState.Wonder:
						enemy.WonderTimer -= deltaTime;
						if(enemy.WonderTimer <= 0) {
							float newAngle = ((float)_r.NextDouble()) * MathHelper.TwoPi;
							enemy.WonderTimer = ((float)_r.NextDouble() * 2f) + 0.25f;
							enemy.WonderDirection = new Vector2(MathF.Cos(newAngle), MathF.Sin(newAngle));
						}
						body.LinearVelocity = 50f * enemy.WonderDirection;
						break;
					case EnemyState.Follow:
						enemy.WonderTimer = 0;
						var difNorm = Vector2.Normalize(dif);
						if(float.IsNaN(difNorm.X) || float.IsNaN(difNorm.Y)) break;
						body.LinearVelocity = difNorm * 80f;
						break;
				}
			}
		}
	}
}
