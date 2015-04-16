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
## Repeat Directive
String Template supports a repeat directive that will iterate up to a given number of times, or iterate through a collection.

Syntax:
```
{$repeat:iteration variable:join string} {$repeat:$end}
```
### Variables
Variables within the Repeat directive are seperated by a ":" character, as such, any colons used within parameters will be seen as a variable end.
#### Iteration Variable
The Repeat directive accepts three different forms for an iteration variable.
|Type | Example | Description |
|--------|------------|----------|
|Static Int | {$repeat:$10}| Static ints require the "$" character, or will be interpolated as the index of an object to interpolate.|
|Interpolated Object| {$repeat:Count}, {$repeat:0.Count} | The same interpolation rules apply to these variables that apply to normal interpolation. |
|Interpolate IEnumerable | {$repeat:Array} | If the interpolated variable used implements IEnumerable, the repeat loop will treat the directive as a "for-each" loop |

### Repeater Sub Directives
#### Index
String Template supports the index directive.  Within nested repeat statements, this will only get the inner most repeat's index.

Syntax:
```
{$index}
```
#### Current
A repeater using an Interpolated Ienumerable can use the Current directive to access the current object in the repeater.

Syntax:
```
{$current}
```
