using StringTemplate.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Expressions.Commands
{
	internal class ParentCommand : CommandExpression.Command
	{
		public ParentCommand()
		{
			CommandString = CommandExpression.CommandPrefix + "parent";
			HasEndCommand = false;
			ExecuteCommand = Run;
		}

		public string Run(CommandExpression parent, TemplateProcessor cache, object[] objs)
		{
			var parentParams = parent.Parameters.TokenizeString(':');

			var parentCount = parentParams.Count(x => x == "$parent") + 2;

			var commandString = parentParams.Last();

			var command = CommandExpression.GetCommands().First(x => x.CommandString == commandString);

			var parentCommand = command as IParentedCommand;

			if(parentCommand == null)
			{
				throw new Exception();
			}

			parentCommand.ParentLevel = parentCount;

			return command.ExecuteCommand(parent, cache, objs);

		}
	}
}
