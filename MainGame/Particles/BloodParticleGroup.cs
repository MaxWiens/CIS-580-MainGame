using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MainGame.Particles {
	public class BloodParticleGroup : ParticleGroup {
		public BloodParticleGroup(SpriteBatch sb, Texture2D texture) : base(200, sb, texture) {
		}

		protected override void InitalizeParticle(ref Particle particle, Vector2 position) {
			base.InitalizeParticle(ref particle, position);
			particle.Color = Color.Crimson;
			particle.SourceRectangle = new Rectangle(0, 0, 1, 1);
			particle.Scale = Vector2.One * 5f;
			particle.Velocity = Util.Rand.UnitCircle()*2f;
			particle.Lifetime = Util.Rand.Float()*10f;
		}

		protected override void UpdateParticle(ref Particle particle, float dt) {
			base.UpdateParticle(ref particle, dt);
			if(dt > 0) {
				particle.Velocity *= 0.99f;
				particle.Scale *= 0.99f;
			}
		}
	}
}
