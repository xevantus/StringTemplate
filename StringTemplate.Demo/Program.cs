using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringTemplate.Cache;
using System.Diagnostics;

namespace StringTemplate.Demo
{
	class Program
	{
		static void Main(string[] args)
		{
			var testString = "{{{Test1.Test1}}}, {Test2}";

			var testClass = new TestClass1();

			testClass.Test1 = new TestClass2();
			testClass.Test1.Test1 = 10;
			testClass.Test2 = "testString";
			var sw = new Stopwatch();
			sw.Start();
			Console.WriteLine(testString.SFormat(testClass));
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
			sw.Reset();
			sw.Start();
			Console.WriteLine(testString.SFormat(testClass));
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
			Console.WriteLine("Clearing Cache");
			StringTemplate.ClearCache();
			sw.Start();
			Console.WriteLine(testString.SFormat(testClass));
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
			sw.Reset();
			sw.Start();
			Console.WriteLine(testString.SFormat(testClass));
			sw.Stop();
			Console.WriteLine(sw.Elapsed);

			var testString2 = "{1} {0.Test1}{{{Test1.Test1}}}, {Test2}";
			Console.WriteLine(testString2.SFormat(testClass, "This is a Test!"));

			sw.Reset();
			sw.Start();
			var testString3 = "[{$repeat:Iterations:,}1{$repeat:$end}]";
			Console.WriteLine(testString3.SFormat(new {Iterations = 1000 }));
			sw.Stop();
			Console.WriteLine(sw.Elapsed);


			//for(int i = 0; i < 100; i++)
			//{
			//	Console.WriteLine("Iteration {0}:", i); 
			//	sw.Start();
			//	Console.WriteLine("\t{0}", testString.SFormat(testClass));
			//	sw.Stop();
			//	Console.WriteLine("\t{0}", sw.Elapsed);
			//	sw.Reset();
			//	sw.Start();
			//	Console.WriteLine("\t{0}", testString.SFormat(testClass));
			//	sw.Stop();
			//	Console.WriteLine("\t{0}", sw.Elapsed);
			//}

			Console.ReadKey();
		}
	}

	public class TestClass1
	{
		public TestClass2 Test1;

		public string Test2 { get; set; }
	}
	public class TestClass2
	{
		public int Test1;
	}
}
