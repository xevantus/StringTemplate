using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StringTemplate.Cache
{
	public class Cache<TKey, TValue> : ICache<TKey, TValue>
	{
		private IImmutableDictionary<TKey,TValue> _Cache = ImmutableDictionary.Create<TKey, TValue>();

		public TValue GetOrAdd(TKey key, Func<TValue> valueFactory)
		{
			
			Lazy<TValue> newValue = new Lazy<TValue>(valueFactory);

			while(true)
			{
				var oldCache = _Cache;
				TValue value;

				if(oldCache.TryGetValue(key, out value))
					return value;

				value = newValue.Value;
				var newCache = oldCache.Add(key, value);
				if(Interlocked.CompareExchange(ref _Cache, newCache, oldCache) == oldCache)
				{
					return value;
				}
			}
		}

		public void Clear()
		{
			while (true)
			{
				var oldCache = _Cache;
				var newCache = _Cache.Clear();
				if(Interlocked.CompareExchange(ref _Cache, newCache, oldCache) == oldCache)
				{
					return;
				}
			}
		}
	}
}
