# AST Model

`Node` is the base class for AST nodes. Important public data for agents:

| Member | Meaning |
| --- | --- |
| `GetType().FullName` | Concrete semantic node type. |
| `ID` | Non-unique identifier based on node kind and name. |
| `URI` | Tree-scoped unique identifier derived from parent URIs and IDs. |
| `Name` | Semantic name where the node has one. |
| `QualifiedName` | Name path through all named parents. |
| `VisualQualifiedName` | Readable name path scoped for display. |
| `Parent` | Parent node. Do not serialize directly. |
| `Children` | Child nodes. Traverse this for AST export. |
| `CodeElement` | Parsed code element associated with the node. |
| `Lines` | Source text lines consumed by the node's code element, excluding imported tokens. |
| `Diagnostics` | Diagnostics attached to the node. |
| `Flags` | Internal and semantic bit flags. |

Program nodes live under `TypeCobol.Compiler.CodeModel`:

- `Program`
- `SourceProgram`
- `NestedProgram`
- `StackedProgram`

Use the type name plus normalized derived fields for retrieval. For example, identify divisions, sections, paragraphs, data declarations, `CALL`, and `PERFORM` statements from concrete node types and code element types.

## Serialization Rule

Do not serialize `Node` directly. Export a DTO with scalar fields and recursive child DTOs. Raw nodes include parent links, semantic symbol references, code-generation state, and mutable helper caches.

