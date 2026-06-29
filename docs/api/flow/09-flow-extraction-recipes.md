# Flow Extraction Recipes

## Parse With CFG/DFA Analyzer

```csharp
using TypeCobol;
using TypeCobol.Analysis;
using TypeCobol.Analysis.Dfa;
using TypeCobol.Analysis.Graph;
using TypeCobol.Compiler;
using TypeCobol.Compiler.Directives;
using TypeCobol.Compiler.Nodes;
using TypeCobol.Compiler.Symbols;
using TypeCobol.Compiler.Text;

var options = new TypeCobolOptions
{
    IsCobolLanguage = true,
    HaltOnMissingCopy = false,
    OptimizeWhitespaceScanning = true
};

var parser = Parser.Parse(
    path: sourcePath,
    isCopy: false,
    options: options,
    format: DocumentFormat.RDZReferenceFormat,
    copies: copyDirectories,
    analyzerProvider: new FlowAnalyzerProvider(CfgBuildingMode.WithDfa));

var compilationUnit = parser.Results;
var root = compilationUnit.ProgramClassDocumentSnapshot?.Root;
```

Provider:

```csharp
public sealed class FlowAnalyzerProvider : IAnalyzerProvider
{
    private readonly CfgBuildingMode _mode;

    public FlowAnalyzerProvider(CfgBuildingMode mode) => _mode = mode;

    public ISyntaxDrivenAnalyzer[] CreateSyntaxDrivenAnalyzers(
        TypeCobolOptions options,
        TextSourceInfo textSourceInfo)
    {
        var analyzer = CfgDfaAnalyzerFactory.CreateCfgAnalyzer(_mode, options);
        return analyzer == null ? Array.Empty<ISyntaxDrivenAnalyzer>() : new[] { analyzer };
    }

    public IQualityAnalyzer[] CreateQualityAnalyzers(TypeCobolOptions options)
        => Array.Empty<IQualityAnalyzer>();
}
```

Retrieve CFGs:

```csharp
var id = CfgDfaAnalyzerFactory.GetIdForMode(CfgBuildingMode.WithDfa);
var analyzerResults = compilationUnit.ProgramClassDocumentSnapshot
    ?.PreviousStepSnapshot
    ?.AnalyzerResults;

if (analyzerResults != null &&
    analyzerResults.TryGetResult(id, out IList<ControlFlowGraph<Node, DfaBasicBlockInfo<VariableSymbol>>> cfgs))
{
    foreach (var cfg in cfgs)
    {
        cfg.SetupPredecessorEdgesFromRoot();
    }
}
```

## Build A Basic CFG DTO

```csharp
foreach (var cfg in cfgs)
{
    foreach (var block in cfg.AllBlocks)
    {
        var label = string.Join("\\n", block.Instructions.Select(LabelFor));

        foreach (var edge in block.SuccessorEdges)
        {
            var successor = cfg.SuccessorEdges[edge];
            AddControlEdge(block.Index, successor.Index);
        }
    }
}

static string LabelFor(Node node)
{
    var type = node.GetType().Name;
    var name = string.IsNullOrWhiteSpace(node.Name) ? null : node.Name;
    return name == null ? type : $"{type}: {name}";
}
```

## Build Statement-Level DFD Edges

```csharp
foreach (var node in Walk(root))
{
    if (node.StorageAreaReadsDataDefinition != null)
    {
        foreach (var pair in node.StorageAreaReadsDataDefinition)
        {
            AddDataEdge(pair.Value.URI, node.URI, "read", pair.Key.ToString());
        }
    }

    if (node.StorageAreaWritesDataDefinition != null)
    {
        foreach (var pair in node.StorageAreaWritesDataDefinition)
        {
            AddDataEdge(node.URI, pair.Value.URI, "write", pair.Key.ToString());
        }
    }
}
```

Recursive walk:

```csharp
static IEnumerable<Node> Walk(Node node)
{
    yield return node;
    foreach (var child in node.Children)
    {
        foreach (var descendant in Walk(child))
        {
            yield return descendant;
        }
    }
}
```

## Build Reaching-Definition Edges

```csharp
foreach (var cfg in cfgs)
{
    var dfa = new DefaultDataFlowGraphBuilder(cfg);
    dfa.ComputeUseDefSet();

    foreach (var use in dfa.UseList)
    {
        if (use.UseDef == null) continue;

        var defIndex = -1;
        while ((defIndex = use.UseDef.NextSetBit(defIndex + 1)) >= 0)
        {
            var def = dfa.DefList[defIndex];
            AddReachingDefinitionEdge(def.Instruction.URI, use.Instruction.URI, use.Variable.Name);
        }
    }
}
```

## Important Caveats

- `Node.URI` can be null. Fall back to graph/block/instruction index or source token span.
- Missing COPYBOOKs can remove declarations and alter flow.
- DFA uses resolved symbols. If semantic cross-check fails, DFA quality drops sharply.
- CALL parameter direction is not always fully inferable from a single file.
- `ValueOrigin` is intentionally partial. Treat unsupported cases as unknown.

