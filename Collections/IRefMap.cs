using System.Collections;
using System.Collections.Generic;
namespace MainGame.Collections {
	public interface IRefMap {
		void Add(object key, object value);
		bool Remove(object key);
		int Count { get; }
	}

	public interface IKeyRefMap<TKey> : IRefMap {
		void Add(TKey key, object value);
		bool Remove(TKey key);
	}
	
	public interface IValueRefMap<TValue> : IRefMap {
		void Add(object key, TValue value);
	}

	public interface IRefMap<TKey,TValue> : IKeyRefMap<TKey>, IValueRefMap<TValue> where TValue : struct {
		void Add(TKey key, TValue value);
	}
}