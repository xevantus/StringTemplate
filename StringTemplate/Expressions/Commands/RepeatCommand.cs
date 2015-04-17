using StringTemplate.Cache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Expressions.Commands
{
	internal class RepeatCommand : CommandExpression.Command
	{
		public RepeatCommand()
		{
			CommandString = CommandExpression.CommandPrefix + "repeat";
			EndCommand = CommandExpression.CommandPrefix + "repeat:" + CommandExpression.CommandPrefix + "end";
			HasEndCommand = true;
			ExecuteCommand = Run;
		}


		public string Run(CommandExpression parent, TemplateProcessor cache, object[] objs)
		{
			var splitParams = parent.Parameters.TokenizeString(':', StringSplitOptions.None);
			var param = splitParams.First();

			int? iterations = (int?)parent.GetParameter("Iterations");

			Func<object, object> enumGet = (Func<object, object>)parent.GetParameter("Enumerator");

			int? objIndex = (int?)parent.GetParameter("ObjectIndex");

			string joinCharacter = ((string)parent.GetParameter("Join")) ?? string.Empty;

			if (iterations == null && enumGet == null)
			{

				joinCharacter = splitParams.Skip(1).FirstOrDefault() ?? string.Empty;

				parent.SetParameter("Join", joinCharacter);

				if (param[0] == CommandExpression.CommandPrefix)
				{
					param = param.Substring(1);
					iterations = int.Parse(param);

					parent.SetParameter("Iterations", iterations);
				}
				else
				{
					var exp = new FormatExpression(string.Format("{{{0}}}", param)).GetFunctionFromCache(cache, objs);

					var splitParam = param.TokenizeString();

					var index = 0;

					int.TryParse(splitParam.First(), out index);

					var checkObj = exp(objs[index]);

					if (checkObj is IEnumerable)
					{
						parent.SetParameter("Enumerator", exp);
						enumGet = exp;
						parent.SetParameter("ObjectIndex", index);
						objIndex = index;
					}
					else
					{
						iterations = (int)checkObj;
						parent.SetParameter("Iterations", iterations);
					}
				}
			}


			List<string> evaluatedExpressions = new List<string>();
			parent.CommandData.Reset();
			if (iterations != null)
			{
				for (int i = 0; i < iterations; i++)
				{
					parent.CommandData.SetParameter("index", i);

					evaluatedExpressions.Add(string.Join(string.Empty, parent.ParsedExpressions.Select(x => x.Eval(cache, objs))));

				}
			}
			else
			{
				var count = 0;
				foreach (var obj in ((IEnumerable)enumGet(objs[objIndex.Value])))
				{
					parent.CommandData.SetParameter("index", count);
					parent.CommandData.SetParameter("current", obj);
					evaluatedExpressions.Add(string.Join(string.Empty, parent.ParsedExpressions.Select(x => x.Eval(cache, objs))));

					count++;
				}
			}

			return string.Join(joinCharacter, evaluatedExpressions);
		}
	}
}
