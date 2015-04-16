# StringTemplate
String interpolation and data templating for .NET

# How it works
StringTemplate works almost exactly the same as the standard "string.format()" function.  It will even accept the same formatting.
```c#
//Outputs 1 2
var output = StringTemplate.Format( "{0} {1}", "1", 2 );
```
StringTemplate.Format is also available as the string extension method "SFormat"
```c#
//Outputs 1 2
var output = "{0} {1}".SFormat( "1", 2 );
```
## Object Interpolation
StringTemplate's standard operating mode will interpolate a string based on the object provided.
```c#
var testClass = new TestClass1();

testClass.Test1 = new TestClass2();
testClass.Test1.Test1 = 10;
testClass.Test2 = "testString";

//Outputs 10 testString
var output = "{Test1.Test1} {Test2}".SFormat( testClass );
```
## Mixed Interpolation
StringTemplate is also capable of mising indexed parameters with interpolated parameters.
```c#
//Outputs 10 testString test 1
var output = "{0.Test1.Test1} {Test2} {1} {2}".SFormat( testClass, "test", 1 );
```
## Repeater Directive
String Template supports a repeater directive.
Syntax:
```
{$repeat:(Interpolated Int|Interpolated IEnumerable|$int):join string} {$repeat:$end}
```
### Repeater Sub Directives
#### Index
String Template supports the index directive.  Within nested repeat statements, this will only get the inner most repeat's index.
Syntax:
```
{$index}
```
#### Current
A repeater using an Interpolated Ienumerable can use the Current directive to access the current object in the repeater
Syntax:
```
{$current}
```
