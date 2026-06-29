# TypeCobol.Compiler.Parser Program/Class Snapshots

Program/class snapshots are the AST-facing compiler artifacts exposed by `CompilationUnit`.

Declared in:

- `TypeCobol/Compiler/Parser/ProgramClassDocument.cs`
- `TypeCobol/Compiler/Parser/TemporarySemanticDocument.cs`

## ProgramClassDocument

Final semantic view of a source document after program/class parsing and semantic cross-check.

| Member | Meaning |
| --- | --- |
| `TextSourceInfo` | Source file or in-memory buffer metadata. |
| `PreviousStepSnapshot` | `TemporarySemanticDocument` used to compute this snapshot. |
| `CurrentVersion` | Numeric version of this program/class snapshot. |
| `Root` | AST root as `SourceFile`. Use this for normal AST export. |
| `NodeCodeElementLinkers` | Map between code elements and AST nodes. Useful for source-to-AST correlation. |

## TemporarySemanticDocument

Intermediate AST view produced before semantic cross-check.

| Member | Meaning |
| --- | --- |
| `TextSourceInfo` | Source file or in-memory buffer metadata. |
| `Root` | AST root as `SourceFile`. Use as fallback when final snapshot is unavailable. |
| `NodeCodeElementLinkers` | Map between code elements and AST nodes. |
| `AnalyzerResults` | Results from syntax-driven analyzers. |
| `TypedVariablesOutsideTypedef` | Type-linking worklist for semantic processing. |
| `TypeThatNeedTypeLinking` | Types requiring later linking. |
| `Diagnostics` | Diagnostics produced while parsing program/class nodes. |
| `PreviousStepSnapshot` | Code elements document used to produce this temporary AST. |
| `Lines` | Code element lines used by this snapshot. |

Agent guidance:

- Prefer `CompilationUnit.ProgramClassDocumentSnapshot.Root`.
- Fall back to `CompilationUnit.TemporaryProgramClassDocumentSnapshot.Root`.
- Include `TemporarySemanticDocument.Diagnostics` indirectly through `CompilationUnit.AllDiagnostics()`.

