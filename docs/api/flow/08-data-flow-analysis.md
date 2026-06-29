# Data Flow Analysis API

Declared in `TypeCobol.Analysis/Dfa`.

The built-in DFA layer computes use/definition lists, GEN/KILL sets, IN/OUT reaching definitions, and USE-DEF chains over a CFG.

## `DefaultDataFlowGraphBuilder`

Declared in `TypeCobol.Analysis/Dfa/DefaultDataFlowGraphBuilder.cs`.

Construct it with a CFG built in `CfgBuildingMode.WithDfa`:

```csharp
var dfa = new DefaultDataFlowGraphBuilder(cfg);
dfa.ComputeUseList();
dfa.ComputeDefList();
dfa.ComputeGenSet();
dfa.ComputeKillSet();
dfa.ComputeInOutSet();
dfa.ComputeUseDefSet();
```

Important behavior:

- Uses `node.StorageAreaReadsSymbol` for USE variables.
- Uses `node.StorageAreaWritesSymbol` for DEF variables.
- Treats `DataDefinition` nodes with `SemanticData` as initial definitions and inserts them near the root block.

## `DataFlowGraphBuilder<N,V>`

Relevant members:

| Member | Flow use |
| --- | --- |
| `Cfg` | Underlying `ControlFlowGraph<N, DfaBasicBlockInfo<V>>`. |
| `UseList` | All use points after `ComputeUseList()`. |
| `DefList` | All definition points after `ComputeDefList()`. |
| `VariableDefMap` | Variable to all definition bits. |
| `GetUseVariables(N)` | Override point for custom variable extraction. |
| `GetDefVariables(N)` | Override point for custom definition extraction. |
| `UsePointCreated` | Event emitted while building use list. |
| `DefPointCreated` | Event emitted while building def list. |
| `ComputeGenSet()` | Computes last local definitions per block. |
| `ComputeKillSet()` | Computes definitions killed by block definitions. |
| `ComputeInOutSet()` | Computes reaching definitions. |
| `ComputeUseDefSet()` | Computes definitions reaching each use. |

State flags:

| Member | Meaning |
| --- | --- |
| `IsGenSetCalculated` | GEN sets are available. |
| `IsKillSetCalculated` | KILL sets are available. |
| `IsInOutSetCalculated` | IN/OUT sets are available. |
| `IsUseDefSetCalculated` | USE-DEF chains are available. |

## `DfaBasicBlockInfo<V>`

Block data stored in `BasicBlock<Node, DfaBasicBlockInfo<VariableSymbol>>.Data`.

| Member | Meaning |
| --- | --- |
| `Gen` | Local definitions that reach block end. |
| `Kill` | Definitions outside the block killed by local definitions. |
| `In` | Definitions reaching block entry. |
| `Out` | Definitions reaching block exit. |
| `UseListFirstIndex`, `UseCount` | Slice into global use list for this block. |
| `DefListFirstIndex`, `DefCount` | Slice into global def list for this block. |

## `DfaUsePoint<I,V>`

Represents one variable use.

| Member | Meaning |
| --- | --- |
| `Instruction` | AST node/instruction where the use occurs. |
| `InstructionIndex` | Instruction index inside its basic block. |
| `Variable` | Used variable. |
| `BlockIndex` | Owning basic block. |
| `UseDef` | Bit set of definitions reaching this use after `ComputeUseDefSet()`. |

## `DfaDefPoint<I,V>`

Represents one variable definition.

| Member | Meaning |
| --- | --- |
| `Index` | Definition index. |
| `Instruction` | AST node/instruction where the definition occurs. |
| `InstructionIndex` | Instruction index inside its block. |
| `Variable` | Defined variable. |
| `BlockIndex` | Owning basic block. |
| `UserData` | Optional analysis-specific data. |

## `ValueOrigin`

`ValueOrigin.ComputeFrom(...)` traces possible assignment origins for a variable use.

Supported origins include:

- simple `MOVE` from alphanumeric literal,
- simple `MOVE` from another identifier,
- `SET` condition variable to true,
- initial values from `DataDescription` or `DataRedefines`.

Unsupported or partial cases include `MOVE CORRESPONDING` and many complex expression assignments. Treat null or empty origins as "unknown", not as proof of no flow.

## DFD Diagram Extraction

Recommended edge model:

| Source | Target | How |
| --- | --- | --- |
| statement node | data definition | write edge from `StorageAreaWritesDataDefinition` |
| data definition | statement node | read edge from `StorageAreaReadsDataDefinition` |
| def point | use point | reaching-definition edge from `DfaUsePoint.UseDef` |
| caller actual | callee formal | call edge from `CallSiteParameter` / `CallTargetParameter` |
| callee return/output | caller receiver | call return/output edge |

Use DFA for precise reaching-definition diagrams. Use node read/write dictionaries for simpler statement-level DFDs.

