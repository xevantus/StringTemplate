using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate
{
	internal static class Utilities
	{
		public static IEnumerable<string> TokenizeString(this string source, char token = '.', StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
		{
			var tokens = source.Split(new char[]{token}, options).AsEnumerable();

			tokens = tokens.Select(x => x.Last() == '\\' ? x + token : x);

			return tokens;
		}
	}
}
