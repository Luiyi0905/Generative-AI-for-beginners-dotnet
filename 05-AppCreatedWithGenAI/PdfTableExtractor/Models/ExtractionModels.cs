namespace PdfTableExtractor.Models;

public class ExtractedTable
{
    public string TableName { get; set; } = "Table";
    public List<string> Headers { get; set; } = new();
    public List<List<string>> Rows { get; set; } = new();
}

public class ExtractionResult
{
    public List<ExtractedTable> Tables { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
}
