using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Systems {
	[MoonSharpUserData]
	class PhysicsSystem : BaseSystem, IFixedUpdateable {
		public override uint Priority => 0;

		public readonly tainicom.Aether.Physics2D.Dynamics.World PhysicsWorld;
		public PhysicsSystem(ECS.World world, tainicom.Aether.Physics2D.Dynamics.World physicsWorld) : base(world) {
			PhysicsWorld = physicsWorld;
			PhysicsWorld.Gravity = Vector2.Zero;
		}

		public void FixedUpdate(float fixedDeltaTime) {
			PhysicsWorld.Step(fixedDeltaTime);
		}
	}
}
