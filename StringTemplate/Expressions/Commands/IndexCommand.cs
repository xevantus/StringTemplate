using StringTemplate.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Expressions.Commands
{
	internal class IndexCommand : CommandExpression.Command, IParentedCommand
	{
		public int ParentLevel { get; set; }

		public IndexCommand()
		{
			CommandString = CommandExpression.CommandPrefix + "index";
			HasEndCommand = false;
			ExecuteCommand = Run;
			ParentLevel = 1;
		}

		public string Run(CommandExpression parent, TemplateProcessor cache, object[] objs)
		{
			var p = parent as ITextExpression;

			var foundCount = 0;

			while (foundCount < ParentLevel)
			{
				p = p.Parent;

				if(p is CommandExpression)
					foundCount++;
			}

			var cmd = p as CommandExpression;

			return cmd.CommandData.GetParameter("index").ToString();

		}
	}
}
