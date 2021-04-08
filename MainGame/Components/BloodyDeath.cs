using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class BloodyDeath : Component {
		public BloodyDeath(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) {
			return new BloodyDeath(entity) {};
		}

		[MessageHandler]
		public bool OnDeath(Message _) {
			Entity.World.GetSystem<Systems.ParticleSystem>().AddParticles<Particles.BloodParticleGroup>(Entity.GetComponent<Body>().Position, 10);
			return true;
		}
	}
}
