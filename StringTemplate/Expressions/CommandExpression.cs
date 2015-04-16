using StringTemplate.Algorithms;
using StringTemplate.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace StringTemplate
{
	public class CommandExpression : ITextExpression
	{
		public static char CommandPrefix = '$';

		public string Expression { get; private set; }

		public string Parameters { get; private set; }

		private Command CommandData { get; set; }

		private IEnumerable<ITextExpression> _ParsedExpressions { get; set; }

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
			if( _ParsedExpressions == null )
				_ParsedExpressions = cache.ExpressionCache.GetOrAdd( Expression, this );

			var evaluatedExpression = CommandData.ExecuteCommand( this, cache, objs );

			return evaluatedExpression;
		}


		private void SetParameter( string key, object obj )
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

		private object GetParameter( string key )
		{
			if( _DataCache.ContainsKey( key ) )
			{
				return _DataCache[key];
			}

			return null;
		}


		private static IEnumerable<Command> GetCommands()
		{
			yield return new Command
								{
									CommandString = CommandPrefix + "repeat",
									EndCommand = CommandPrefix + "repeat:" + CommandPrefix + "end",
									HasEndCommand = true,
									ExecuteCommand = ( parent, cache, objs ) =>
									{
										var splitParams = parent.Parameters.TokenizeString(':', StringSplitOptions.None);
										var param = splitParams.First();

										int? iterations = (int?) parent.GetParameter( "Iterations" );

										Func<object, object> enumGet = (Func<object, object>) parent.GetParameter( "Enumerator" );

										int? objIndex = (int?) parent.GetParameter( "ObjectIndex" );

										string joinCharacter = ((string)parent.GetParameter("Join")) ?? string.Empty;

										if( iterations == null && enumGet == null )
										{
											
											joinCharacter = splitParams.Skip(1).FirstOrDefault() ?? string.Empty;

											parent.SetParameter("Join", joinCharacter);

											if( param[0] == CommandPrefix )
											{
												param = param.Substring( 1 );
												iterations = int.Parse( param );

												parent.SetParameter( "Iterations", iterations );
											}
											else
											{
												var exp = new FormatExpression( string.Format( "{{{0}}}", param ) ).GetFunctionFromCache( cache, objs );

												var splitParam = param.TokenizeString();

												var index = 0;

												int.TryParse( splitParam.First(), out index );

												var checkObj = exp( objs[index] );

												if( checkObj is IEnumerable )
												{
													parent.SetParameter( "Enumerator", exp );
													enumGet = exp;
													parent.SetParameter( "ObjectIndex", index );
													objIndex = index;
												}
												else
												{
													iterations = (int) checkObj;
													parent.SetParameter( "Iterations", iterations );
												}
											}
										}

										
										List<string> evaluatedExpressions = new List<string>();
										parent.CommandData.Reset();
										if( iterations != null )
										{
											for( int i = 0; i < iterations; i++ )
											{
												parent.CommandData.SetParameter( "index", i );

												evaluatedExpressions.Add(string.Join( string.Empty, parent._ParsedExpressions.Select( x => x.Eval( cache, objs ) ) ) );

											}
										}
										else
										{
											var count = 0;
											foreach( var obj in ( (IEnumerable) enumGet( objs[objIndex.Value] ) ) )
											{
												parent.CommandData.SetParameter( "index", count );
												parent.CommandData.SetParameter( "current", obj );
												evaluatedExpressions.Add(string.Join( string.Empty, parent._ParsedExpressions.Select( x => x.Eval( cache, objs ) ) ));

												count++;
											}
										}

										return string.Join(joinCharacter, evaluatedExpressions);
									}
								};
			yield return new Command
								{
									CommandString = CommandPrefix + "index",
									HasEndCommand = false,
									ExecuteCommand = ( parent, cache, objs ) =>
									{
										var p = parent.Parent;

										while( !( p is CommandExpression ) )
										{
											p = p.Parent;
										}

										var cmd = p as CommandExpression;

										return cmd.CommandData.GetParameter( "index" ).ToString();

									}
								};
			yield return new Command
								{
									CommandString = CommandPrefix + "current",
									HasEndCommand = false,
									ExecuteCommand = ( parent, cache, objs ) =>
									{
										var p = parent.Parent;

										while( !( p is CommandExpression ) )
										{
											p = p.Parent;
										}

										var cmd = p as CommandExpression;

										return cmd.CommandData.GetParameter( "current" ).ToString();

									}
								};
		}

		private class Command
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
