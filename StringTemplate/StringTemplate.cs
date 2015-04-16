using StringTemplate.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate
{
	public static class StringTemplate
	{
		private static TemplateProcessor _Cache;

		private static TemplateProcessor Cache
		{
			get
			{
				if(_Cache == null )
				{
					_Cache = TemplateProcessor.CreateDefaultCache();
				}

				return _Cache;
			}
		}

		public static string SFormat(this string sourceString, params object[] nObjects)
		{
			return Format(sourceString, nObjects);
		}

		public static string Format(string sourceString, params object[] nObjects)
		{
			return Cache.Format(sourceString, nObjects);
		}

		public static void ClearCache()
		{
			Cache.ClearCache();
		}

	}
}
