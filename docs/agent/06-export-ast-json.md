# Export AST JSON

Use the DTO pattern in `docs/examples/BasicAstJsonExporter`. The core shape is:

```json
{
  "parserVersion": "string",
  "source": "path",
  "copyDirectories": ["path"],
  "missingCopies": ["COPY-NAME"],
  "diagnostics": ["diagnostic text"],
  "ast": {
    "clrType": "TypeCobol.Compiler.CodeModel.SourceProgram",
    "id": "program",
    "uri": "source.program",
    "name": "PROGRAM-NAME",
    "qualifiedName": "PROGRAM-NAME",
    "visualQualifiedName": "PROGRAM-NAME",
    "flags": "0",
    "codeElementType": "TypeCobol.Compiler.CodeElements.ProgramIdentification",
    "codeElementText": "PROGRAM-ID. PROGRAM-NAME.",
    "diagnostics": [],
    "sourceLines": [],
    "children": []
  }
}
```

For RAG, add derived fields during export:

- `nodeKindNormalized`
- `programName`
- `division`
- `section`
- `paragraph`
- `copybookName`
- `sourceStartLine`
- `sourceEndLine`
- `containsDiagnostics`
- `readsStorageAreas`
- `writesStorageAreas`
- `callsPrograms`
- `performsParagraphs`

The derived fields are more useful for retrieval than raw CLR names alone.

