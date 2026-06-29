# Parser And Snapshots For Flow

## `TypeCobol.Parser`

Declared in `TypeCobol/Parser.cs`.

Use `Parser` as the normal entrypoint for flow tooling. It hides project setup, source lookup, COPYBOOK registration, and `FileCompiler` construction.

Relevant members:

| Member | Flow use |
| --- | --- |
| `Parser.Parse(...) static` | One-shot parse. Sets `options.ExecToStep = ExecutionStep.Generate`, so the full pipeline is attempted. |
| `Init(...)` | Use when injecting an `IAnalyzerProvider` for CFG/DFA generation. |
| `Parse(path)` | Runs compilation for the initialized file. |
| `Results` | Returns `CompilationUnit`, the source for snapshots, diagnostics, analyzer results, and AST root. |
| `MissingCopys` | Export with flow output because missing copies can remove data declarations and procedure bodies. |

For CFG generation, pass an analyzer provider to `Parser.Parse(...)` or `Parser.Init(...)`. The provider should create a CFG analyzer with `CfgDfaAnalyzerFactory.CreateCfgAnalyzer(...)`.

## `CompilationUnit`

Declared in `TypeCobol/Compiler/CompilationUnit.cs`.

Relevant members:

| Member | Flow use |
| --- | --- |
| `CodeElementsDocumentSnapshot` | Pre-AST view. Useful when a node was not produced but code elements exist. |
| `TemporaryProgramClassDocumentSnapshot` | AST before semantic cross-check. Use only as fallback for structural diagrams. |
| `ProgramClassDocumentSnapshot` | Preferred AST for flow diagrams. Semantic cross-check has populated reads/writes and resolved targets where possible. |
| `CodeAnalysisDocumentSnapshot` | Holds quality analyzer diagnostics and results. |
| `AllDiagnostics()` | Export with every diagram. |

Flow tools should generally require `ProgramClassDocumentSnapshot != null`. If it is null, you can produce a partial structural diagram from `TemporaryProgramClassDocumentSnapshot`, but DFD quality is lower because cross-check dictionaries may be absent.

## `ProgramClassDocument`

Declared in `TypeCobol/Compiler/Parser/ProgramClassDocument.cs`.

Relevant members:

| Member | Flow use |
| --- | --- |
| `Root` | AST root as `SourceFile`. Traverse from here. |
| `NodeCodeElementLinkers` | Map from `CodeElement` to `Node`. Useful when analysis starts from code elements or diagnostics. |
| `PreviousStepSnapshot` | The `TemporarySemanticDocument` used to compute this snapshot. |
| `TextSourceInfo` | Source metadata for diagram provenance. |
| `CurrentVersion` | Snapshot version. Include in cache keys if doing incremental tooling. |

## `TemporarySemanticDocument`

Declared in `TypeCobol/Compiler/Parser/TemporarySemanticDocument.cs`.

Relevant members:

| Member | Flow use |
| --- | --- |
| `Root` | Partial AST root before cross-check. |
| `Diagnostics` | AST parse diagnostics. |
| `AnalyzerResults` | Results produced by syntax-driven analyzers during AST construction. CFG analyzers store their graphs here. |
| `NodeCodeElementLinkers` | Code element to node map before final snapshot. |
| `TypedVariablesOutsideTypedef`, `TypeThatNeedTypeLinking` | Internal semantic worklists. Usually not needed for diagrams. |

## Analyzer Results

Syntax-driven analyzers run during `CompilationUnit.ProduceTemporarySemanticDocument()`. CFG analyzers are syntax-driven, so retrieve their result from:

```csharp
var results = compilationUnit.TemporaryProgramClassDocumentSnapshot?.AnalyzerResults
    ?? compilationUnit.ProgramClassDocumentSnapshot?.PreviousStepSnapshot?.AnalyzerResults;
```

Use the identifier returned by `CfgDfaAnalyzerFactory.GetIdForMode(mode)`.

