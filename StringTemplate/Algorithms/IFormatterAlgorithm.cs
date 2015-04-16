using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Algorithms
{
	public interface IFormatterAlgorithm
	{
		IEnumerable<ITextExpression> Parse(ITextExpression parent, string sourceText);
	}
}
