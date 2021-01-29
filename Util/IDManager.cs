using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Util {
	class IDManager {
		private ulong _id = 0;
		private Queue<ulong> _available = new Queue<ulong>();

		public ulong NextID() {
			return _available.TryDequeue(out ulong result) ? result : _id++;
		}
		public void ReturnID(ulong id) {
			if(id < _id && !_available.Contains(id)) _available.Enqueue(id);
		}
	}
}
