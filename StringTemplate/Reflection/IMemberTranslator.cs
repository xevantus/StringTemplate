using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Reflection
{
	internal interface IMemberTranslator
	{
		object GetValue(object obj);

		Func<object, object> GetGetter();

		Type GetMemberType();
	}
}
