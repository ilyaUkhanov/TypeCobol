# Agent Pitfalls

- `Parser.Parse(...)` returns a `Parser`, not a `CompilationUnit`. Read `parser.Results`.
- The static `Parser.Parse(...)` helper changes `options.ExecToStep` to `ExecutionStep.Generate`.
- `Parser.Results` is only valid after parsing. Calling it before `Parse(path)` leaves no active compiler result.
- Missing COPYBOOKs should be exported from `Parser.MissingCopys`.
- `HaltOnMissingCopy = true` can prevent AST snapshots from being produced.
- `DocumentFormat` controls COBOL column interpretation. Do not default every corpus to free format.
- `ProgramClassDocumentSnapshot.Root` is the preferred AST root. Fall back to `TemporaryProgramClassDocumentSnapshot.Root`.
- `AllDiagnostics()` aggregates scanner, code element, node, and analyzer diagnostics depending on completed steps.
- `Node.Parent` creates cycles. Do not serialize it.
- `Node.CodeElement` can be useful as text, but do not recursively serialize the full code element object graph.

