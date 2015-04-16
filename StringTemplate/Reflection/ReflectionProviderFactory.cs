using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Reflection
{
	internal static class ReflectionProviderFactory
	{
		internal static IReflectionProvider GetReflectionProvider(Type type)
		{
			if(typeof(IDictionary<string, object>).IsAssignableFrom(type))
			{
				return new DynamicObjectReflectionProvider{ Type = type};
			}

			return new DefaultReflectionProvider{ Type = type};
		}
	}
}
