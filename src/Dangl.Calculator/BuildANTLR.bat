@echo off
SET JAVACOMMAND=java
where %JAVACOMMAND%
if %ERRORLEVEL% NEQ 0 (
	SET JAVACOMMAND=%UserProfile%/.nuget/packages/Antlr4.CodeGenerator/4.5.4-beta001/tools/ikvm.exe
)
%JAVACOMMAND% -jar %UserProfile%/.nuget/packages/Antlr4.CodeGenerator/4.5.4-beta001/tools/antlr4-csharp-4.5.4-SNAPSHOT-complete.jar Calculator.g4 -package Dangl.Calculator.Generated -o Generated -Dlanguage=CSharp_v4_5 -visitor -no-listener