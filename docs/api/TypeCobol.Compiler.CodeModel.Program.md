# TypeCobol.Compiler.CodeModel.Program

COBOL program node family.

Declared in `TypeCobol/Compiler/CodeModel/Program.cs`.

## Types

| Type | Meaning |
| --- | --- |
| `Program` | Base node for a COBOL source program. |
| `SourceProgram` | Outermost program in a compilation unit. |
| `NestedProgram` | Program contained in another program. |
| `StackedProgram` | Additional source-level program. |

## Important Members

| Member | Meaning |
| --- | --- |
| `Name` | Program name from `PROGRAM-ID`, or `program` fallback. |
| `Hash` | 8-character COBOL-name hash based on the program name. |
| `Namespace` | Program namespace value from identification. |
| `IsNested` | True for nested programs. |
| `IsStacked` | True for stacked programs. |
| `IsMainProgram` | True for the main source program. |
| `Identification` | Program identification code element. |
| `NestedPrograms` | Direct nested program children. |
| `ContainingProgram` | Parent program for `NestedProgram`. |

For RAG, program nodes are strong chunk boundaries.

