# Code Elements And Storage References

Declared mainly in:

- `TypeCobol/Compiler/CodeElements/CodeElement.cs`
- `TypeCobol/Compiler/CodeElements/StatementElement.cs`
- `TypeCobol/Compiler/CodeElements/Expressions/StorageArea.cs`
- `TypeCobol/Compiler/CodeElements/Expressions/Variable.cs`
- `TypeCobol/Compiler/CodeElements/Expressions/CallParameter.cs`

## `CodeElement`

`CodeElement` is the parser's semantic unit before the AST node layer. Each `Node.CodeElement` points to one when applicable.

Relevant members:

| Member | Flow use |
| --- | --- |
| `Type` | `CodeElementType`, stable classification. |
| `ConsumedTokens` | Source token span. Use for source line/column extraction. |
| `DebugMode` | Whether statement is on debug lines. |
| `SymbolInformationForTokens` | Token-level symbol info when enabled. |
| `StorageAreaDefinitions` | Data definitions and compiler-generated storage allocations introduced by this code element. |
| `StorageAreaReads` | Raw storage-area reads. |
| `StorageAreaWrites` | Raw storage-area writes as `ReceivingStorageArea`. |
| `StorageAreaGroupsCorrespondingImpact` | Group-level impact for `MOVE/ADD/SUBTRACT CORRESPONDING`. |
| `CallTarget` | Entry point defined by this code element. |
| `CallSites` | Calls made by this code element. |
| `Diagnostics` | Code-element diagnostics. |
| `IsInsideCopy()` | COPY provenance. |
| `FirstCopyDirective` | First COPY source involved when inside/across a copy. |

Use `CodeElement` raw reads/writes only when semantic resolution is unavailable. Prefer the containing node's resolved dictionaries after cross-check.

## `StatementElement`

Base class for executable statement code elements.

Relevant members:

| Member | Flow use |
| --- | --- |
| `StatementType` | Detailed executable statement classification. |
| `DataReadAccess` | Statement read storage areas. |
| `DataWriteAccess` | Statement write storage areas. |
| `ExpressionsToCompute` | Expressions evaluated by the statement. |
| `FunctionCalls` | Intrinsic/user function result allocations needed by the statement. |

`StatementType` is usually more specific than `CodeElement.Type`. For example, a `MoveStatement` can be `MoveSimpleStatement` or `MoveCorrespondingStatement`.

## Storage Areas

`StorageArea` represents a program storage reference. Subclasses:

| Type | Meaning |
| --- | --- |
| `DataOrConditionStorageArea` | Reference to a data item or condition name. |
| `IndexStorageArea` | Reference to an index. |
| `IntrinsicStorageArea` | Implicit special register without declaration requirement. |
| `StorageAreaPropertySpecialRegister` | `ADDRESS OF` / `LENGTH OF` property reference. |
| `FilePropertySpecialRegister` | File property such as `LINAGE-COUNTER`. |
| `FunctionCallResult` | Compiler-allocated result of a function call. |

Important `StorageArea` members:

| Member | Flow use |
| --- | --- |
| `Kind` | `StorageAreaKind` classifier. |
| `SymbolReference` | Raw symbol reference. |
| `IsReadFrom`, `IsWrittenTo` | Direction marker on the syntactic reference. |
| `ReferenceModifier` | Substring/slice information. |
| `NeedDeclaration` | False for some intrinsic areas. |
| `GetStorageAreaThatNeedDeclaration` | Underlying declaration-bearing area. |

Important `DataOrConditionStorageArea` members:

| Member | Flow use |
| --- | --- |
| `Subscripts` | Table indexes/subscripts. These are read dependencies too. |
| `IsPartOfFunctionArgument` | True when reference appears inside a function argument. |
| `Hash` | Codegen-oriented index hash. Usually not needed for diagrams. |

## Variables

`VariableBase` wraps either a storage area or a literal/expression value.

Important types:

| Type | Meaning |
| --- | --- |
| `Variable` | Any storage reference, literal, or symbol reference. |
| `VariableOrExpression` | Variable or arithmetic expression. |
| `ReceivingStorageArea` | Write target. Marks its storage area as written. |
| `IntegerVariable`, `NumericVariable`, `CharacterVariable`, `AlphanumericVariable` | Typed variable or literal wrappers. |
| `SymbolReferenceVariable` | Symbol reference wrapper for program names, method names, pointers, etc. |

Important members:

| Member | Flow use |
| --- | --- |
| `StorageArea` | Underlying storage reference if variable is not a literal. |
| `MainSymbolReference` | Primary symbol reference for simple variables. |
| `IsLiteral` | True for numeric/alphanumeric literals. |
| `NumericValue`, `AlphanumericValue`, `RepeatedCharacterValue` | Literal data. |
| `ReceivingStorageArea.SendingStorageAreas` | Input areas that feed this write target. |

## Calls

Call representation:

| Type | Flow use |
| --- | --- |
| `CallSite` | A call instruction with target and parameters. |
| `CallSiteParameter` | One actual argument. |
| `CallTarget` | Entry point definition with formal parameters. |
| `CallTargetParameter` | One formal parameter. |
| `FunctionCall` | Intrinsic, user-defined function, or procedure call base. |
| `ProcedureCall` | TypeCobol procedure call with input/inout/output parameter lists. |

Important call members:

| Member | Flow use |
| --- | --- |
| `CallSite.CallTarget` | Called program/method/function symbol. |
| `CallSite.Parameters` | Actual arguments. |
| `CallSiteParameter.SharingMode` | BY REFERENCE, CONTENT, or VALUE. |
| `CallSiteParameter.IsOmitted` | Omitted argument marker. |
| `CallSiteParameter.StorageAreaOrValue` | Actual data/literal/expression. |
| `CallTarget.Name` | Entry point symbol definition. Null can mean procedure division header. |
| `CallTarget.Parameters` | Formal parameters. |
| `ProcedureCall.InputParameters` | Input actuals. |
| `ProcedureCall.InoutParameters` | Bidirectional actuals. |
| `ProcedureCall.OutputParameters` | Output actuals. |

For DFD:

- `ByReference` and `InOutParameters` imply read/write coupling.
- `ByContent` and `ByValue` imply caller-to-callee flow without caller-side mutation.
- `OutputParameter` / returning parameters imply callee-to-caller flow.

