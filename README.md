# Dangl.Calculator

[![Build Status](https://jenkins.dangl.me/buildStatus/icon?job=GeorgDangl%2FDangl.Calculator%2Fdev)](https://jenkins.dangl.me/job/GeorgDangl/job/Dangl.Calculator/job/dev/)
[![NuGet](https://img.shields.io/nuget/v/Dangl.Calculator.svg)](https://www.nuget.org/packages/Dangl.Calculator)
[![MyGet](https://img.shields.io/myget/dangl/v/Dangl.Calculator.svg)]()

[Online Documentation](https://docs.dangl-it.com/Projects/Dangl.Calculator)  
[Changelog](./CHANGELOG.md)  

This calculator is using the [ANTLR4 C# target](https://github.com/tunnelvisionlabs/antlr4cs)
to calculate results from formulas that are passed in as string.

Whenever a calculation is performed, a `CalculationResult` is returned with the following properties:

| Property      | Type    |                                                                                             |
|---------------|---------|---------------------------------------------------------------------------------------------|
| IsValid       | bool    | `true` if the formula could be parsed and calculated, else `false`                          |
| ErrorPosition | int     | Zero-based character position in the formula, or -1 when no error position is available   |
| ErrorMessage  | string  | Syntax, substitution or numeric evaluation error message, else null                         |
| Result        | double  | `NaN` for invalid formulas, else the actual result                                          |

You can find the TypeScript version here: https://github.com/GeorgDangl/antlr-calculator

## Installation

Install it via NuGet: `Dangl.Calculator`

CI builds are available via MyGet.

    https://www.myget.org/F/dangl/api/v3/index.json

## Compatibility

This project targets `netstandard2.0` and `net462`. The latest .NET version is also supported via a dedicated target framework to leverage features not available in .NET Standard.

## Project Configuration

If this project is consumed in a project using the full .Net framework with a newer version of
`Antlr4.Runtime`, the necessary AssemblyBindingRedirects are not automatically generated with the current
dotnet CLI tooling. This is scheduled to be fixed with the 2.0 release. In the meantime, the following should
be added to the consumers `csproj`:

  ```xml
<PropertyGroup Condition=" '$(TargetFramework)' == 'net462' ">
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

## Decimal Calculations

Use `Calculator.CalculateDecimal(...)` for decimal arithmetic without conversions through `double`.
`Calculate(...)` delegates to the same decimal evaluator and converts only the final result to `double`.
Its public signatures remain unchanged, but evaluation now follows decimal range, precision and domain rules.
For example, `Calculate("roundk(5,475;2)")` returns `5.48`, and `Calculate("0.1+0.2-0.3")` returns zero.
Invalid calculations, including division by zero and overflow, return `IsValid == false` with `Result == NaN`
and error diagnostics; they no longer return infinity or a valid `NaN` result.
Error positions are absolute zero-based character indices, including in multiline formulas.
Signed zero is not retained during evaluation, so `arctan2(0;-0)` evaluates to zero.

```csharp
var calculation = Calculator.CalculateDecimal("0.1+0.2-0.3");
// calculation.IsValid == true
// calculation.Result == 0m

var substituted = Calculator.CalculateDecimal("#price*#quantity", name =>
    name == "#price" ? 0.1m : name == "#quantity" ? 3m : (decimal?)null);
// substituted.Result == 0.3m
```

`CalculateDecimal` supports the same formula syntax and functions as `Calculate`. Its overloads accept
`Func<string, decimal?>` substitution resolvers and `Func<RangeSubstitution, decimal?>` range resolvers.
Use `DecimalCalculatorOptions` to supply both resolvers and configure
`DetectUnaryMinusInPowersAtStartOfFormula`, which defaults to `false` just as in `CalculatorOptions`.
Omitted resolvers and callbacks returning `null` produce an invalid result when the formula requires them.
Exceptions thrown by your callbacks propagate to the caller.

The returned `DecimalCalculationResult` has the following properties:

| Property | Type | Description |
|----------|------|-------------|
| `IsValid` | `bool` | Whether the formula was successfully calculated |
| `Result` | `decimal?` | The calculated value, or `null` for an invalid calculation |
| `ErrorPosition` | `int` | Zero-based character position of the error, or `-1` when none is available |
| `ErrorMessage` | `string` | Syntax, substitution or numeric error details; `null` for a valid calculation |

Decimal provides approximately 28-29 significant digits, a maximum magnitude of
`79,228,162,514,264,337,593,543,950,335`, and a smallest positive value of `0.0000000000000000000000000001`.
It is not arbitrary-precision arithmetic: literals with excess precision and arithmetic results are
rounded to the available decimal precision, and very small values can round to zero.
`ROUND` and `ROUNDK` use midpoint-to-even rounding; `ROUNDK` accepts 0 through 28 decimal places.
Division by zero, overflow and functions outside their real-valued domains return `IsValid == false`
with a `null` result, rather than `NaN` or infinity.

Scientific functions are implemented without additional NuGet dependencies. They use the built-in
`BigInteger` type for base-10 fixed-point calculations with guard digits, then round to a decimal.
They remain numerical approximations, not symbolic or exact calculations.
Trigonometric functions use radians; `PI` and `E` are decimal constants.
Fractional powers require a nonnegative base; the nth-root operators additionally support negative
values when their degree is an odd integer.

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
`MIN '(' expression (';' expression)* ')'` | Minimum |
`MAX '(' expression (';' expression)* ')'` | Maximum |
`NUMBER	`                                  | Single integer or float number        |
`'(' expression ')'`                       | Expression within parentheses         |
`PI '()'?`                                 | Mathematical constant pi = 3,141593   |
`expression E+ expression`                 | Exponent, e.g. 10e+43                 |
`expression E- expression`                 | Inverted Exponent, e.g. 10e-43        |
`EULER`                                    | Mathematical constant e = 2,718282    |
`'-' expression`                           | Unary minus sign (negative numbers)   |
`'+' expression`                           | Unary plus sign (positive numbers)    |
`'(' expression ')' expression`               | Expressions without multiplication sign, e.g. `2(3)` -> `2*(3)` |
`expression '(' expression ')'`              | Expressions without multiplication sign, e.g. `2(3)` -> `2*(3)` |

_expression_ may be any expression as functions can be nested. Example: `DEG(2*PI)` or `LOG(10^3)`.

Formulas can be case invariant, e.g. `SIN`, `sin` and `siN` are all considered the same.

An optional equals sign `=` at the end of the function is allowed.

## Comments in Formulas

Comments in Formulas are supported by encapsulating them either in `/*...*/`, `'...'` or `"..."` quote styles. Examples:

`4/*Length*/*3/*Width*/` resolves to `12`

`4'Length'*3'Width'` resolves to `12`

`4"Length"*3"Width"` resolves to `12`

## Substitutions

The calculator can be called with an overload that accepts a callback function for substitution values. For example, take the following formula:  
`1,2*#Z4+3`  
Here, `#Z4` is a _substitution_, which is a placeholder that can be externally supplied. Let's say you want to resolve `#Z4` to the value three, you could
make this simple call:

```csharp
var formula = "1,2*#Z4+3";
var result = Calculator.Calculate(formula, substitution =>
{
    if (substitution == "#Z4")
    {
        return 3;
    }

    return null;
});
```

The callback is in the form of a `Func<string, double?>`, and it will be called for every substitution found in the formula. Multiple substitutions are supported.
If duplicates in substitutions are present, the calculator will request each one individually. If a substitution resolves to `null`, the formula is considered invalid.
Double callback values are formatted with invariant `G15` and parsed as decimal, preserving 15 significant decimal digits.
Non-finite values or values outside the decimal range make the calculation invalid. Very small values can round to zero.
Exceptions thrown by callbacks propagate to the caller. Use `CalculateDecimal` with decimal callbacks to retain more decimal precision.

Substitutions must always start with the `#` character and can then have the following characters: `[a-z] | [A-Z] | [äÄöÖüÜ] | [0-9]`

## Ranges

Similar to `Substitutions`, the calculator also can handle `Ranges`. They look like two substitutions joined by two points, e.g. `#1..#5`. A separate range resolver can be supplied to the calculator as a callback function, so that you can plug in your own logic to resolve ranges when being given a `Start` and `End` substitution.
Double range callback values use the same 15-significant-digit decimal conversion as substitutions.

## Trailing comments

Formulas may be terminated with a semicolon `;` at the end, followed by extra input that is not evaluated. This is useful when, instead of regular comments, you
just want to attach some trailing formation at the end of a formula. For example, the following formula:  
`1 + 3; As per our counting`  
Would just evaluate the `1 + 3` portion and return a valid result with the value `4`, ignoring the trailing semicolon and all input that follows.

## Assembly Strong Naming & Usage in Signed Applications

This module produces strong named assemblies when compiled. When consumers of this package require strongly named assemblies, for example when they
themselves are signed, the outputs should work as-is.
The key file to create the strong name is adjacent to the `csproj` file in the root of the source project. Please note that this does not increase
security or provide tamper-proof binaries, as the key is available in the source code per 
[Microsoft guidelines](https://msdn.microsoft.com/en-us/library/wd40t7ad(v=vs.110).aspx)

---

[MIT Licence](Licence.md)