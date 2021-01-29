using System;
using System.Collections.Generic;
using System.Text;
using MainGame.Util;
namespace MainGame.ECS {
	class Component {
		public readonly ulong ID;
		public Component() {
			ID = IDManager.NextID();
		}

		~Component() {
			IDManager.ReturnID(ID);
		}
	}
}
