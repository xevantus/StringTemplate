using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Reflection
{
	internal static class MemberTranslatorFactory
	{
		
		public static IMemberTranslator CreateTranslator(this MemberInfo info)
		{
			if(info is PropertyInfo)
			{
				return new PropertyTranslator(info as PropertyInfo);
			}
			if(info is FieldInfo)
			{
				return new FieldTranslator(info as FieldInfo);
			}

			throw new Exception();
		}
	}
}
