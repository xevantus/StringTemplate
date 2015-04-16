# StringTemplate
String interpolation and data templating for .NET

# How it works
StringTemplate works almost exactly the same as the standard "string.format()" function.  It will even accept the same formatting.
```c#
var output = StringTemplate.Format( "{0} {1}", "1", 2 );
```
StringTemplate.Format is also available as the string extension method "SFormat"
```c#
var output = "{0} {1}".SFormat( "1", 2 );
```
## Object Interpolation
StringTemplate's standard operating mode will interpolate a string based on the object provided.
The following code will produce the string "10 testString".
```c#
var testClass = new TestClass1();

			testClass.Test1 = new TestClass2();
			testClass.Test1.Test1 = 10;
			testClass.Test2 = "testString";

			var output = "{Test1.Test1} {Test2}".SFormat( testClass );
```

