﻿using ECS;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision;
using MoonSharp.Interpreter;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using System;

namespace MainGame.Components {
	[MoonSharpUserData]
	public class Body : tainicom.Aether.Physics2D.Dynamics.Body, IComponent {
		public uint Priority => (uint)DefaultPriority.Physics;
		
		private readonly Entity _entity;
		public Entity Entity => _entity;

		public Body(Entity entity) {
			_entity = entity;
		}

		[MessageHandler]
		public bool OnEnable(Message _) {
			OnCollision += SendCollisionMessage;
			Enabled = true;
			if(World == null) {
				Entity.World.GetSystem<Systems.PhysicsSystem>().PhysicsWorld.Add(this);
			}
			if(FixtureList.Count > 0) {
				FixtureList[0].GetAABB(out AABB ab1, 0);
				AABB aab = new AABB(Position, ab1.Width, ab1.Height);
				Entity.World.GetSystem<Systems.PhysicsSystem>().PhysicsWorld.QueryAABB(EnabledCallback, aab);
			}
			return true;
		}

		private bool EnabledCallback(Fixture fixture) {
			if(fixture.Body != this) {
				Entity.World.GetSystem<Systems.PhysicsSystem>().AddTodo(() => { Entity.SendMessage(new Message("OnCollision") { Content = { { "Sender", FixtureList[0] }, { "Other", fixture } } }); });
			}
			return true;
		}

		[MoonSharpHidden]
		private bool SendCollisionMessage(Fixture sender, Fixture other, Contact contact) {
			Entity.World.GetSystem<Systems.PhysicsSystem>().AddTodo(() => { Entity.SendMessage(new Message("OnCollision") { Content = { { "Sender", sender }, { "Other", other }} }); });
			
			return true;
		}

		[MessageHandler]
		public bool OnDisable(Message _) {
			Enabled = false;
			OnCollision -= SendCollisionMessage;
			return true;
		}

		[MessageHandler]
		public bool OnDestroy(Message _) {
			World.Remove(this);
			return true;
		}

		public IComponent Clone(Entity entity) {
			Body b = new Body(entity) {
				AngularDamping = AngularDamping,
				AngularVelocity = AngularVelocity,
				Awake = Awake,
				BodyType = BodyType,
				ControllerFilter = ControllerFilter,
				Enabled = Enabled,
				IgnoreCCD = IgnoreCCD,
				IgnoreGravity = IgnoreGravity,
				Inertia = Inertia,
				IsBullet = IsBullet,
				IslandIndex = IslandIndex,
				FixedRotation = FixedRotation,
				LinearDamping = LinearDamping,
				LinearVelocity = LinearVelocity,
				LocalCenter = LocalCenter,
				SleepingAllowed = SleepingAllowed,
				Position = Position,
				Rotation = Rotation,
				Mass = Mass,
				Tag = entity
			};
			foreach(Fixture f in FixtureList) {
				f.CloneOnto(b);
			}
			return b;
		}
	}
}
