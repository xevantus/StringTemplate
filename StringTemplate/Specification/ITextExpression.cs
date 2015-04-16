using StringTemplate.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate
{
	public interface ITextExpression
	{
		ITextExpression Parent { get; set; }
		string Eval( TemplateProcessor cache, params object[] objs);
	}
}
