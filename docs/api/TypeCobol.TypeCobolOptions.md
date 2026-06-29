# TypeCobol.Compiler.Directives.TypeCobolOptions

Parser and compiler configuration object. It extends IBM Enterprise COBOL compiler options and implements TypeCobol check options.

Declared in `TypeCobol/Compiler/Directives/TypeCobolOptions.cs`.

## Agent-Relevant Members

| Member | Meaning |
| --- | --- |
| `HaltOnMissingCopy` | Stops before semantic phases when missing COPYBOOKs are detected. |
| `ExecToStep` | Maximum compiler pipeline step. Defaults to `ExecutionStep.Generate`. |
| `UseAntlrProgramParsing` | Selects ANTLR program parsing behavior where supported. |
| `IsCobolLanguage` | Set true for strict COBOL source rather than TypeCobol. |
| `OptimizeWhitespaceScanning` | Disables `SpaceSeparator` token creation for speed and memory. Disable when code generation requires whitespace tokens. |
| `EnableSqlParsing` | Parses SQL in `EXEC SQL ... END-EXEC` blocks. |
| `CheckEndAlignment` | Configures end-statement alignment checking. |
| `CheckPerformPrematureExits` | Configures CFG-based premature PERFORM exit checking. |
| `CheckPerformThruOrder` | Configures CFG-based PERFORM THRU order checking. |
| `CheckRecursivePerforms` | Configures recursive PERFORM checking. |
| `CheckCodeElementMixedDebugType` | Configures mixed debug/non-debug code element checking. |

## Guidance

For AST export, use options that allow parsing to continue:

```csharp
var options = new TypeCobolOptions
{
    IsCobolLanguage = true,
    EnableSqlParsing = false,
    HaltOnMissingCopy = false,
    OptimizeWhitespaceScanning = true
};
```

If you call static `Parser.Parse(...)`, expect `ExecToStep` to be overwritten with `ExecutionStep.Generate`.

