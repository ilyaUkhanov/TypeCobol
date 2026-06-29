# Control Flow Analysis API

Declared mainly in `TypeCobol.Analysis`.

The built-in CFG builder is a syntax-driven analyzer. It receives AST construction events and returns `ControlFlowGraph<Node, D>` instances through analyzer results.

## `CfgBuildingMode`

Declared in `TypeCobol.Analysis/CfgBuildingMode.cs`.

| Value | Meaning |
| --- | --- |
| `None` | Do not build CFG. |
| `Standard` | Build standard CFG. |
| `Extended` | Build CFG with expanded PERFORM targets. |
| `WithDfa` | Build CFG with `DfaBasicBlockInfo<VariableSymbol>` block data, suitable for DFA. |

## `CfgDfaAnalyzerFactory`

Declared in `TypeCobol.Analysis/CfgDfaAnalyzerFactory.cs`.

Relevant members:

| Member | Flow use |
| --- | --- |
| `GetIdForMode(CfgBuildingMode)` | Analyzer result key, e.g. `cfg-WithDfa`. |
| `CreateCfgAnalyzer(CfgBuildingMode, TypeCobolOptions)` | Creates the syntax-driven CFG analyzer. |

Use `WithDfa` when you plan to run `DefaultDataFlowGraphBuilder`.

## Analyzer Hook

Implement `IAnalyzerProvider` and return the CFG analyzer from `CreateSyntaxDrivenAnalyzers(...)`.

```csharp
public sealed class FlowAnalyzerProvider : IAnalyzerProvider
{
    public ISyntaxDrivenAnalyzer[] CreateSyntaxDrivenAnalyzers(
        TypeCobolOptions options,
        TextSourceInfo textSourceInfo)
    {
        return new[]
        {
            CfgDfaAnalyzerFactory.CreateCfgAnalyzer(CfgBuildingMode.WithDfa, options)
        };
    }

    public IQualityAnalyzer[] CreateQualityAnalyzers(TypeCobolOptions options)
        => Array.Empty<IQualityAnalyzer>();
}
```

After parsing:

```csharp
var mode = CfgBuildingMode.WithDfa;
var id = CfgDfaAnalyzerFactory.GetIdForMode(mode);
var analyzerResults = parser.Results.ProgramClassDocumentSnapshot
    ?.PreviousStepSnapshot
    ?.AnalyzerResults;

if (analyzerResults != null &&
    analyzerResults.TryGetResult(id, out IList<ControlFlowGraph<Node, DfaBasicBlockInfo<VariableSymbol>>> cfgs))
{
    // cfgs contains one graph per program/function boundary.
}
```

## `ControlFlowGraph<N,D>`

Declared in `TypeCobol.Analysis/Graph/ControlFlowGraph.cs`.

Relevant members:

| Member | Flow use |
| --- | --- |
| `ProgramOrFunctionNode` | Program/function node the graph represents. |
| `ProcedureDivisionNode` | Procedure division node for this graph. |
| `RootBlock` | Entry basic block. |
| `AllBlocks` | All basic blocks in graph. |
| `BlockFor` | Map from AST node to owning basic block. |
| `SuccessorEdges` | Global successor edge target list. Block edge indexes point here. |
| `PredecessorEdges` | Global predecessor edge source list after setup. |
| `TerminalsBlocks` | Terminal blocks after predecessor setup. |
| `ParentGraph` | Parent graph for nested program/function graphs. |
| `NestedGraphs` | Nested graphs. |
| `UnreachableBlocks` | Blocks detected as unreachable. |
| `PrematurePerformExits` | PERFORMs broken by flow exits. |
| `WrongOrderPerformThrus` | PERFORM THRU ranges declared in incorrect order. |
| `RecursivePerforms` | Recursive PERFORM cycles. |
| `IsInitialized` | True after a procedure division was encountered. |
| `SetupPredecessorEdgesFromRoot()` | Computes predecessor edges and terminal blocks. |
| `DFS(...)` | Depth-first traversal API. |

Graph flags:

| Flag | Meaning |
| --- | --- |
| `Compound` | Graph has subgraphs. |

## `BasicBlock<N,D>`

Declared in `TypeCobol.Analysis/Graph/BasicBlock.cs`.

Relevant members:

| Member | Flow use |
| --- | --- |
| `Index` | Block index. Use as diagram node ID within a graph. |
| `Instructions` | Linked list of AST nodes in this block. |
| `Data` | Optional block data, e.g. `DfaBasicBlockInfo<VariableSymbol>`. |
| `SuccessorEdges` | Indexes into `cfg.SuccessorEdges`. |
| `PredecessorEdges` | Indexes into `cfg.PredecessorEdges` after setup. |
| `Context` | Multi-branch context for complex branch constructs. |
| `Flag` / `HasFlag(...)` | Block classification. |

Block flags:

| Flag | Meaning |
| --- | --- |
| `Ending` | Ending block. |
| `Default` | Default branch, such as WHEN OTHER. |
| `Declaratives` | Inside declaratives section. |
| `Start` | Start block. |
| `End` | End block. |
| `GroupGrafted` | Grafted block group. |
| `Recursive` | Recursive block. |

## CFG Diagram Extraction

To render edges:

```csharp
cfg.SetupPredecessorEdgesFromRoot();

foreach (var block in cfg.AllBlocks)
{
    foreach (var edgeIndex in block.SuccessorEdges)
    {
        var successor = cfg.SuccessorEdges[edgeIndex];
        // edge: block.Index -> successor.Index
    }
}
```

To label a block:

- Prefer first/last instruction node type and source lines.
- Include section/paragraph ancestry from each instruction's parents.
- Include flags such as default/end/recursive.

