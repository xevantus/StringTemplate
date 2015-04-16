using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StringTemplate.Tests
{
    public class StringTemplateTests
    {
		[Fact]
		public void StringTemplate_RegularStringFormat()
		{
			var output = StringTemplate.Format("{0} {1}", "1", 2);

			Assert.Equal("1 2", output);
		}

		[Fact]
		public void StringTemplate_RegularStringFormat_Extension()
		{
			var output = "{0} {1}".SFormat("1", 2);

			Assert.Equal("1 2", output);
		}
		[Fact]
		public void StringTemplate_NamedPath()
		{
			var testClass = new TestClass1();

			testClass.Test1 = new TestClass2();
			testClass.Test1.Test1 = 10;
			testClass.Test2 = "testString";

			var output = StringTemplate.Format("{Test1.Test1} {Test2}", testClass);

			Assert.Equal("10 testString", output);
		}

		[Fact]
		public void StringTemplate_NamedPath_Extension()
		{
			var testClass = new TestClass1();

			testClass.Test1 = new TestClass2();
			testClass.Test1.Test1 = 10;
			testClass.Test2 = "testString";

			var output = "{Test1.Test1} {Test2}".SFormat(testClass);

			Assert.Equal("10 testString", output);
		}

		[Fact]
		public void StringTemplate_MixedIndexedAndNamedPath_Extension()
		{
			var testClass = new TestClass1();

			testClass.Test1 = new TestClass2();
			testClass.Test1.Test1 = 10;
			testClass.Test2 = "testString";

			var output = "{0.Test1.Test1} {Test2} {1} {2}".SFormat(testClass, "test", 1);

			Assert.Equal("10 testString test 1", output);
		}

		[Fact]
		public void StringTemplate_MixedIndexedAndNamedPath()
		{
			var testClass = new TestClass1();

			testClass.Test1 = new TestClass2();
			testClass.Test1.Test1 = 10;
			testClass.Test2 = "testString";

			var output = StringTemplate.Format("{0.Test1.Test1} {Test2} {1} {2}", testClass, "test", 1);

			Assert.Equal("10 testString test 1", output);
		}
		[Fact]
		public void StringTemplate_Repeater_StaticIterations()
		{
			var output = StringTemplate.Format("{$repeat:$5}1{$repeat:$end}");

			Assert.Equal("11111", output);
		}
		[Fact]
		public void StringTemplate_Repeater_VariableIterations()
		{
			var output = StringTemplate.Format("{$repeat:Iterations}1{$repeat:$end}", new { Iterations = 5 });

			Assert.Equal("11111", output);
		}

		[Fact]
		public void StringTemplate_Repeater_IndexSubCommand()
		{
			var output = StringTemplate.Format("{$repeat:Iterations}{$index}{$repeat:$end}", new { Iterations = 5 });

			Assert.Equal("01234", output);
		}

		[Fact]
		public void StringTemplate_Repeater_Iterator()
		{
			var output = StringTemplate.Format("{$repeat:Iteratior}{$index}{$repeat:$end}", new { Iteratior = new int[5] });

			Assert.Equal("01234", output);
		}
		[Fact]
		public void StringTemplate_Repeater_Join()
		{
			var output = StringTemplate.Format("{$repeat:Iteratior:,}{$index}{$repeat:$end}", new { Iteratior = new int[5] });

			Assert.Equal("0,1,2,3,4", output);
		}

		[Fact]
		public void StringTemplate_Repeater_Iterator_Current()
		{
			var output = StringTemplate.Format("{$repeat:Iteratior}{$current}{$repeat:$end}", new { Iteratior = new int[] { 0, 1, 2, 3, 4 } });

			Assert.Equal("01234", output);
		}
		[Fact]
		public void StringTemplate_Repeater_NestedIterator_Index()
		{
			var output = StringTemplate.Format("{$repeat:Iteratior}{$index} {$repeat:Iteratior}{$index}{$repeat:$end} {$repeat:$end}", new { Iteratior = new int[5] });

			Assert.Equal("0 01234 1 01234 2 01234 3 01234 4 01234 ", output);
		}
		[Fact]
		public void StringTemplate_Repeater_NestedIterator_Current()
		{
			var output = StringTemplate.Format("{$repeat:Iteratior}{$current} {$repeat:Iteratior}{$current}{$repeat:$end} {$repeat:$end}", new { Iteratior = new int[] { 1, 2, 3, 4, 5 } });

			Assert.Equal("1 12345 2 12345 3 12345 4 12345 5 12345 ", output);
		}
		[Fact]
		public void StringTemplate_DynamicObjects()
		{
			var expando = new ExpandoObject();

			var d = (IDictionary<string, object>)expando;

			var testClass = new TestClass1();

			testClass.Test1 = new TestClass2();
			testClass.Test1.Test1 = 10;
			testClass.Test2 = "testString";

			d["Test1"] = 1;
			d["Test2"] = testClass;

			var output = StringTemplate.Format("{Test1} {Test2.Test2}", expando);

			Assert.Equal("1 testString", output);
		}
		[Fact]
		public void StringTemplate_Dictionary()
		{
			var d = new Dictionary<string, object>();

			var testClass = new TestClass1();

			testClass.Test1 = new TestClass2();
			testClass.Test1.Test1 = 10;
			testClass.Test2 = "testString";

			d["Test1"] = 1;
			d["Test2"] = testClass;

			var output = StringTemplate.Format("{Test1} {Test2.Test2}", d);

			Assert.Equal("1 testString", output);
		}



		private class TestClass1
		{
			public TestClass2 Test1;

			public string Test2 { get; set; }
		}
		private class TestClass2
		{
			public int Test1;
		}
    }
}
