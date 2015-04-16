using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Reflection
{
	internal class PropertyTranslator : IMemberTranslator
	{

		public PropertyInfo Info { get; set; }

		internal PropertyTranslator(PropertyInfo info)
		{
			Info = info;
		}

		public object GetValue( object obj )
		{
			return Info.GetValue(obj);
		}

		public Func<object, object> GetGetter()
		{
			return Info.GetValue;
		}


		public Type GetMemberType()
		{
			return Info.PropertyType;
		}
	}
}
