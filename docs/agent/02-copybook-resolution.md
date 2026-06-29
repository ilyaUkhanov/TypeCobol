# COPYBOOK Resolution

COPYBOOK directories are passed through the `copies` parameter of `Parser.Parse(...)` or `Parser.Init(...)`.

```csharp
var parser = Parser.Parse(
    path: sourcePath,
    isCopy: false,
    options: options,
    format: DocumentFormat.RDZReferenceFormat,
    copies: new[] { "/repo/copybooks", "/repo/common-copybooks" });
```

The parser registers each directory as a local copy library using TypeCobol's default copy extensions and the selected `DocumentFormat` encoding, line delimiter, and fixed line length.

## Missing COPYBOOKs

After parsing, read:

```csharp
var missingCopies = parser.MissingCopys?.ToArray() ?? Array.Empty<string>();
```

Treat missing COPYBOOKs as first-class export metadata. They affect AST completeness and can explain downstream diagnostics.

## Halt Behavior

`TypeCobolOptions.HaltOnMissingCopy` controls whether compilation stops before semantic phases when the preprocessor sees missing copies.

For agent documentation and RAG extraction, `false` is usually better because it allows TypeCobol to produce as much syntax and diagnostic information as possible. For strict batch validation, `true` may be preferable.

