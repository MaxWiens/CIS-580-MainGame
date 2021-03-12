using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;

namespace MainGame.Systems {
	using ECS.S;
	class Physics : System, IUpdateable {
		private readonly World _physicsWorld;
		public Physics(ECS.World world, World physicsWorld) : base(world) {

			_physicsWorld = physicsWorld;
			_physicsWorld.Gravity = Vector2.Zero;
		}

		public void Update(float deltaTime) {
			_physicsWorld.Step(deltaTime);
		}
	}
}
