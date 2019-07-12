# Changelog

All notable changes to **Dangl.Calculator** are documented here.

## v1.1.5:
- CI tests are now also run on Linux

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
