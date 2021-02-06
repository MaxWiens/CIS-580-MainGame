using System;
using System.Collections.Generic;

namespace MainGame.Collections {
	public class Bag<TKey, TValue> : IBag<TKey,TValue> where TValue : struct {
		private readonly Dictionary<TKey, int> _dict = new Dictionary<TKey, int>();
		private readonly SortedSet<int> _availableIdxs = new SortedSet<int>();
		private TValue[] _values;
		private static readonly TValue[] _emptyValues = new TValue[0];
		private int _count = 0;
		private int _size = 0;
		private const int DEFAULT_SIZE = 4;
		private int _firstUnfilledIdx = 0;
		private int _lastFilleddIdx = -1;
		
		public int Count => _count;

		public IEnumerable<TKey> Keys => _dict.Keys;

		public ref TValue this[TKey id] => ref _values[_dict[id]];

		public bool TryGetValue(TKey id, ref TValue value) {
			if(_dict.TryGetValue(id, out int idx)) {
				ref TValue v = ref _values[idx];
				value = v;
				return true;
			}
			return false;
		}
			

		public Bag() {
			_values = _emptyValues;
		}

		public Bag(int startingSize) {
			if(startingSize < 0)
				throw new ArgumentOutOfRangeException();
			_values = new TValue[startingSize];
			_size = startingSize;
		}

		public void Add(TKey key, TValue value) {
			if(!_dict.ContainsKey(key)) {
				if(_count >= _size) {
					if(_size == 0) {
						_values = new TValue[DEFAULT_SIZE];
						_size = DEFAULT_SIZE;
					} else {
						Array.Resize(ref _values, _size <<= 1);
					}
				}
				_values[_firstUnfilledIdx] = value;
				_dict.Add(key, _firstUnfilledIdx);
				if(_availableIdxs.Count > 0) {
					// empty in array before last filled value
					_firstUnfilledIdx = _availableIdxs.Min;
					_availableIdxs.Remove(_availableIdxs.Min);
				} else if(_lastFilleddIdx < _firstUnfilledIdx) {
					// array filled up until last filled value
					_firstUnfilledIdx = _count + 1;
					_lastFilleddIdx = _count;
				} else {
					_firstUnfilledIdx = _count;
					_lastFilleddIdx = _count - 1;
				}
				_count++;
			} else
				throw new ArgumentException($"Already contains key \"{key}\"");
		}
		
		public void Add(object key, object value) => Add((TKey)key, (TValue)value);
		public void Add(TKey key, object value) => Add(key, (TValue)value);
		public void Add(object key, TValue value) => Add(key, (TValue)value);

		public bool Remove(TKey key) {
			if(_dict.TryGetValue(key, out int i)) {
				_dict.Remove(key);
				_values[i] = default;
				if(_firstUnfilledIdx > _lastFilleddIdx) {
					// if filled up until last filled index
					_firstUnfilledIdx = i;
					if(i == _lastFilleddIdx) _lastFilleddIdx = i - 1;
				}
				// if has spaces in array
				else if(i < _firstUnfilledIdx) {
					_availableIdxs.Add(_firstUnfilledIdx);
					_firstUnfilledIdx = i;
				} else if(i == _lastFilleddIdx) {
					//move back
					int max;
					while((max = _availableIdxs.Max) == --_lastFilleddIdx)
						_availableIdxs.Remove(max);
				} else {
					// if the item removed is somewhere in the middle of the first unfilled and hte last filled idx
					_availableIdxs.Add(i);
				}
				_count--;
				return true;
			}
			return false;
		}
		public bool Remove(object key) => Remove((TKey)key);
	}
}
