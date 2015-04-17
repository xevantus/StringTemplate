using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTemplate.Expressions.Commands
{
	internal interface IParentedCommand
	{
		int ParentLevel { get; set; }
	}
}
