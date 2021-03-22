using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using ECS;
namespace MainGame.Components.AI {
	public enum EnemyState {
		Wonder,
		Follow
	}
	[MoonSharp.Interpreter.MoonSharpUserData]
	public class BasicEnemyAI : Component {
		public EnemyState State = EnemyState.Wonder;
		[JsonInclude] public float Range = 128f;
		[JsonInclude] public float Speed = 20f;
		public float WonderTimer = 0f;
		public Vector2 WonderDirection = Vector2.Zero;

		public BasicEnemyAI(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new BasicEnemyAI(entity) { 
			Range = Range
		};
	}
}
