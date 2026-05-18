using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Writer;
using System.Text;

namespace PdfTableExtractor.Services;

public class PdfService
{
    public int GetPageCount(Stream pdfStream)
    {
        using var document = PdfDocument.Open(pdfStream);
        return document.NumberOfPages;
    }

    public byte[] ExtractSpecificPages(byte[] pdfBytes, List<int> pageNumbers)
    {
        using var document = PdfDocument.Open(pdfBytes);
        var builder = new PdfDocumentBuilder();

        foreach (var pageNumber in pageNumbers)
        {
            if (pageNumber > 0 && pageNumber <= document.NumberOfPages)
            {
                builder.AddPage(document, pageNumber);
            }
        }

        return builder.Build();
    }

    public string ExtractTextFromPages(Stream pdfStream, List<int> pageNumbers)
    {
        var sb = new StringBuilder();
        using var document = PdfDocument.Open(pdfStream);

        foreach (var pageNumber in pageNumbers)
        {
            if (pageNumber > 0 && pageNumber <= document.NumberOfPages)
            {
                var page = document.GetPage(pageNumber);
                sb.AppendLine($"--- Page {pageNumber} ---");
                sb.AppendLine(page.Text);
            }
        }

        return sb.ToString();
    }
}
