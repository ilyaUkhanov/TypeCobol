# Basic Parser Usage

The simplest supported flow is:

1. Build `TypeCobolOptions`.
2. Choose a `DocumentFormat`.
3. Call `Parser.Parse(...)`.
4. Read `parser.Results`.
5. Read diagnostics and AST snapshots.

```csharp
using TypeCobol;
using TypeCobol.Compiler;
using TypeCobol.Compiler.Directives;

var options = new TypeCobolOptions
{
    IsCobolLanguage = true,
    EnableSqlParsing = false,
    HaltOnMissingCopy = false,
    OptimizeWhitespaceScanning = true
};

var parser = Parser.Parse(
    path: "program.cbl",
    isCopy: false,
    options: options,
    format: DocumentFormat.RDZReferenceFormat,
    copies: new[] { "copybooks" });

var compilationUnit = parser.Results;
var root = compilationUnit.ProgramClassDocumentSnapshot?.Root
    ?? compilationUnit.TemporaryProgramClassDocumentSnapshot?.Root;

var diagnostics = compilationUnit.AllDiagnostics();
var missingCopies = parser.MissingCopys;
```

Use `Parser.Init(...)` and `Parser.Parse(path)` instead when you need a two-step lifecycle:

```csharp
var parser = new Parser();
parser.Init("program.cbl", isCopy: false, options, DocumentFormat.RDZReferenceFormat, copies);
parser.Parse("program.cbl");
```

## Choosing the Snapshot

Use this order for agent tooling:

| Snapshot | Use when |
| --- | --- |
| `ProgramClassDocumentSnapshot` | You want the cross-checked semantic AST. This is the preferred AST export source. |
| `TemporaryProgramClassDocumentSnapshot` | Cross-check did not run or failed, but AST production completed. |
| `CodeElementsDocumentSnapshot` | You only need pre-AST code elements or the pipeline stopped early. |

Always export diagnostics with the AST. Syntax or semantic errors do not necessarily throw; they are often represented as diagnostics.

