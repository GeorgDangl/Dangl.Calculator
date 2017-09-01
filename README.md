# Dangl.Calculator

[![Build Status](https://jenkins.dangl.me/buildStatus/icon?job=Dangl.Calculator.Tests)](https://jenkins.dangl.me/job/Dangl.Calculator.Tests/)
[![NuGet](https://img.shields.io/nuget/v/Dangl.Calculator.svg)](https://www.nuget.org/packages/Dangl.Calculator)
[![MyGet](https://img.shields.io/myget/dangl/v/Dangl.Calculator.svg)]()

This calculator is using the [ANTLR4 C# target](https://github.com/tunnelvisionlabs/antlr4cs)
to calculate results from formulas that are passed in as string.

Whenever a calculation is performed, a `CalculationResult` is returned with the following properties:

| Property      | Type    |                                                                                             |
|---------------|---------|---------------------------------------------------------------------------------------------|
| IsValid       | bool    | `true` if the formula could be parsed and calculated, else `false`                          |
| ErrorPosition | int     | Position of the offending symbol in the line, 0 based index, for invalid results, else null |
| ErrorMessage  | string  | ANTLR error message for invalid formulas, else null                                         |
| Result        | double  | `NaN` for invalid formulas, else the actual result                                          |

## Installation

Install it via NuGet: `Dangl.Calculator`

CI builds are available via MyGet.

    https://www.myget.org/F/dangl/api/v3/index.json

## Project Configuration

If this project is consumed in a project using the full .Net framework with a newer version of
`Antlr4.Runtime`, the necessary AssemblyBindingRedirects are not automatically generated with the current
dotnet CLI tooling. This is scheduled to be fixed with the 2.0 release. In the meantime, the following should
be added to the consumers `csproj`:

  ```xml
<PropertyGroup Condition=" '$(TargetFramework)' == 'net461' ">
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
    <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
</PropertyGroup>
```

The `Condition=" '$(TargetFramework)' == 'net461' "` attribute may be changed as necessary or removed.

## Example

``` csharp
using Dangl.Calculator;

public void Example()
{
    var formula = "5+5";
    var calculation = Calculator.Calculate(formula);

    Console.WriteLine(calculation.Result);
    Console.WriteLine(calculation.IsValid);

    // 10.0
    // true
}
```
## Supported functions

| Expression                               |                                       |
|------------------------------------------|---------------------------------------|
`FLOOR  expression`                        | Round down to zero accuracy           |
`CEIL  expression`                         | Round up to zero accuracy             |
`ABS  expression`                          | Absolute value                        |
`ROUNDK '(' expression ';' expression ')'` | Round expr_1 with expr_2 accuracy     |
`ROUND  expression`                        | Round with zero accuracy              |
`TRUNC  expression`                        | Trim decimal digits                   |
`SIN  expression`                          | Sinus                                 |
`COS  expression`                          | Cosinus                               |
`TAN  expression`                          | Tangens                               |
`COT  expression`                          | Cotangens	                           |
`SINH  expression`                         | Sinus Hypererbolicus                  |
`COSH  expression`                         | Cosinus Hyperbolicus                  |
`TANH  expression`                         | Tangens Hyperbolicus                  |
`ARCSIN  expression`                       | Inverse Sinus                         |
`ARCCOS  expression`                       | Inverse Cosinus                       |
`ARCTAN  expression`                       | Inverse Tangens                       |
`ARCTAN2 '(' expression ';' expression ')'`| Atan2                                 |
`ARCCOT  expression`                       | Inverse Cotangens                     |
`EXP  expression`                          | e ^ expr                              |
`LN  expression`                           | Logarithm to e                        |
`EEX  expression`                          | 10 ^ expr                             |
`LOG  expression`                          | Logarithm to 10                       |
`RAD  expression`                          | Angle to radians (360° base)          |
`DEG  expression`                          | Radians to angle (360° base)          |
`SQRT expression`                          | Square root                           |
`SQR expression`                           | Square product                        |
`expression op = ('^'\|'**') expression`    | expr_1 to the expr_2 th power         |
`expression (MOD \| '%' ) expression`       | Modulo                                |
`expression DIV expression`                | Whole part of division rest           |
`expression op = ('~'\|'//') expression`    | expr_1 nth root of expr_2             |
`expression op = ('*'\|'/') expression`     | Multiplication or division            |
`expression op = ('+'\|'-') expression`     | Addition or subtraction               |
`NUMBER	`                                  | Single integer or float number        |
`'(' expression ')'`                       | Expression within parentheses         |
`PI '()'?`                                 | Mathematical constant pi = 3,141593   |
`expression E+ expression`                 | Exponent, e.g. 10e+43                 |
`expression E- expression`                 | Inverted Exponent, e.g. 10e-43        |
`EULER`                                    | Mathematical constant e = 2,718282    |
`'-' expression`                           | Unary minus sign (negative numbers)   |
`'+' expression`                           | Unary plus sign (positive numbers)    |

_expression_ may be any expression as functions can be nested. Example: `DEG(2*PI)` or `LOG(10^3)`.

Formulas can be case invariant, e.g. `SIN`, `sin` and `siN` are all considered the same.

## Comments in Formulas

Comments in Formulas are supported by encapsulating them either in `/*...*/`, `'...'` or `"..."` quote styles. Examples:

`4/*Length*/*3/*Width*/` resolves to `12`

`4'Length'*3'Width'` resolves to `12`

`4"Length"*3"Width"` resolves to `12`

---

[MIT Licence](Licence.md)