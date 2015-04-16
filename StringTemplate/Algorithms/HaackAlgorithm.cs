using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Algorithms
{
	public class HaackAlgorithm : IFormatterAlgorithm
	{
		


		public IEnumerable<ITextExpression> Parse(ITextExpression parent, string sourceText)
		{
			int exprEndIndex = -1;
			int expStartIndex;

			do
			{
				expStartIndex = IndexOfExpressionStart(sourceText, Math.Min(exprEndIndex + 1, sourceText.Length));
				if (expStartIndex < 0)
				{
					//everything after last end brace index.
					if (exprEndIndex + 1 < sourceText.Length)
					{
						yield return new LiteralExpression(
							sourceText.Substring(exprEndIndex + 1)) { Parent = parent };
					}
					break;
				}

				if (expStartIndex - exprEndIndex - 1 > 0)
				{
					//everything up to next start brace index
					yield return new LiteralExpression(sourceText.Substring(exprEndIndex + 1
					  , expStartIndex - exprEndIndex - 1)) { Parent = parent };
				}

				int endBraceIndex = IndexOfExpressionEnd(sourceText, expStartIndex + 1);
				if (endBraceIndex < 0)
				{
					yield return new FormatExpression(sourceText.Substring(expStartIndex)) { Parent = parent };
				}
				else
				{
					//everything from start to end brace.
					if (CommandLookahead(sourceText, expStartIndex))
					{
						var command = new CommandExpression(sourceText.Substring(expStartIndex,
																endBraceIndex - expStartIndex + 1)) { Parent = parent };
						exprEndIndex = command.SetExpression(sourceText, endBraceIndex + 1);
						
						yield return command;
					}
					else
					{
						exprEndIndex = endBraceIndex;
						yield return new FormatExpression(sourceText.Substring(expStartIndex,
														endBraceIndex - expStartIndex + 1)) { Parent = parent };
					}

				}
			} while (expStartIndex > -1);
		}

		private int IndexOfExpressionStart(string format, int startIndex)
		{
			int index = format.IndexOf('{', startIndex);
			if (index == -1)
			{
				return index;
			}

			//peek ahead.
			if (index + 1 < format.Length)
			{
				char nextChar = format[index + 1];
				if (nextChar == '{')
				{
					return IndexOfExpressionStart(format, index + 2);
				}
			}

			return index;
		}

		private int IndexOfExpressionEnd(string format, int startIndex)
		{
			int endBraceIndex = format.IndexOf('}', startIndex);
			if (endBraceIndex == -1)
			{
				return endBraceIndex;
			}
			//start peeking ahead until there are no more braces...
			// }}}}
			int braceCount = 0;
			for (int i = endBraceIndex + 1; i < format.Length; i++)
			{
				if (format[i] == '}')
				{
					braceCount++;
				}
				else
				{
					break;
				}
			}
			if (braceCount % 2 == 1)
			{
				return IndexOfExpressionEnd(format, endBraceIndex + braceCount + 1);
			}

			return endBraceIndex;
		}

		//Additions made 4/15/2015

		private bool CommandLookahead(string format, int index)
		{
			if(index + 1 < format.Length)
			{
				return format[index+1] == CommandExpression.CommandPrefix;
			}

			return false;
		}
	}
}
