using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.ECS {
	public abstract class DrawSystem : System {
		public DrawSystem(World world) : base(world) { }

		public abstract void Draw();

		// Messages
		// void OnEnd();
		// void OnEnable();
		// void OnDisable();
	}
}
