# Compilation Pipeline

`FileCompiler.CompileOnce(...)` defines the concrete pipeline:

| Step | Method | Output |
| --- | --- | --- |
| Scanner | `UpdateTokensLines()` and `RefreshTokensDocumentSnapshot()` | Token lines |
| Preprocessor | `RefreshProcessedTokensDocumentSnapshot()` | Processed tokens and COPY expansion state |
| CodeElement | `RefreshCodeElementsDocumentSnapshot()` | `CodeElementsDocumentSnapshot` |
| AST | `ProduceTemporarySemanticDocument()` | `TemporaryProgramClassDocumentSnapshot` |
| SemanticCrossCheck | `RefreshProgramClassDocumentSnapshot()` | `ProgramClassDocumentSnapshot` |
| CodeAnalysis | `RefreshCodeAnalysisDocumentSnapshot()` | `CodeAnalysisDocumentSnapshot` |

`TypeCobolOptions.ExecToStep` controls the maximum step. The static `Parser.Parse(...)` helper sets it to `ExecutionStep.Generate` before running the parser, so one-shot parsing attempts the full available pipeline.

Agents should normally call `Parser` rather than `FileCompiler` directly. Use `FileCompiler` only for editor-style incremental behavior, direct snapshot refresh control, or integration into a larger compilation project.

