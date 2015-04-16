using StringTemplate.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Cache
{
	public class ExpressionCache
	{
		private Cache<string, IEnumerable<ITextExpression>> _InternalCache = new Cache<string,IEnumerable<ITextExpression>>();

		private IFormatterAlgorithm _Algorithm;

		public ExpressionCache(IFormatterAlgorithm algorithm = null)
		{
			_Algorithm = algorithm ?? new HaackAlgorithm();
		}

		public IEnumerable<ITextExpression> GetOrAdd(string key, ITextExpression parentExpression = null)
		{
			var returnedCache = _InternalCache.GetOrAdd(key, () => _Algorithm.Parse(parentExpression, key));

			returnedCache = returnedCache.Select(x => {x.Parent = parentExpression; return x;});

			return returnedCache;
		}

		public void Clear()
		{
			_InternalCache.Clear();
		}
	}
}
