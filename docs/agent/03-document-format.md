# Document Format

`DocumentFormat` defines how TypeCobol reads a source file:

- `Encoding`
- `EndOfLineDelimiter`
- `FixedLineLength`
- `ColumnsLayout`

This is not cosmetic. COBOL reference format uses column positions to determine sequence numbers, indicator area, Area A, Area B, and comments.

## Common Formats

| Format | Meaning | Use when |
| --- | --- | --- |
| `DocumentFormat.ZOsReferenceFormat` | IBM code page 1147, fixed 80-character records, COBOL reference columns. | Mainframe z/OS-style source files. |
| `DocumentFormat.RDZReferenceFormat` | UTF-8, CRLF lines, COBOL reference columns. | RDz or workstation files that preserve fixed/reference columns. |
| `DocumentFormat.FreeTextFormat` | Windows-1252, CRLF lines, free text columns. | Free-format source encoded with code page 1252. |
| `DocumentFormat.FreeUTF8Format` | UTF-8, CRLF lines, free text columns. | Free-format UTF-8 source. |

## Agent Guidance

Start with `RDZReferenceFormat` or `ZOsReferenceFormat` for classic COBOL. Use `FreeUTF8Format` only when the corpus is truly free-format COBOL.

Wrong format choices often surface as misleading parser diagnostics because tokens are read from the wrong COBOL areas.

