# Basic AST JSON Exporter

This example shows the supported agent pattern:

1. Configure `TypeCobolOptions`.
2. Choose a `DocumentFormat`.
3. Call `Parser.Parse(...)`.
4. Read the best available AST root.
5. Serialize a compact DTO instead of the raw TypeCobol object graph.

Run from the repository root:

```bash
dotnet run --project docs/examples/BasicAstJsonExporter/BasicAstJsonExporter.csproj -- path/to/program.cbl path/to/copybooks
```

By default, the example uses `DocumentFormat.RDZReferenceFormat`, which is usually a better starting point for classic COBOL than free format.

