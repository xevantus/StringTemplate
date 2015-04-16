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
{$repeat:iteration variable:join string}Repeated Text Or {Interpolated} Variables{$repeat:$end}
```
### Variables
Variables within the Repeat directive are seperated by a ":" character, as such, any colons used within parameters will be seen as a variable end.
#### Iteration Variable
The Repeat directive accepts three different forms for an iteration variable.


##### Static Int
```
{$repeat:$10}
```
Static ints require the "$" character, or will be interpolated as the index of an object to interpolate.
##### Interpolated Object
```
{$repeat:Count}, {$repeat:0.Count} 
```
The same interpolation rules apply to these variables that apply to normal interpolation. 
##### Interpolate IEnumerable
```
{$repeat:Array}
```
If the interpolated variable used implements IEnumerable, the repeat loop will treat the directive as a "for-each" loop

#### Join String (Optional)
```
{$repeat:$10:,}
```
The Join String will be used to join the iterations together when the string is re-compiled.
### Repeater Sub Directives
#### Index
String Template supports the index directive.  Within nested repeat statements, this will only get the inner most repeat's index.

Syntax:
```
{$repeat:$10}{$index}{$repeat:$end}
```
#### Current
A repeater using an Interpolated Ienumerable can use the Current directive to access the current object in the repeater.

Syntax:
```
{$repeat:$10}{$current}{$repeat:$end}
```
### Nested Repeats
String Template parses the interior of a repeat as if it were a new string to be interpolated.  Because of this, repeats can be nested indefinately within other repeats.
```
{$repeat:$10}{$current}{$repeat:$10}{$current}{$repeat:$end}{$repeat:$end}
```
Note: $current and $index will interpolate to the most inner repeat they are aware of.  The system does not currently support using the variables of a higher level repeat.

## Caching
The String Template Library caches both parsed expressions and Interpolation Getter functions in order to speed up performance over multiple calls.  Any given string will only be parsed into expressions the first time it is encountered.  

The same process is followed with Property getters.  String Template uses reflection to get the Property or Field Get methods.  Since reflection is extremely expensive, these getters are cached at both a Type and request level. For instance, consider the following classes:
```c#
class TestClass1
{
	public TestClass2 Test1;

	public string Test2 { get; set; }
}
class TestClass2
{
	public int Test1;
}
...
StringTemplate.Format( "{0.Test1.Test1} {Test2} {1} {2}", testClass, "test", 1 );
```
The Cache after this request would contain the following keys, with their associated getter functions :

| Type | Request |
|------|---------|
| TestClass1 | Test2 |
| TestClass1 | Test1.Test1 |
| TestClass2 | Test1 |

This allows lookup times to be independent of interpolate chain length.
