using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.ECS {
	class World {
		public Guid MakeEntity() {
			return Guid.NewGuid();
		}
	}
}
