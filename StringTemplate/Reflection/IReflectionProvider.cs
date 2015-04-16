using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StringTemplate.Cache;

namespace StringTemplate.Reflection
{
	internal interface IReflectionProvider
	{
		Type Type { get;set; }

		ObjectGetCache Cache { get;set; }

		Func<object, object> GetGetter(string path);
	}
}
