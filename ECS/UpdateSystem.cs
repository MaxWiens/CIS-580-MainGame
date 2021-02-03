using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.ECS {
	public abstract class UpdateSystem : System {
		public UpdateSystem(ZaWarudo world) : base(world) { }

		public abstract void Update(float deltaTime);

		// Messages
		// void OnEnd();
		// void OnEnable();
		// void OnDisable();
	}
}
