# Flow Diagram API Map

This section documents the TypeCobol API surface most relevant to data-flow diagram and control-flow diagram generation.

Scope:

- Parser facade and compilation snapshots.
- AST nodes under `TypeCobol.Compiler.Nodes` and `TypeCobol.Compiler.CodeModel`.
- Code elements attached to AST nodes when they expose reads, writes, calls, branch targets, and source spans.
- Storage-area, call-site, and variable-reference objects used by the AST and code elements.
- CFG/DFA analyzer and graph objects in `TypeCobol.Analysis` because they are built from parser/AST events and are the supported flow-analysis path.

Out of scope:

- Scanner internals except where `CodeElement.ConsumedTokens` gives source locations.
- Code generation internals.
- Language-server transport objects except as optional consumers of CFG/DFA results.
- Generated parser internals.

## Recommended Flow

For custom DFD/CFG tooling:

1. Parse with `Parser.Parse(...)`, passing a CFG analyzer provider when you want built CFGs.
2. Read `parser.Results`.
3. Prefer `ProgramClassDocumentSnapshot.Root` for AST traversal.
4. For DFD, inspect `Node.StorageAreaReadsDataDefinition`, `Node.StorageAreaWritesDataDefinition`, `Node.StorageAreaReadsSymbol`, and `Node.StorageAreaWritesSymbol`.
5. For CFG, either use `TypeCobol.Analysis.CfgDfaAnalyzerFactory` or traverse procedure nodes and statement nodes yourself.
6. For DFA, build or retrieve `ControlFlowGraph<Node, DfaBasicBlockInfo<VariableSymbol>>`, then run `DefaultDataFlowGraphBuilder`.

## Primary Types

| Area | Types |
| --- | --- |
| Parser entrypoint | `Parser`, `TypeCobolOptions`, `DocumentFormat`, `FileCompiler`, `CompilationUnit` |
| Snapshots | `ProgramClassDocument`, `TemporarySemanticDocument`, `CodeElementsDocument` |
| AST base | `Node`, `GenericNode<T>`, `SyntaxTree`, `SourceFile` |
| Program/procedure | `Program`, `SourceProgram`, `NestedProgram`, `ProcedureDivision`, `Section`, `Paragraph`, `Sentence`, `FunctionDeclaration` |
| Data model | `DataDivision`, `DataSection`, `DataDefinition`, `DataDescription`, `DataRedefines`, `DataCondition`, `TypeDefinition` |
| Statement model | `Statement`, `StatementWithBody`, statement node classes in `Nodes/Statement.cs` |
| Code elements | `CodeElement`, `StatementElement`, statement-specific code element classes |
| DFD primitives | `StorageArea`, `ReceivingStorageArea`, `Variable`, `CallSite`, `CallTarget`, `FunctionCall`, `GroupCorrespondingImpact` |
| CFG/DFA | `CfgDfaAnalyzerFactory`, `CfgBuildingMode`, `ControlFlowGraph<N,D>`, `BasicBlock<N,D>`, `DefaultDataFlowGraphBuilder`, `DfaBasicBlockInfo<V>` |

## Important Design Point

The AST node gives structure and identity. The attached `CodeElement` gives many semantic details needed for flow analysis. The semantic cross-check fills node dictionaries that resolve syntactic storage-area references to data definitions and variable symbols.

For flow work, use both:

```csharp
foreach (var node in Walk(root))
{
    var codeElement = node.CodeElement;
    var reads = node.StorageAreaReadsDataDefinition;
    var writes = node.StorageAreaWritesDataDefinition;
}
```

