# Changelog

All notable changes to **Dangl.Calculator** are documented here.

## v2.0.0:
- The ANTLR dependencies were updated from _Antlr.Runtime_ to _Antlr.Runtime.Standard_. The new package is the now official ANTLR runtime package and includes many performance improvements
- Compatibility for `net40` and `netstandard1.1` was dropped, the lowest supported frameworks now are ´net45` and `netstandard2.0`

## v1.5.0:
- Added support for `Min` and `Max` formulas
- Dropped tests for `netcoreapp2.1` and added tests for `net5.0` and `net6.0`

## v1.4.0:
- The calculator now supports trailing comments in a formula, separated by a semicolon `;` at the end of the actual formula input. For example, `1 + 2; Hello World!` now just evaluates `1 + 2` and ignores everything after the semicolon `;`

## v1.3.1:
- Updates to README

## v1.3.0:
- Add support for substitutions in formulas

## v1.2.1:
- Add `net40` as target framework
- Drop tests for `netcoreapp2.2` and add tests for `netcoreapp3.1`

## v1.2.0
- The generated assemblies now have a strong name. This is a breaking change of the binary API and will require recompilation on all systems that consume this package. The strong name of the generated assembly allows compatibility with other, signed tools. Please note that this does not increase security or provide tamper-proof binaries, as the key is available in the source code per [Microsoft guidelines](https://msdn.microsoft.com/en-us/library/wd40t7ad(v=vs.110).aspx)

## v1.1.5:
- CI tests are now also run on Linux
- Added capability to detect missing multiplication signs, e.g. the formula `3pi` is now recognized as `3*pi` or `2(3)` as `2*(3)`

## v1.1.4:
- Added support for formulas ending with an equals sign `=`

## v1.1.3:
- Dropped tests for `netcoreapp2.0`, added tests for `netcoreapp2.2`
- Update of dependencies

## v1.1.2
- Bugfix: Fixed operator precedence for exponential expression, e.g. `1e+2*2 == 200`

## v1.1.1
- Bugfix: Unary minus signs were not always evaluated to the nearest matching expression, e.g. `-3+5` was evaluated as `-(3+5)`. See [GitHub Issue #1](https://github.com/GeorgDangl/Dangl.Calculator/issues/1)

## v1.1.0
- Dropped test support for .NET Core 1.0 and 1.1
- Updated Antlr4 to latest stable version

## v1.0.6
- Add `netstandard2.0` target
- Switch build system to NUKE

## v1.0.5
- Downgrade to netstandard1.1 and net45 for broader compatibility
    
## v1.0.4
- Update ANTLR to latest version
      
## v1.0.3
- Target NETStandard 1.3
