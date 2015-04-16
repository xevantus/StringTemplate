using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringTemplate.Cache;

namespace StringTemplate.Reflection
{
	internal class DynamicObjectReflectionProvider : IReflectionProvider
	{
		public Func<object, object> GetGetter(string path )
		{
		
			var splitPath = path.TokenizeString();

			var subPath = string.Join(".", splitPath.Skip(1));

			if(subPath.Length > 0)
			{
				return (obj) =>
				{
					var dictionary = (IDictionary<string, object>)obj;

					var rootObj = dictionary[splitPath.First()];

					return Cache.GetOrAdd(rootObj.GetType(), subPath)(rootObj);
				};
			}
			else
			{
				return (obj) =>
				{
					var dictionary = (IDictionary<string, object>)obj;

					return dictionary[path];
				};
			}
		}

		public Type Type { get; set; }

		public ObjectGetCache Cache { get;set; }

	}
}
