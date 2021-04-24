using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class KillAdder : Component {
		[JsonInclude] public bool IsSkeleton;
		public KillAdder(Entity entity) : base(entity) { }

		[MessageHandler(10)]
		public bool OnDeath(Message message) {
			Entity.World.GetSystem<Systems.UI.HealthBarSystem>().AddKill(IsSkeleton);
			return true;
		}

		public override IComponent Clone(Entity entity) => new KillAdder(entity) {
			IsSkeleton = IsSkeleton
		};
	}
}
