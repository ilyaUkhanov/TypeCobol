# RAG Indexing

Index two corpora.

## API Member Chunks

Create one chunk per documented public member:

```json
{
  "chunk_type": "api_member",
  "fq_name": "TypeCobol.Parser.Parse",
  "signature": "public static Parser Parse(...)",
  "semantic_summary": "Initializes and compiles a file, returning the parser facade.",
  "preconditions": ["The path must exist in the configured source provider."],
  "side_effects": ["Sets options.ExecToStep to ExecutionStep.Generate."],
  "returns": "Parser with Results and MissingCopys populated.",
  "failure_modes": ["Throws ParsingException for unexpected internal failures."],
  "source_file": "TypeCobol/Parser.cs"
}
```

## Task Recipe Chunks

Create one chunk per agent task:

```json
{
  "chunk_type": "recipe",
  "task": "export_ast_json",
  "steps": [
    "create TypeCobolOptions",
    "choose DocumentFormat",
    "call Parser.Parse",
    "read Parser.Results",
    "read ProgramClassDocumentSnapshot.Root",
    "serialize a DTO"
  ],
  "pitfalls": [
    "do not serialize raw Node",
    "include diagnostics",
    "include missing COPYBOOKs",
    "verify fixed versus free source format"
  ]
}
```

For AST chunks, chunk by semantic boundary rather than arbitrary line ranges: program, division, section, paragraph, data declaration, and statement cluster.

