# TypeCobol.Compiler.DocumentFormat

Source layout contract used when loading COBOL files.

Declared in `TypeCobol/Compiler/DocumentFormat.cs`.

## Constructor

```csharp
public DocumentFormat(
    Encoding encoding,
    EndOfLineDelimiter endOfLineDelimiter,
    int fixedLineLength,
    ColumnsLayout columnsLayout)
```

When `EndOfLineDelimiter.FixedLengthLines` is used, fixed length is validated against the selected column layout.

## Properties

| Property | Meaning |
| --- | --- |
| `Encoding` | File encoding used to read/write source. |
| `EndOfLineDelimiter` | How the character stream is split into lines. |
| `FixedLineLength` | Record length for fixed-length sources. |
| `ColumnsLayout` | How character positions are interpreted. |

## Common Static Formats

| Member | Meaning |
| --- | --- |
| `ZOsReferenceFormat` | CCSID 1147, fixed 80-character records, COBOL reference format. |
| `RDZReferenceFormat` | UTF-8 CRLF, COBOL reference format. |
| `FreeTextFormat` | Windows-1252 CRLF, free text format. |
| `FreeUTF8Format` | UTF-8 CRLF, free text format. |

