using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Reflection
{
	internal class FieldTranslator : IMemberTranslator
	{

		public FieldInfo Info { get; set; }

		internal FieldTranslator(FieldInfo info)
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
			return Info.FieldType;
		}
	}
}
