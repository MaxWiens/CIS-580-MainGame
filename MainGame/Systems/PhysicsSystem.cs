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
	public class PhysicsSystem : BaseSystem, IFixedUpdateable {

		public readonly tainicom.Aether.Physics2D.Dynamics.World PhysicsWorld;

		private readonly Queue<Action> _todos = new Queue<Action>();

		public void AddTodo(Action todoAction) => _todos.Enqueue(todoAction);

		public PhysicsSystem(ECS.World world, tainicom.Aether.Physics2D.Dynamics.World physicsWorld) : base(world) {
			PhysicsWorld = physicsWorld;
			PhysicsWorld.Gravity = Vector2.Zero;
		}

		public void FixedUpdate(float fixedDeltaTime) {
			PhysicsWorld.Step(fixedDeltaTime);
			while(_todos.Count > 0) _todos.Dequeue()();
		}
	}
}
