using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
namespace MainGame.Particles {
	public struct Particle {
		public bool IsActive;
		public Vector2 Position;
		public Vector2 Velocity;
		public Vector2 Scale;
		public Color Color;
		public float Lifetime;
		public float Rotation;
		public float AngularVelocity;
		public Rectangle SourceRectangle;
	}
}
