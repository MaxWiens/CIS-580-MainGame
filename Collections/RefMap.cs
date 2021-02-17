using System;
using System.Collections;
using System.Collections.Generic;

namespace MainGame.Collections {
	public class RefMap<TKey, TValue> : IRefMap<TKey,TValue> where TValue : struct {
		private readonly Dictionary<TKey, int> _keyIdxMap = new Dictionary<TKey, int>();
		private TValue[] _values;
		private int _count = 0;
		private int _size = 0;
		private const int DEFAULT_SIZE = 4;
		private static readonly TValue[] _emptyValues = new TValue[0];
		public int Count => _count;
		private readonly SortedSet<int> _continiousEndIdxs = new SortedSet<int>();
		private readonly HashSet<int> _continiousStartIdxs = new HashSet<int>();

		public IEnumerable<TKey> Keys => _keyIdxMap.Keys;

		public ref TValue this[TKey id] => ref _values[_keyIdxMap[id]];

		public ref TValue TryGetValue(TKey id, ref TValue fallbackValue, out bool isSuccessful) {
			if(_keyIdxMap.TryGetValue(id, out int idx)) {
				isSuccessful = true;
				return ref _values[idx];
			}
			isSuccessful = false;
			return ref fallbackValue;
		}

		public RefMap() {
			_values = _emptyValues;
			_continiousEndIdxs.Add(0);
		}

		public RefMap(int startingSize) {
			if(startingSize < 0)
				throw new ArgumentOutOfRangeException();
			_values = new TValue[startingSize];
			_size = startingSize;
			_continiousEndIdxs.Add(0);
		}

		public void Add(TKey key, TValue value) {
			if(!_keyIdxMap.ContainsKey(key)) {
				// resize array if needed
				if(_count >= _size) {
					if(_size == 0) {
						_values = new TValue[DEFAULT_SIZE];
						_size = DEFAULT_SIZE;
					} else {
						Array.Resize(ref _values, _size <<= 1);
					}
				}

				int availableIdx = _continiousEndIdxs.Min; // add at index
				_continiousEndIdxs.Remove(availableIdx);
				if(availableIdx == _count) { //at very end
					_continiousEndIdxs.Add(_count + 1);
				} else if(!_continiousStartIdxs.Remove(availableIdx + 1)) {
					//didn't connect two continious portions	
					_continiousEndIdxs.Add(availableIdx + 1);
				}
				/*cool buggy code that inables the foobag glitch
				else if(_continiousStartIdxs.Remove(availableIdx + 1)) {
					_continiousStartIdxs.Add(availableIdx);
				}*/

				_keyIdxMap.Add(key, availableIdx);
				_values[availableIdx] = value;
				_count++;
			} else
				throw new ArgumentException($"Already contains key \"{key}\"");
		}

		public void Add(object key, object value) => Add((TKey)key, (TValue)value);
		public void Add(TKey key, object value) => Add(key, (TValue)value);
		public void Add(object key, TValue value) => Add(key, (TValue)value);

		public bool Remove(TKey key) {
			if(_keyIdxMap.TryGetValue(key, out int i)) {
				_keyIdxMap.Remove(key);
				_values[i] = default;

				bool isStart = _continiousStartIdxs.Remove(i);
				bool isEnd = _continiousEndIdxs.Remove(i + 1);
				if(!(isStart && isEnd)) {
					if(isStart) {
						// removed item at start of continious segment
						_continiousStartIdxs.Add(i + 1);
					} else if(isEnd) {
						// removed item at end of continious segment
						_continiousEndIdxs.Add(i);
					} else {
						// removed in middle of continious segment
						_continiousEndIdxs.Add(i);
						_continiousStartIdxs.Add(i + 1);
					}
				}
				_count--;
				return true;
			}
			return false;
		}
		public bool Remove(object key) => Remove((TKey)key);
	}
}
