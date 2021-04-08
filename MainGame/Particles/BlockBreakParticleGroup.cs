using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MainGame.Particles {
	public class BlockBreakParticleGroup : ParticleGroup {
		public BlockBreakParticleGroup(SpriteBatch sb, Texture2D texture) : base(200, sb, texture) {
		}
	}
}
