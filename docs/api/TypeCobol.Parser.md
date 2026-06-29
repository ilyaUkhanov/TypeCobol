# TypeCobol.Parser

High-level facade for parsing COBOL or TypeCobol files.

Declared in `TypeCobol/Parser.cs`.

## Key Members

| Member | Meaning |
| --- | --- |
| `Version` | Assembly informational version. Include it in exported metadata. |
| `MissingCopys` | COPYBOOK names unresolved during the most recent parse. |
| `CustomSymbols` | Optional symbol table used for name and type resolution. Most agents should leave this unset. |
| `Parser()` | Creates an empty parser facade. |
| `Parser(SymbolTable)` | Creates a parser with external symbols. |
| `Init(...)` | Creates the compilation project, registers source/copy paths, and creates a `FileCompiler`. |
| `Parse(string path)` | Runs compilation for a path previously registered with `Init(...)`. |
| `Results` | Returns the active `CompilationUnit`. |
| `Parse(...) static` | One-shot convenience API. Initializes, parses, and returns a `Parser`. |

## `Init(...)`

Signature:

```csharp
public void Init(
    string path,
    bool isCopy,
    TypeCobolOptions options,
    DocumentFormat format = null,
    IList<string> copies = null,
    IAnalyzerProvider analyzerProvider = null)
```

Creates a `CompilationProject`, registers copy directories, and creates a `FileCompiler` for the file name. If `format` is null, the current implementation uses `DocumentFormat.FreeUTF8Format`.

## `Parse(string path)`

Runs `FileCompiler.CompileOnce()` for a previously initialized path, updates `MissingCopys`, and makes `Results` available.

Preconditions:

- `Init(path, ...)` must already have been called.
- The chosen `TypeCobolOptions.ExecToStep` must reach the snapshots you want.

Side effects:

- Sets the active compiler.
- Updates compilation snapshots inside `Results`.
- Updates `MissingCopys`.
- Subscribes to internal change events during the parse.

Failure modes:

- Throws `InvalidOperationException` if the path was not initialized.
- Wraps unexpected compilation exceptions in `ParsingException`.
- COBOL syntax and semantic problems are usually returned as diagnostics rather than exceptions.

## Static `Parse(...)`

Signature:

```csharp
public static Parser Parse(
    string path,
    bool isCopy,
    TypeCobolOptions options,
    DocumentFormat format,
    IList<string> copies = null,
    IAnalyzerProvider analyzerProvider = null)
```

Best one-shot API for agents. Important: this method sets `options.ExecToStep = ExecutionStep.Generate`.

