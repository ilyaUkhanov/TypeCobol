# TypeCobol.Compiler.FileCompiler

Lower-level compiler orchestrator for one file or an in-memory text document.

Declared in `TypeCobol/Compiler/FileCompiler.cs`.

Agents should normally use `TypeCobol.Parser` instead. Use `FileCompiler` when you need editor-style incremental behavior, direct access to text documents, or explicit step execution.

## Key Members

| Member | Meaning |
| --- | --- |
| `TextDocument` | Source text buffer in memory. |
| `CompilerOptions` | Active `TypeCobolOptions`. |
| `CompilationResultsForCopy` | Compilation document for copy files. |
| `CompilationResultsForProgram` | `CompilationUnit` for program/class files. |
| `GeneratedTextDocument` | Generated COBOL text buffer. |
| `CompileOnce()` | Runs the pipeline using `CompilerOptions.ExecToStep` and `HaltOnMissingCopy`. |
| `CompileOnce(ExecutionStep, bool)` | Runs the pipeline to a requested step. |
| `ExecutionStepEventHandler` | Optional hook notified after pipeline steps. |

## Pipeline

`CompileOnce(...)` runs scanner, preprocessor, code element parsing, AST production, semantic cross-check, and code analysis according to the requested `ExecutionStep`.

