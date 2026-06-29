# Statement Nodes For CFG And DFD

Declared in `TypeCobol/Compiler/Nodes/Statement.cs`.

Statement nodes are AST wrappers around statement `CodeElement` subclasses. The node determines AST position; the code element exposes statement-specific operands, branch targets, and data accesses.

## Interfaces

| Type | Meaning |
| --- | --- |
| `Statement` | Marker for executable statement nodes. |
| `StatementWithBody` | Statement with explicit end code element. Exposes `EndType`. |
| `StatementCondition` | Condition branch node family, such as `Else`, `When`, `WhenOther`. |

For CFG tools, a node is an executable statement if `ControlFlowGraphBuilder.IsStatement(node)` returns true.

## Branch And Control Nodes

| Node | Code element | CFG use |
| --- | --- | --- |
| `If` | `IfStatement` | Conditional branch. `EndType` is `IfStatementEnd`. |
| `Else` | `ElseCondition` | Alternative branch. |
| `Evaluate` | `EvaluateStatement` | Multi-branch decision. Can be transformed into cascaded IF by CFG builder. |
| `WhenGroup` | None | Group container for WHEN nodes. |
| `When` | `WhenCondition` | EVALUATE branch. |
| `WhenOther` | `WhenOtherCondition` | Default EVALUATE branch. |
| `Search` | `SearchStatement` | Search loop/branch. CFG builder can cascade search branches. |
| `Goto` | `GotoStatement` | Explicit transfer to section/paragraph. |
| `NextSentence` | `NextSentenceStatement` | Transfers to next sentence boundary. |
| `Perform` | `PerformStatement` | Inline perform body/loop. |
| `PerformProcedure` | `PerformProcedureStatement` | Out-of-line procedure call-like control transfer. |
| `Exit`, `ExitParagraph`, `ExitSection`, `ExitPerform`, `ExitProgram`, `ExitMethod`, `Goback`, `Stop` | Exit statement code elements | Terminal or boundary-exit edges. |

`PerformProcedure` exposes resolved targets after semantic cross-check:

| Member | Meaning |
| --- | --- |
| `ProcedureParagraphSymbol` | Resolved start paragraph symbol. |
| `ProcedureSectionSymbol` | Resolved start section symbol. |
| `ThroughProcedureParagraphSymbol` | Resolved THRU end paragraph symbol. |
| `ThroughProcedureSectionSymbol` | Resolved THRU end section symbol. |

## Call Nodes

| Node | Code element | Flow use |
| --- | --- | --- |
| `Call` | `CallStatement` | COBOL `CALL`. Has input/output parameters in the code element. |
| `ProcedureStyleCall` | `ProcedureStyleCallStatement` | TypeCobol procedure-style call. Exposes `FunctionCall`, `FunctionDeclaration`, and documentation writer. |
| `Invoke` | `InvokeStatement` | OO method invocation. |

For interprocedural DFD, inspect `CodeElement.CallSites`, `CallTarget`, `InputParameters`, and `OutputParameter`.

## Assignment And Data-Movement Nodes

| Node | Flow use |
| --- | --- |
| `Move` | Source-to-target data movement. Implements `VariableWriter` and `FunctionCaller`. |
| `Set` | Assignment to variables, indexes, switches, and conditions. Implements `VariableWriter`. |
| `Compute` | Arithmetic assignment. Implements `VariableWriter`. |
| `Add`, `Subtract`, `Multiply`, `Divide` | Arithmetic statements with reads and writes. Implement `VariableWriter`. |
| `Initialize` | Writes defaults to target storage. |
| `Inspect` | Can write/tally/replace. Exposes `VariablesWritten` and `IsUnsafe`. |
| `String`, `Unstring` | Character data movement. |
| `Accept` | Writes receiving storage area from input/date/environment. |
| `Read`, `Return` | Reads records and writes receiving areas. |
| `Write`, `Rewrite`, `Release` | Writes records/files. |
| `Sort`, `Merge` | Compound data/control operations with input/output procedures and storage reads. |
| `JsonGenerate`, `JsonParse`, `XmlGenerate`, `XmlParse` | Structured data conversion statements with explicit source/destination operands. |

For DFD, do not special-case every statement first. Start with:

- `node.StorageAreaReadsDataDefinition`
- `node.StorageAreaWritesDataDefinition`
- `node.CodeElement.StorageAreaGroupsCorrespondingImpact`
- `node.CodeElement.CallSites`
- `node.CodeElement.CallTarget`

Then add statement-specific enrichment only where needed.

## Other Statement Nodes

Other wrappers include:

`Accept`, `Allocate`, `Alter`, `Cancel`, `Continue`, `Delete`, `Display`, `Entry`, `Exec`, `ExecText`, `Free`, `Initialize`, `Open`, `Close`, `Start`, `Commit`, `Rollback`, SQL statement nodes when SQL parsing is enabled, and statement end markers represented as `CodeElementEnd` instances rather than normal executable nodes.

