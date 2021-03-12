using System.Collections.Generic;
namespace MainGame.Collections {
	public interface IKeyRefMap<TKey> {
		int Count { get; }
		void Add(TKey key, object value);
		bool Remove(TKey key);
		bool Remove(TKey key, out object value);
		IEnumerable<TKey> Keys { get; }
		object this[TKey id] { get; set; }
		bool TryGetValue(TKey id, out object value);
	}
	
	public interface IRefMap<TKey,TValue> : IKeyRefMap<TKey> {
		void Add(TKey key, TValue value);
		bool Remove(TKey key, out TValue value);
		new ref TValue this[TKey id] { get; }
		ref TValue TryGetValue(TKey id, ref TValue fallbackValue, out bool isSuccessful);
	}
}