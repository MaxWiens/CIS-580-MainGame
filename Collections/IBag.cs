using System.Collections;
using System.Collections.Generic;
namespace MainGame.Collections {
	public interface IBag {
		void Add(object key, object value);
		bool Remove(object key);
		int Count { get; }
	}

	public interface IKeyBag<TKey> : IBag {
		void Add(TKey key, object value);
		bool Remove(TKey key);
	}
	
	public interface IValueBag<TValue> : IBag {
		void Add(object key, TValue value);
	}

	public interface IBag<TKey,TValue> : IKeyBag<TKey>, IValueBag<TValue> where TValue : struct {
		void Add(TKey key, TValue value);
	}
}