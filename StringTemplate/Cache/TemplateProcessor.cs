using StringTemplate.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Cache
{
	public class TemplateProcessor
	{
		private ObjectGetCache _ObjectCache;

		public ObjectGetCache ObjectCache
		{
			get { return _ObjectCache; }
			set { _ObjectCache = value; }
		}

		private ExpressionCache _ExpressionCache;

		public ExpressionCache ExpressionCache
		{
			get { return _ExpressionCache; }
			set { _ExpressionCache = value; }
		}

		public TemplateProcessor()
		{
			ExpressionCache = new ExpressionCache();
			ObjectCache = new ObjectGetCache();
		}

		public TemplateProcessor(ExpressionCache expressionCache, ObjectGetCache objectCache)
		{
			ExpressionCache = expressionCache;
			ObjectCache = objectCache;
		}

		public static TemplateProcessor CreateDefaultCache()
		{
			return new TemplateProcessor();
		}

		public string Format(string sourceString, params object[] nObjects)
		{
			var expressions = ExpressionCache.GetOrAdd(sourceString);

			var evaluatedExpressions = expressions.Select(x => x.Eval(this, nObjects));

			return string.Join("", evaluatedExpressions);
		}

		public void ClearCache()
		{
			ExpressionCache.Clear();
			ObjectCache.Clear();
		}

	}
}
