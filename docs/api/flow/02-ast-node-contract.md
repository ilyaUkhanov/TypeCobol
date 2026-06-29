# AST Node Contract For Flow

## `Node`

Declared in `TypeCobol/Compiler/Nodes/Node.cs`.

`Node` is the base AST abstraction. For DFD and CFG, it is both:

- the structural unit in the AST, and
- the instruction unit used by the built-in CFG/DFA builders.

Relevant identity and traversal members:

| Member | Flow use |
| --- | --- |
| `GetType().FullName` | Concrete node kind. Use as a stable fallback classifier. |
| `ID` | Non-unique semantic identifier such as `program`, `paragraph`, or `data-definition`. |
| `URI` | Tree-scoped identifier. Useful for diagram node IDs, but can be null for nodes without `ID`. |
| `Name` | Program, section, paragraph, data, or function name where applicable. |
| `QualifiedName` | Fully qualified named ancestor path. Useful for data and procedure labels. |
| `VisualQualifiedName` | Shorter readable path. Useful for diagram labels. |
| `Parent` | Parent node. Do not serialize recursively. |
| `Children` | AST child nodes. Traverse this for custom graph extraction. |
| `ChildrenCount` | Fast child count. |
| `GetChildren<T>()` | Typed child filtering. |
| `GetProgramNode()` | Nearest containing `Program`. |
| `GetEnclosingProgramOrFunctionNode()` | Nearest containing `Program` or function declaration. |
| `Lines` | Source lines from the associated code element. |
| `SelfAndChildrenLines` | Source lines for a subtree. |
| `IsInsideCopy()` | True when the node's code element comes from a COPY. |
| `Diagnostics` | Node-level diagnostics. |

Relevant code and semantic members:

| Member | Flow use |
| --- | --- |
| `CodeElement` | Attached syntax/semantic code element. Most statement-specific flow details live here. |
| `SemanticData` | Symbol associated with the node. For data nodes this is often a variable symbol. |
| `SymbolTable` | Symbol scope available at the node. |
| `Flags` | Internal semantic/generation flags. Useful for storage-section classification and special cases. |
| `IsFlagSet(...)` | Check node flags. |

Resolved DFD members populated by semantic cross-check:

| Member | Flow use |
| --- | --- |
| `StorageAreaReadsDataDefinition` | Maps syntactic read storage areas to resolved `DataDefinition` nodes. |
| `StorageAreaWritesDataDefinition` | Maps syntactic write storage areas to resolved `DataDefinition` nodes. |
| `StorageAreaReadsSymbol` | Maps syntactic read storage areas to resolved `VariableSymbol`s. Used by DFA. |
| `StorageAreaWritesSymbol` | Maps syntactic write storage areas to resolved `VariableSymbol`s. Used by DFA. |
| `GetDataDefinitionFromStorageAreaDictionary(...)` | Resolve one storage-area key against read/write dictionaries. |
| `GetDataDefinitionForQualifiedName(...)` | Resolve a qualified name against read/write dictionaries. |
| `QualifiedStorageAreas` | Stores qualified storage-area paths for some resolved references. |
| `GetQualifiedName(StorageArea)` | Returns the fully qualified name for a storage-area reference. |

Agent guidance:

- For DFD edges, prefer the resolved node dictionaries over raw `CodeElement.StorageAreaReads` and `StorageAreaWrites`.
- If resolved dictionaries are null, the semantic cross-check likely did not run or the reference could not resolve.
- Do not use `ToString()` as a unique ID. Use `URI` or derive a stable ID from source span plus node type.

## Visitor APIs

`Node.AcceptASTVisitor(...)` visits:

1. `BeginNode(node)`
2. node-specific `Visit(...)`
3. attached `CodeElement.AcceptASTVisitor(...)`
4. child nodes
5. `EndNode(node)`

Use visitors when you need the same traversal behavior as TypeCobol analyzers. Use explicit recursive `Children` traversal when you need simple serialization or custom graph extraction.

## Source Locations

There is no single `Node.SourceSpan` property. Use:

- `node.CodeElement?.ConsumedTokens` for token-level positions.
- `node.Lines` for source-line snapshots.
- `CodeElement.IsInsideCopy()`, `FirstCopyDirective`, and consumed token types for COPY provenance.

