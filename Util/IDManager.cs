using System.Collections.Generic;

namespace Util {
	public static class IDManager {
		private static ulong _id = 0;
		private static Queue<ulong> _available = new Queue<ulong>();

		public static ulong NextID() {
			lock(_available) {
				if(_available.Count > 0)
					return _available.Dequeue();
			}
			return _id++;
		}
		public static void ReturnID(ulong id) {
			lock(_available) {
				if(id < _id && !_available.Contains(id))
					_available.Enqueue(id);
			}
		}

		public static IEnumerator<ulong> NextIDs(int numIds) {
			ulong id;
			for(; numIds >= 0; numIds--) {
				lock(_available)
					id = _available.Count > 0 ? _available.Dequeue() : _id++;
				yield return id;
			}
		}
	}
}
