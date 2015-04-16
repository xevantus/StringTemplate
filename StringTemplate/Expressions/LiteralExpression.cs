using StringTemplate.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate
{
	public class LiteralExpression : ITextExpression
	{
		public LiteralExpression(string literalText)
		{
			LiteralText = literalText;
		}

		public ITextExpression Parent { get; set; }

		public string LiteralText
		{
			get;
			private set;
		}

		public string Eval(TemplateProcessor cache, params object[] objs)
		{
			string literalText = LiteralText
				.Replace("{{", "{")
				.Replace("}}", "}");
			return literalText;
		}

	}
}
