using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MainGame.Particles {
	public class AttackParticleGroup : ParticleGroup {
		public AttackParticleGroup(SpriteBatch sb, Texture2D texture) : base(200, sb, texture) {
		}

		protected override void InitalizeParticle(ref Particle particle, Vector2 position) {
			base.InitalizeParticle(ref particle, position);
			particle.Color = Color.White;
			particle.SourceRectangle = new Rectangle(0, 0, 1, 1);
			particle.Position += (Util.Rand.UnitCircle()*10f)-(Vector2.One*5f);
			particle.Scale = new Vector2(2, 20);
			//particle.Velocity = Util.Rand.UnitCircle()*2f;
			particle.Lifetime = Util.Rand.Float()*0.2f;
			particle.AngularVelocity = (Util.Rand.Float() * MathHelper.TwoPi*10f) - MathHelper.TwoPi*5f;
		}

		protected override void UpdateParticle(ref Particle particle, float dt) {
			base.UpdateParticle(ref particle, dt);
			if(dt > 0) {
				if(particle.Lifetime < 0.1f) {
					particle.Scale *= 0.99f;
				} else if(particle.Scale.X < 5f) {
					particle.Scale *= 1.01f;
				}
			}
		}
	}
}
