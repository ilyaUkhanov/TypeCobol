# TypeCobol.Compiler.CompilationUnit

Central compilation artifact from text to code model.

Declared in `TypeCobol/Compiler/CompilationUnit.cs`.

## Key Snapshots

| Member | Meaning |
| --- | --- |
| `CodeElementsDocumentSnapshot` | Parsed code elements after processed tokens. |
| `TemporaryProgramClassDocumentSnapshot` | AST before semantic cross-check. |
| `ProgramClassDocumentSnapshot` | Cross-checked semantic program/class AST. |
| `CodeAnalysisDocumentSnapshot` | Program/class snapshot plus optional analyzer diagnostics. |

## Refresh Methods

| Method | Step |
| --- | --- |
| `RefreshCodeElementsDocumentSnapshot()` | Code element parser. |
| `ProduceTemporarySemanticDocument()` | AST creation. |
| `RefreshProgramClassDocumentSnapshot()` | Semantic cross-check. |
| `RefreshCodeAnalysisDocumentSnapshot()` | Quality analyzer pass. |

## Diagnostics

`AllDiagnostics()` returns diagnostics from available snapshots. `AllDiagnostics(true)` limits diagnostics to those produced up to the code element phase.

Agent guidance:

- Export diagnostics alongside every AST export.
- Prefer `ProgramClassDocumentSnapshot.Root` for complete AST export.
- Fall back to `TemporaryProgramClassDocumentSnapshot.Root` when cross-checking did not complete.

