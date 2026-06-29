using System.Text.Json;
using TypeCobol;
using TypeCobol.Compiler;
using TypeCobol.Compiler.Directives;
using TypeCobol.Compiler.Nodes;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: BasicAstJsonExporter <program.cbl> [copybook-dir-1] [copybook-dir-2] ...");
    Environment.Exit(1);
}

var sourcePath = args[0];
var copyDirectories = args.Skip(1).ToList();

var options = new TypeCobolOptions
{
    IsCobolLanguage = true,
    EnableSqlParsing = false,
    HaltOnMissingCopy = false,
    OptimizeWhitespaceScanning = true
};

var parser = Parser.Parse(
    path: sourcePath,
    isCopy: false,
    options: options,
    format: DocumentFormat.RDZReferenceFormat,
    copies: copyDirectories);

var compilationUnit = parser.Results;
var root = compilationUnit.ProgramClassDocumentSnapshot?.Root
    ?? compilationUnit.TemporaryProgramClassDocumentSnapshot?.Root;

var payload = new AstExportDto
{
    ParserVersion = Parser.Version,
    Source = Path.GetFullPath(sourcePath),
    CopyDirectories = copyDirectories,
    MissingCopies = parser.MissingCopys?.ToArray() ?? Array.Empty<string>(),
    Diagnostics = compilationUnit.AllDiagnostics().Select(d => d.ToString()).ToArray(),
    Ast = root == null ? null : ToDto(root)
};

var json = JsonSerializer.Serialize(
    payload,
    new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

var outputPath = Path.ChangeExtension(sourcePath, ".ast.json");
File.WriteAllText(outputPath, json);
Console.WriteLine($"AST JSON written to: {outputPath}");

static AstNodeDto ToDto(Node node)
{
    return new AstNodeDto
    {
        ClrType = node.GetType().FullName,
        Id = node.ID,
        Uri = node.URI,
        Name = string.IsNullOrWhiteSpace(node.Name) ? null : node.Name,
        QualifiedName = node.QualifiedName?.ToString(),
        VisualQualifiedName = node.VisualQualifiedName?.ToString(),
        Flags = node.Flags.ToString(),
        CodeElementType = node.CodeElement?.GetType().FullName,
        CodeElementText = node.CodeElement?.ToString(),
        Diagnostics = node.Diagnostics?.Select(d => d.ToString()).ToArray() ?? Array.Empty<string>(),
        SourceLines = node.Lines?
            .Select(line => line?.ToString())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => line!)
            .ToArray() ?? Array.Empty<string>(),
        Children = node.Children.Select(ToDto).ToList()
    };
}

internal sealed class AstExportDto
{
    public string ParserVersion { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public IReadOnlyList<string> CopyDirectories { get; set; } = Array.Empty<string>();
    public string[] MissingCopies { get; set; } = Array.Empty<string>();
    public string[] Diagnostics { get; set; } = Array.Empty<string>();
    public AstNodeDto? Ast { get; set; }
}

internal sealed class AstNodeDto
{
    public string? ClrType { get; set; }
    public string? Id { get; set; }
    public string? Uri { get; set; }
    public string? Name { get; set; }
    public string? QualifiedName { get; set; }
    public string? VisualQualifiedName { get; set; }
    public string? Flags { get; set; }
    public string? CodeElementType { get; set; }
    public string? CodeElementText { get; set; }
    public string[] Diagnostics { get; set; } = Array.Empty<string>();
    public string[] SourceLines { get; set; } = Array.Empty<string>();
    public List<AstNodeDto> Children { get; set; } = new();
}
