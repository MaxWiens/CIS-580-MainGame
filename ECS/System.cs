using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.ECS {
	public abstract class System {
		protected readonly ZaWarudo world;
	
		public System(ZaWarudo world) {
			this.world = world;
		}

		// Messages
		// void OnEnd();
		// void OnEnable();
		// void OnDisable();
		// void OnInit();
		// void OnContentLoad();
	}
}
