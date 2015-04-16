using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StringTemplate.Reflection;

namespace StringTemplate.Cache
{
	public class ObjectGetCache
	{
		private Cache<Type, Cache<string, Func<object,object>>> _InternalCache = new Cache<Type,Cache<string,Func<object,object>>>();



		public Func<object, object> GetOrAdd(Type key, string referenceText)
		{
			var typeCache = _InternalCache.GetOrAdd(key, () => new Cache<string, Func<object,object>>());

			return typeCache.GetOrAdd(referenceText, () =>
			{
				var reflectionProvider = ReflectionProviderFactory.GetReflectionProvider(key);

				reflectionProvider.Cache = this;

				return reflectionProvider.GetGetter(referenceText);

			});
			
		}

		public void Clear()
		{
			_InternalCache.Clear();
		}
	}
}
