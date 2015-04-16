using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StringTemplate.Cache;

namespace StringTemplate.Reflection
{
	internal class DefaultReflectionProvider : IReflectionProvider
	{
		
		public Type Type { get; set; }

		public ObjectGetCache Cache { get;set; }

		public Func<object, object> GetGetter(string path )
		{
			var refPieces = path.TokenizeString();

			if (refPieces.Any())
			{
				var reflectionInfo = GetRootReflectionTranlator(refPieces.First());

				if (refPieces.Count() == 1)
				{
					return reflectionInfo.GetGetter();

				}
				else if (refPieces.Count() > 1)
				{
					var subReference = string.Join(".", refPieces.Skip(1));

					var subFunction = Cache.GetOrAdd(reflectionInfo.GetMemberType(), subReference);

					return (obj) => subFunction(reflectionInfo.GetGetter()(obj));

				}
			}
			throw new Exception();
		}

		private IMemberTranslator GetRootReflectionTranlator(string path)
		{
			var members = Type.GetMember(path);
			
			IMemberTranslator reflectionInfo = null;

			if (members.Any())
			{
				return reflectionInfo = members.First().CreateTranslator();

			}

			throw new Exception();
					
		}
	}
}
