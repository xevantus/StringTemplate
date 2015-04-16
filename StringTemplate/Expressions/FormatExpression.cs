using StringTemplate.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate
{
	public class FormatExpression : ITextExpression
	{
		bool _invalidExpression = false;

		public FormatExpression(string expression)
		{
			if (!expression.StartsWith("{") || !expression.EndsWith("}"))
			{
				_invalidExpression = true;
				Expression = expression;
				return;
			}

			string expressionWithoutBraces = expression.Substring(1
				, expression.Length - 2);
			int colonIndex = expressionWithoutBraces.IndexOf(':');
			if (colonIndex < 0)
			{
				Expression = expressionWithoutBraces;
			}
			else
			{
				Expression = expressionWithoutBraces.Substring(0, colonIndex);
				Format = expressionWithoutBraces.Substring(colonIndex + 1);
			}
		}

		public string Expression
		{
			get;
			private set;
		}

		public string Format
		{
			get;
			private set;
		}

		public ITextExpression Parent { get; set; }

		public string Eval(TemplateProcessor cache, params object[] objs)
		{
			if (_invalidExpression)
			{
				throw new FormatException("Invalid expression");
			}
			
			object returnedObject = null;

			returnedObject = GetObjectFromCache(cache, objs);
			
			if(returnedObject == null)
			{
				return string.Empty;
			}

			if(string.IsNullOrWhiteSpace(Format))
			{
				return returnedObject.ToString();
			}
			else
			{
				return string.Format("{0:" + Format + "}", returnedObject);
			}

		}

		public object GetObjectFromCache(TemplateProcessor cache, object[] objs)
		{
			
			object returnedObject = null;

			int index;

			var dotSplit = Expression.TokenizeString();

			if(int.TryParse(dotSplit.First(), out index))
			{
				if(dotSplit.Count() > 1)
				{
					var subExpression = string.Join(".", dotSplit.Skip(1));

					returnedObject = cache.ObjectCache.GetOrAdd(objs[index].GetType(), subExpression)(objs[index]);
				}
				else if(index < objs.Length)
				{
					returnedObject = objs[index];
				}
				else
				{
					throw new Exception();
				}
			}
			else
			{ 
				returnedObject = cache.ObjectCache.GetOrAdd(objs[0].GetType(), Expression)(objs[0]);
			}

			return returnedObject;
		}
		
		public Func<object, object> GetFunctionFromCache(TemplateProcessor cache, object[] objs)
		{
			
			Func<object, object> returnedObject = null;

			int index;

			var dotSplit = Expression.Split(new char[] {'.'}, StringSplitOptions.RemoveEmptyEntries);

			if(int.TryParse(dotSplit[0], out index))
			{
				if(dotSplit.Length > 1)
				{
					var subExpression = string.Join(".", dotSplit.Skip(1));

					returnedObject = cache.ObjectCache.GetOrAdd(objs[index].GetType(), subExpression);
				}
				else if(index < objs.Length)
				{
					returnedObject = (_) => objs[index];
				}
				else
				{
					throw new Exception();
				}
			}
			else
			{ 
				returnedObject = cache.ObjectCache.GetOrAdd(objs[0].GetType(), Expression);
			}

			return returnedObject;
		}

	}
}
