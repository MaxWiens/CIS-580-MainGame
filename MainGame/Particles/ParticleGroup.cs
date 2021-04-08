using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MainGame.Particles {
	public abstract class ParticleGroup {
		private readonly Particle[] _particles;
		private int _aliveParticleCount = 0;
		private readonly Queue<int> _availableIndicies = new Queue<int>();
		private readonly SpriteBatch _spriteBatch;
		private readonly Texture2D _texture;
		public ParticleGroup(int maxParticles, SpriteBatch spriteBatch, Texture2D texture) {
			_particles = new Particle[maxParticles];
			_spriteBatch = spriteBatch;
			_texture = texture;
		}

		public void AddParticles(Vector2 position, int amount) {
			for(int i = 0; i < amount; i++) {
				if(_aliveParticleCount >= _particles.Length) {
					break;
				}
				if(!_availableIndicies.TryDequeue(out int index)) index = _aliveParticleCount;
				_particles[index].IsActive = true;
				InitalizeParticle(ref _particles[index], position);
				_aliveParticleCount++;
				
			}
		}

		public void Update(float dt) {
			for(int i = 0; i < _particles.Length; i++) {
				if(_particles[i].IsActive) {
					UpdateParticle(ref _particles[i], dt);
					if(!_particles[i].IsActive) {
						_availableIndicies.Enqueue(i);
						_aliveParticleCount--;
					}
				}
			}
		}

		public void Draw() {
			for(int i = 0; i < _particles.Length; i++) {
				if(_particles[i].IsActive) {
					Particle p = _particles[i];
					_spriteBatch.Draw(
						_texture,
						p.Position,
						p.SourceRectangle,
						p.Color,
						p.Rotation,
						Vector2.Zero,
						p.Scale,
						SpriteEffects.None,
						0
					);
				}
			}
		}

		protected virtual void UpdateParticle(ref Particle particle, float dt) {
			particle.Lifetime -= dt;
			if(particle.Lifetime <= 0) {
				particle.IsActive = false;
				return;
			}
			particle.Position += particle.Velocity * dt;
			particle.Rotation += particle.AngularVelocity * dt;
		}

		protected virtual void InitalizeParticle(ref Particle particle, Vector2 position) {
			particle.Position = position;
			particle.Velocity = Util.Rand.UnitCircle()*10f;
			particle.Lifetime = Util.Rand.Float();
			particle.Color = Color.White;
			particle.SourceRectangle = new Rectangle(_texture.Width / 2, _texture.Height / 2, 2, 2);
			particle.Scale = Vector2.One;
		}
	}
}
