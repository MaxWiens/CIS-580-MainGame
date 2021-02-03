using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.ECS {
	public abstract class DrawSystem : System {
		public DrawSystem(ZaWarudo world) : base(world) { }

		public abstract void Draw();

		// Messages
		// void OnEnd();
		// void OnEnable();
		// void OnDisable();
	}
}
