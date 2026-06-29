# TypeCobol.Compiler.Nodes.Node

Base AST node class.

Declared in `TypeCobol/Compiler/Nodes/Node.cs`.

## Agent-Relevant Members

| Member | Meaning |
| --- | --- |
| `CodeElement` | Parsed code element associated with this node, if any. |
| `SemanticData` | Symbol associated with this node. |
| `Parent` | Parent node. Creates object graph cycles. |
| `Children` | Read-only child nodes. |
| `ChildrenCount` | Number of child nodes. |
| `Flags` | 64-bit node flags. |
| `Name` | Semantic name, when available. |
| `QualifiedName` | Qualified name through named ancestors. |
| `VisualQualifiedName` | Display-oriented qualified name. |
| `ID` | Non-unique node identifier. |
| `URI` | Tree-scoped unique identifier. |
| `Lines` | Source lines consumed by this node. |
| `Diagnostics` | Node diagnostics. May be null before any diagnostic is added. |
| `GetChildren<T>()` | Typed child filter. |
| `GetProgramNode()` | Nearest containing `Program`. |
| `GetEnclosingProgramOrFunctionNode()` | Nearest containing `Program` or function declaration. |

## Serialization Guidance

Serialize a DTO instead of the raw node. Include scalar fields and recursively serialize `Children`. Do not serialize `Parent`, `SemanticData`, or raw `CodeElement` graphs.

