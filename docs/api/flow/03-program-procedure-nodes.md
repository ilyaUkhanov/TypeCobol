# Program And Procedure Nodes

Declared mainly in:

- `TypeCobol/Compiler/CodeModel/Program.cs`
- `TypeCobol/Compiler/Nodes/Procedure.cs`

These nodes define natural CFG boundaries and diagram grouping.

## Program Nodes

| Type | Flow role |
| --- | --- |
| `Program` | Base program node. Use as a CFG root boundary and DFD namespace. |
| `SourceProgram` | Outermost program of a compilation unit. |
| `NestedProgram` | Program nested inside another program. Creates nested CFGs. |
| `StackedProgram` | Additional source-level program. Creates a separate graph. |

Important `Program` members:

| Member | Flow use |
| --- | --- |
| `Name` | Program identifier for graph label. |
| `Hash` | 8-character COBOL-name hash. Useful when matching codegen naming, not as a general diagram ID. |
| `Namespace` | Namespace value from identification. |
| `IsNested`, `IsStacked`, `IsMainProgram` | Classify program graph boundaries. |
| `Identification` | Underlying `ProgramIdentification` code element. |
| `NestedPrograms` | Direct nested programs. |
| `ProcStyleCalls` | Procedure-style calls collected by generator/qualification logic. |

## Procedure Division

`ProcedureDivision` is the container for executable COBOL flow.

Important members:

| Member | Flow use |
| --- | --- |
| `ID` | Always `procedure-division`. |
| `CodeElement` | `ProcedureDivisionHeader`, including entry point information through code element call-target metadata. |
| `Children` | Declaratives, sections, paragraphs, sentences, and statements. |

CFG generation starts when a `ProcedureDivision` node is encountered.

## Sections, Paragraphs, Sentences

| Type | Flow role |
| --- | --- |
| `Section` | Procedure grouping and possible `PERFORM`/`GO TO` target. |
| `Paragraph` | Procedure grouping and common `PERFORM`/`GO TO` target. |
| `Sentence` | Sequence boundary inside paragraphs. Used by `NEXT SENTENCE`. |

Important members:

| Type | Members |
| --- | --- |
| `Section` | `Name`, `ID == "section"`, `CodeElement.SectionName` |
| `Paragraph` | `Name`, `ID == "paragraph"`, `CodeElement.ParagraphName` |
| `Sentence` | Generated `ID` based on child index |

For custom CFGs, create explicit labels for sections and paragraphs, then connect statement edges inside their child order.

## TypeCobol Function Nodes

| Type | Flow role |
| --- | --- |
| `FunctionDeclaration` | Function/procedure boundary, call target, nested generated program candidate. |
| `FunctionEnd` | End marker. |

Important `FunctionDeclaration` members:

| Member | Flow use |
| --- | --- |
| `Name` | Function name. |
| `QualifiedName` | Function name as URI. |
| `Library` / `Copy` | External copy/library provenance. |
| `Profile` | Input, inout, output, and returning parameters. |
| `ProcStyleCalls` | Procedure-style calls contained by the function. |
| `GenerateAsNested` | Generation behavior that may affect downstream COBOL shape. |

