using System;
using System.Collections.Generic;
using System.Text;
namespace MainGame.ECS {
	using Util;
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
