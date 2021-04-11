using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;

namespace MainGame.Systems {
	using ECS;
	using Components;
	using Particles;
	[MoonSharpUserData]
	public class ParticleSystem : BaseSystem, IUpdateable, IDrawable {
		private readonly Dictionary<Type, ParticleGroup> _particleGroups = new Dictionary<Type,ParticleGroup>();

		public ParticleSystem(World world, MainGame game) : base(world) {
			AddParticleGroup(new BlockBreakParticleGroup(game.SpriteBatch, game.Content.Load<Texture2D>("Textures/pixel")));
			AddParticleGroup(new BloodParticleGroup(game.SpriteBatch, game.Content.Load<Texture2D>("Textures/pixel")));
			AddParticleGroup(new AttackParticleGroup(game.SpriteBatch, game.Content.Load<Texture2D>("Textures/pixel")));
		}

		public void AddParticles<T>(Vector2 position, int amount) where T : ParticleGroup
			=> _particleGroups[typeof(T)].AddParticles(position, amount);

		private void AddParticleGroup(ParticleGroup group) {
			_particleGroups.Add(group.GetType(), group);
		}

		public void Update(float deltaTime) {
			foreach(ParticleGroup group in _particleGroups.Values) {
				group.Update(deltaTime);
			}
		}

		public void Draw() {
			foreach(ParticleGroup group in _particleGroups.Values) {
				group.Draw();
			}
		}
	}
}
