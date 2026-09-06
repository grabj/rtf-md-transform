using System.Text;
using ReverseMarkdown;
using RtfPipe;

Console.OutputEncoding = Encoding.UTF8;
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

if (args is not [var input, ..])
{
    PrintUsage();
    return 1;
}

var output = args.Length > 1 ? args[1] : null;
var converter = new Converter(new Config
{
    GithubFlavored = true,
    RemoveComments = true,
    SmartHrefHandling = true,
    UnknownTags = Config.UnknownTagsOption.PassThrough,
});

try
{
    if (Directory.Exists(input))
    {
        ConvertDirectory(input, output, converter);
    }
    else if (File.Exists(input))
    {
        ConvertFile(input, ResolveOutputFile(input, output), converter);
    }
    else
    {
        Console.Error.WriteLine($"Nie znaleziono: {input}");
        return 1;
    }
}
catch (Exception ex)
{
    PrintException(ex);
    return 1;
}

return 0;

static void ConvertDirectory(string inputDir, string? outputDir, Converter converter)
{
    inputDir = Path.GetFullPath(inputDir);
    outputDir = Path.GetFullPath(outputDir ?? inputDir);
    Directory.CreateDirectory(outputDir);

    var files = Directory.GetFiles(inputDir, "*.rtf", SearchOption.AllDirectories);
    if (files.Length == 0)
    {
        Console.WriteLine("Brak plików .rtf w katalogu (łącznie z podkatalogami).");
        return;
    }

    foreach (var file in files)
    {
        var relative = Path.GetRelativePath(inputDir, file);
        var dest = Path.Combine(outputDir, Path.ChangeExtension(relative, ".md"));
        try
        {
            ConvertFile(file, dest, converter);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Błąd: {file}");
            PrintException(ex);
        }
    }
}

static void ConvertFile(string rtfPath, string mdPath, Converter converter)
{
    var rtf = File.ReadAllText(rtfPath, Encoding.Latin1);
    var html = Rtf.ToHtml(rtf);
    var markdown = converter.Convert(html).Trim() + Environment.NewLine;

    Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(mdPath))!);
    File.WriteAllText(mdPath, markdown, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    Console.WriteLine($"{Path.GetFileName(rtfPath)} -> {mdPath}");
}

static string ResolveOutputFile(string rtfPath, string? output)
{
    if (string.IsNullOrWhiteSpace(output))
        return Path.ChangeExtension(rtfPath, ".md");

    if (Directory.Exists(output) || output.EndsWith(Path.DirectorySeparatorChar) || output.EndsWith(Path.AltDirectorySeparatorChar))
        return Path.Combine(output, Path.GetFileNameWithoutExtension(rtfPath) + ".md");

    return output;
}

static void PrintException(Exception ex)
{
    for (var e = ex; e is not null; e = e.InnerException)
        Console.Error.WriteLine(e.Message);
}

static void PrintUsage()
{
    Console.WriteLine("""
        Konwersja RTF → Markdown

        Użycie:
          rtf-md-transform.exe <wejście> [wyjście]

        Argumenty:
          wejście    Wymagane: plik .rtf albo katalog z plikami .rtf
          wyjście    Opcjonalnie: plik .md albo katalog wynikowy

        Zachowanie:
          • podkatalogi i struktura folderów są zachowane
          • pliki .rtf nie są usuwane

        Działanie:
          • wejście: plik,                          — note.rtf → note.md (ten sam katalog)
          • wejście: katalog,                       — każdy .rtf → .md obok siebie (z podkatalogami)
          • wejście: plik,    wyjście: plik         — note.rtf wynik.md
          • wejście: plik,    wyjście: katalog      — note.rtf .\md\ → .\md\note.md
          • wejście: katalog, wyjście: katalog      — .\notes .\notes-md (struktura zachowana)

        Przykłady:
          rtf-md-transform.exe note.rtf
          rtf-md-transform.exe ".\notes"
          rtf-md-transform.exe note.rtf wynik.md
          rtf-md-transform.exe note.rtf ".\md\"
          rtf-md-transform.exe ".\notes" ".\notes-md"
        """);
}
