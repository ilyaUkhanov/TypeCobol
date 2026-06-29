# Data Nodes For DFD

Declared in `TypeCobol/Compiler/Nodes/Data.cs`.

Data nodes are the DFD entity layer. Statement nodes reference data through `StorageArea` objects; semantic cross-check resolves those references back to `DataDefinition` nodes and symbols.

## Data Division And Sections

| Type | Flow role |
| --- | --- |
| `DataDivision` | Root for data declarations in a program. |
| `DataSection` | Base class for COBOL data sections. |
| `FileSection` | File records and file descriptions. Shared section. |
| `GlobalStorageSection` | Global storage. |
| `WorkingStorageSection` | Program working storage. |
| `LocalStorageSection` | Local storage. |
| `LinkageSection` | Parameters/shared data from callers. Shared section. |

Important `DataDivision` members:

| Member | Flow use |
| --- | --- |
| `FileSection` | Direct section accessor. |
| `GlobalStorageSection` | Direct section accessor. |
| `WorkingStorageSection` | Direct section accessor. |
| `LocalStorageSection` | Direct section accessor. |
| `LinkageSection` | Direct section accessor. |

`DataSection.IsShared` is true for `FileSection` and `LinkageSection`.

## DataDefinition

`DataDefinition` is the base class for storage declarations.

Important identity and reference members:

| Member | Flow use |
| --- | --- |
| `Name` | Data item name. |
| `QualifiedName` | Full data path through parent groups. |
| `VisualQualifiedName` | Display-friendly data path. |
| `CodeElement` | `DataDefinitionEntry` subclass. |
| `GetReferences()` | Storage-area references collected against this definition. |
| `AddReferences(...)` | Adds a reference from a storage area/node pair. Usually populated internally. |
| `TypeDefinition` | Associated typedef, if any. |
| `DataRedefinitions` | Redefinitions of this data item. |
| `GetBiggestRedefines()` | Redefinition with largest physical footprint. |

Important type and layout members:

| Member | Flow use |
| --- | --- |
| `DataType` | TypeCobol/Cobol data type. |
| `PrimitiveDataType` | Underlying primitive type for typed data. |
| `Picture` | COBOL `PICTURE` phrase when available. |
| `Usage` | COBOL usage, inherited from parent where applicable. |
| `GroupUsage` | Group usage, inherited through parent groups. |
| `MinOccurencesCount`, `MaxOccurencesCount` | Table bounds. |
| `OccursDependingOn` | ODO variable. Important for data-flow dependencies. |
| `HasUnboundedNumberOfOccurences` | Unbounded table marker. |
| `IsTableOccurence` | True when item is a table occurrence. |
| `PhysicalLength` | Computed storage size including children and occurrences. |
| `StartPosition` | Computed storage start position. |
| `PhysicalPosition` | Computed final byte position. |
| `SlackBytes` | Alignment padding. |
| `Synchronized` | Sync alignment. |
| `IsStronglyTyped`, `IsStrictlyTyped` | Type restriction classification. |
| `IsPartOfATypeDef`, `ParentTypeDefinition` | Typedef containment. |

Subclasses:

| Type | Meaning |
| --- | --- |
| `DataDescription` | Standard level 01-49/77 data declaration. |
| `DataCondition` | Level 88 condition value. |
| `DataRedefines` | Redefines another data definition. |
| `DataRenames` | Level 66 rename. |
| `FileDescription` | File description entry. |
| `TypeDefinition` | COBOL 2002/TypeCobol type definition. |

## DFD Guidance

Use each named `DataDefinition` as a candidate data node. Suggested diagram metadata:

- `qualifiedName`
- `section`
- `levelNumber` from `CodeElement`
- `dataType`
- `primitiveDataType`
- `picture`
- `usage`
- `minOccurs`
- `maxOccurs`
- `startPosition`
- `physicalLength`
- `redefines`
- `renames`
- `isCondition`
- `isTable`
- `isFromCopy`

When drawing edges, prefer statement-node reads/writes resolved to `DataDefinition`, not the data definition reference list alone. The statement gives source order and control context.

