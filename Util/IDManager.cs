using System;
using System.Collections.Generic;
using System.Text;

namespace Util {
	public static class IDManager {
		private static ulong _id = 0;
		private static Queue<ulong> _available = new Queue<ulong>();

		public static ulong NextID() {
			lock(_available)
				return _available.TryDequeue(out ulong result) ? result : _id++;
		}
		public static void ReturnID(ulong id) {
			lock(_available)
				if(id < _id && !_available.Contains(id)) _available.Enqueue(id);
		}

		public static IEnumerator<ulong> NextIDs(int numIds) {
			for(; numIds >= 0; numIds--) {
				lock(_available)
					yield return _available.TryDequeue(out ulong result) ? result : _id++;
			}
		}
	}
}
