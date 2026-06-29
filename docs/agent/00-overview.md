# TypeCobol Parser + AST Agent Guide

This guide documents the TypeCobol parser as an external contract for coding agents. It focuses on the API surface that agents should normally call, the semantic meaning of the returned objects, and the compact JSON shape that is suitable for retrieval-augmented generation.

TypeCobol is a compiler front-end for COBOL and TypeCobol. The parser pipeline can scan text, preprocess COPY statements, parse code elements, build a temporary semantic AST, cross-check the program/class model, run optional analyzers, and generate COBOL depending on `TypeCobolOptions.ExecToStep`.

## Recommended Surface

Use these types first:

| Type | Role | Agent priority |
| --- | --- | --- |
| `TypeCobol.Parser` | High-level facade for parsing a file and retrieving a `CompilationUnit`. | Very high |
| `TypeCobol.Compiler.Directives.TypeCobolOptions` | Parser and compiler control plane. | Very high |
| `TypeCobol.Compiler.DocumentFormat` | Source encoding, line delimiter, fixed/free format, and COBOL columns. | Very high |
| `TypeCobol.Compiler.CompilationUnit` | Main compilation artifact containing snapshots and diagnostics. | Very high |
| `TypeCobol.Compiler.Parser.ProgramClassDocument` | Final semantic program/class snapshot. | High |
| `TypeCobol.Compiler.Parser.TemporarySemanticDocument` | AST snapshot before semantic cross-check. | High |
| `TypeCobol.Compiler.Nodes.Node` | Base AST node abstraction. | Very high |
| `TypeCobol.Compiler.CodeModel.Program` | COBOL program node family. | High |
| `TypeCobol.Compiler.FileCompiler` | Lower-level pipeline orchestrator. | Medium |

Agents should prefer `Parser.Parse(...)` for scripts and one-shot tooling. Use `Parser.Init(...)` plus `Parser.Parse(path)` when you need to reuse a parser instance, control initialization per file, or inject custom symbols/analyzers.

See `docs/api/` for the first hand-written semantic reference pages. These pages are intentionally smaller than a generated API reference and focus on the members an agent needs to call safely.

For data-flow and control-flow diagram generation, start with `docs/api/flow/00-flow-api-map.md`. That section documents the parser, AST, code element, storage reference, CFG, and DFA objects that are relevant to graph extraction.

## Mental Model

The result is not a single raw AST object. It is a `CompilationUnit` with step snapshots:

1. Text and token snapshots from scanner steps.
2. Processed token snapshots after preprocessing and COPY expansion.
3. `CodeElementsDocumentSnapshot`, which contains parsed COBOL code elements.
4. `TemporaryProgramClassDocumentSnapshot`, which contains the AST before semantic cross-check.
5. `ProgramClassDocumentSnapshot`, which contains the cross-checked program/class AST.
6. `CodeAnalysisDocumentSnapshot`, which contains quality diagnostics from optional analyzers.

For AST export, read `ProgramClassDocumentSnapshot.Root` when available. Fall back to `TemporaryProgramClassDocumentSnapshot.Root` when semantic cross-checking did not run or did not complete.

## Pitfalls

- `Parser.Parse(...)` is not a pure text parser. It runs the configured compilation pipeline.
- The static `Parser.Parse(...)` method mutates `options.ExecToStep` to `ExecutionStep.Generate`.
- `DocumentFormat` is semantically important. Fixed/reference and free-format COBOL are interpreted differently.
- Missing COPYBOOKs must be captured separately from diagnostics through `Parser.MissingCopys`.
- Do not serialize `Node` objects directly. Nodes contain parent links, semantic references, diagnostics, visitors, and mutable helper state.
- AST completeness depends on `ExecToStep`. Stopping before `ExecutionStep.AST` will not produce a semantic tree.
