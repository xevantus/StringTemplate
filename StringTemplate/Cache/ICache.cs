using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Cache
{
	public interface ICache<TKey, TValue>
	{
		TValue GetOrAdd(TKey key, Func<TValue> valueFactory);

		void Clear();
	}
}
