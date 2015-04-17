using StringTemplate.Algorithms;
using StringTemplate.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using StringTemplate.Expressions.Commands;

namespace StringTemplate
{
	public class CommandExpression : ITextExpression
	{
		public static char CommandPrefix = '$';

		public string Expression { get; private set; }

		public string Parameters { get; private set; }

		internal Command CommandData { get; set; }

		internal IEnumerable<ITextExpression> ParsedExpressions { get; set; }

		public ITextExpression Parent { get; set; }

		private Dictionary<string, object> _DataCache = new Dictionary<string, object>();

		public CommandExpression( string command )
		{
			command = command.Substring( 1, command.Length - 2 );

			var commandAndParams = command.TokenizeString(':', StringSplitOptions.None);

			CommandData = GetCommands().First( x => x.CommandString == commandAndParams.First() );

			Parameters = string.Join( ":", commandAndParams.Skip( 1 ) );

		}

		public int SetExpression( string baseString, int beginIndex )
		{
			var formattedStartCommand = string.Format( "{{{0}", CommandData.CommandString );
			if( !CommandData.HasEndCommand )
			{
				Expression = string.Empty;
				return beginIndex -1;
			}
			var formattedEndCommand = string.Format( "{{{0}}}", CommandData.EndCommand );
			var endIndex = baseString.IndexOf( formattedEndCommand, beginIndex );

			if( endIndex < 0 )
			{
				throw new Exception();
			}

			var nextCommand = baseString.IndexOf( formattedStartCommand, endIndex + 1 );

			while( nextCommand != -1 && nextCommand > endIndex )
			{
				var newStart = FindNextStartBracket( baseString, nextCommand );
				if( newStart < 0 )
				{
					throw new Exception();
				}
				endIndex = baseString.IndexOf( formattedEndCommand, newStart );
				if( endIndex < 0 )
				{
					throw new Exception();
				}

				nextCommand = baseString.IndexOf( formattedStartCommand, endIndex + 1 );
			}
			Expression = baseString.Substring( beginIndex, endIndex - beginIndex );

			endIndex = FindNextEndBracket( baseString, endIndex );

			return endIndex + formattedEndCommand.Length - 1;

		}

		private static int FindNextEndBracket( string format, int startIndex )
		{
			int endBraceIndex = format.IndexOf( '{', startIndex );
			if( endBraceIndex == -1 )
			{
				return endBraceIndex;
			}
			//start peeking ahead until there are no more braces...
			// }}}}
			int braceCount = 0;
			for( int i = endBraceIndex + 1; i < format.Length; i++ )
			{
				if( format[i] == '}' )
				{
					braceCount++;
				}
				else
				{
					break;
				}
			}
			if( braceCount % 2 == 1 )
			{
				return FindNextEndBracket( format, endBraceIndex + braceCount + 1 );
			}

			return endBraceIndex;
		}

		private int FindNextStartBracket( string format, int startIndex )
		{
			int index = format.IndexOf( '{', startIndex );
			if( index == -1 )
			{
				return index;
			}

			//peek ahead.
			if( index + 1 < format.Length )
			{
				char nextChar = format[index + 1];
				if( nextChar == '{' )
				{
					return FindNextStartBracket( format, index + 2 );
				}
			}

			return index;
		}

		public string Eval( Cache.TemplateProcessor cache, params object[] objs )
		{
			if( ParsedExpressions == null )
				ParsedExpressions = cache.ExpressionCache.GetOrAdd( Expression, this );

			var evaluatedExpression = CommandData.ExecuteCommand( this, cache, objs );

			return evaluatedExpression;
		}


		internal void SetParameter( string key, object obj )
		{
			if( _DataCache.ContainsKey( key ) )
			{
				_DataCache[key] = obj;
			}
			else
			{
				_DataCache.Add( key, obj );
			}
		}

		internal object GetParameter( string key )
		{
			if( _DataCache.ContainsKey( key ) )
			{
				return _DataCache[key];
			}

			return null;
		}


		internal static IEnumerable<Command> GetCommands()
		{
			yield return new RepeatCommand();
			yield return new IndexCommand();
			yield return new CurrentCommand();
			yield return new ParentCommand();
		}

		internal class Command
		{
			public string CommandString { get; set; }

			public string EndCommand { get; set; }

			private Func<CommandExpression, TemplateProcessor, object[],  string> _ExecuteCommand = ( x, y, z ) => string.Empty;
			public Func<CommandExpression, TemplateProcessor, object[], string> ExecuteCommand
			{
				get
				{
					return _ExecuteCommand;
				}
				set
				{
					_ExecuteCommand = value;
				}
			}

			public bool HasEndCommand { get; set; }

			private Dictionary<string, object> _Properties = new Dictionary<string, object>();

			public Dictionary<string, object> Properties
			{
				get { return _Properties; }
				set { _Properties = value; }
			}

			public void Reset()
			{
				Properties = new Dictionary<string, object>();
			}

			public void SetParameter( string key, object obj )
			{
				if( Properties.ContainsKey( key ) )
				{
					Properties[key] = obj;
				}
				else
				{
					Properties.Add( key, obj );
				}
			}

			public object GetParameter( string key )
			{
				if( Properties.ContainsKey( key ) )
				{
					return Properties[key];
				}

				return null;
			}

		}
	}
}
