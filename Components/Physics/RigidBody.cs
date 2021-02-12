using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
namespace MainGame.Components {
	public struct RigidBody {
		public Vector2 Velocity;
		public Vector2 Force;
		public int Layers;
	}
}
