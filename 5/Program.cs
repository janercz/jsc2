using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

var baseDir = AppContext.BaseDirectory;
var projectDir = FindProjectDirectory(baseDir) ?? Directory.GetCurrentDirectory();

var booksPath = Path.Combine(projectDir, "books.json");
var xsdPath = Path.Combine(projectDir, "library.xsd");
var outputPath = Path.Combine(projectDir, "library.xml");

if (!File.Exists(booksPath))
{
    throw new FileNotFoundException($"Soubor nebyl nalezen: {booksPath}", booksPath);
}

if (!File.Exists(xsdPath))
{
    throw new FileNotFoundException($"Soubor nebyl nalezen: {xsdPath}", xsdPath);
}


var json = await File.ReadAllTextAsync(booksPath, Encoding.UTF8);
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
};

var books = JsonSerializer.Deserialize<List<Book>>(json, options) ?? new List<Book>();


var validationErrors = ValidateBooks(books);
if (validationErrors.Count > 0)
{
    Console.Error.WriteLine("Validace knih selhala:");
    foreach (var error in validationErrors)
    {
        Console.Error.WriteLine($"- {error}");
    }

    Environment.Exit(2);
}


var grouped = books
    .GroupBy(b => b.Genre!.Trim())
    .OrderBy(g => g.Key, StringComparer.CurrentCulture)
    .Select(g => new
    {
        Genre = g.Key,
        Books = g
            .OrderByDescending(b => b.Rating)
            .ThenBy(b => b.Year)
            .ToList(),
        Count = g.Count(),
        AvgRating = g.Average(b => b.Rating)
    })
    .ToList();


var xml = new XDocument(
    new XDeclaration("1.0", "utf-8", null),
    new XElement("Library",
        grouped.Select(group =>
            new XElement("Genre",
                new XAttribute("name", group.Genre),
                new XAttribute("count", group.Count),
                new XAttribute("avgRating", group.AvgRating.ToString("0.00", CultureInfo.InvariantCulture)),
                group.Books.Select(book =>
                    new XElement("Book",
                        new XElement("Title", book.Title),
                        new XElement("Author", book.Author),
                        new XElement("Year", book.Year),
                        new XElement("ISBN", book.Isbn),
                        new XElement("Rating", book.Rating.ToString("0.00", CultureInfo.InvariantCulture)),
                        new XElement("Pages", book.Pages)
                    )
                )
            )
        )
    )
);


var writerSettings = new XmlWriterSettings
{
    Indent = true,
    Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
    NewLineChars = Environment.NewLine,
    NewLineHandling = NewLineHandling.Replace
};

using (var writer = XmlWriter.Create(outputPath, writerSettings))
{
    xml.Save(writer);
}


var schemaSet = new XmlSchemaSet();
schemaSet.Add(null, xsdPath);

var readerSettings = new XmlReaderSettings
{
    ValidationType = ValidationType.Schema,
    Schemas = schemaSet
};

var xsdErrors = new List<string>();
readerSettings.ValidationEventHandler += (sender, args) =>
{
    xsdErrors.Add($"[VALIDACE] {args.Severity}: {args.Message}");
};

using (var reader = XmlReader.Create(outputPath, readerSettings))
{
    while (reader.Read())
    {
    }
}

if (xsdErrors.Count > 0)
{
    Console.Error.WriteLine("XML není validní podle XSD:");
    foreach (var error in xsdErrors)
    {
        Console.Error.WriteLine($"- {error}");
    }

    Environment.Exit(3);
}

Console.WriteLine("Hotovo: knihy validovány, XML vygenerováno a ověřeno proti XSD.");
Console.WriteLine($"Výstup: {outputPath}");


static List<string> ValidateBooks(List<Book> books)
{
    var errors = new List<string>();
    var currentYear = DateTime.Now.Year;

    var isbnPattern = new Regex(@"^(?:97[89]\d{10}|\d{9}[\dX])$", RegexOptions.CultureInvariant);

    for (var i = 0; i < books.Count; i++)
    {
        var b = books[i];
        var index = i + 1;

        if (string.IsNullOrWhiteSpace(b.Title)) errors.Add($"Položka #{index}: title je prázdný.");
        if (string.IsNullOrWhiteSpace(b.Author)) errors.Add($"Položka #{index}: author je prázdný.");
        if (string.IsNullOrWhiteSpace(b.Genre)) errors.Add($"Položka #{index}: genre je prázdný.");
        
        if (b.Year < 1450 || b.Year > currentYear)
            errors.Add($"Položka #{index}: year {b.Year} je mimo rozsah 1450-{currentYear}.");

        if (string.IsNullOrWhiteSpace(b.Isbn) || !isbnPattern.IsMatch(b.Isbn.Trim()))
            errors.Add($"Položka #{index}: isbn '{b.Isbn}' nemá validní ISBN-10/13 formát.");

        if (b.Rating < 0.0m || b.Rating > 10.0m)
            errors.Add($"Položka #{index}: rating {b.Rating} je mimo rozsah 0.0-10.0.");

        if (b.Pages < 1)
            errors.Add($"Položka #{index}: pages {b.Pages} musí být >= 1.");
    }

    return errors;
}

static string? FindProjectDirectory(string startDirectory)
{
    var dir = new DirectoryInfo(startDirectory);

    while (dir is not null)
    {
        if (File.Exists(Path.Combine(dir.FullName, "books.json")) &&
            File.Exists(Path.Combine(dir.FullName, "library.xsd")))
        {
            return dir.FullName;
        }

        dir = dir.Parent;
    }

    return null;
}

public sealed class Book
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("isbn")]
    public string? Isbn { get; set; }

    [JsonPropertyName("genre")]
    public string? Genre { get; set; }

    [JsonPropertyName("rating")]
    public decimal Rating { get; set; }

    [JsonPropertyName("pages")]
    public int Pages { get; set; }
}