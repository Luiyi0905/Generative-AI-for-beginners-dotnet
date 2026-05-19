using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Writer;
using System.Text;

namespace PdfTableExtractor.Services;

public class PdfService
{
    /// <summary>
    /// Gets the total number of pages in a PDF document.
    /// </summary>
    public int GetPageCount(Stream pdfStream)
    {
        if (pdfStream == null) throw new ArgumentNullException(nameof(pdfStream));
        using var document = PdfDocument.Open(pdfStream);
        return document.NumberOfPages;
    }

    /// <summary>
    /// Extracts text content from specific pages of a PDF.
    /// </summary>
    public string ExtractTextFromPages(byte[] pdfBytes, List<int> pageNumbers)
    {
        if (pdfBytes == null) throw new ArgumentNullException(nameof(pdfBytes));
        if (pageNumbers == null || pageNumbers.Count == 0) return string.Empty;

        var sb = new StringBuilder();
        using var document = PdfDocument.Open(pdfBytes);

        foreach (var pageNumber in pageNumbers.OrderBy(n => n))
        {
            if (pageNumber > 0 && pageNumber <= document.NumberOfPages)
            {
                var page = document.GetPage(pageNumber);
                sb.AppendLine($"--- Página {pageNumber} ---");
                sb.AppendLine(page.Text);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Creates a new PDF containing only the specified pages.
    /// </summary>
    public byte[] ExtractSpecificPages(byte[] pdfBytes, List<int> pageNumbers)
    {
        if (pdfBytes == null) throw new ArgumentNullException(nameof(pdfBytes));
        if (pageNumbers == null || pageNumbers.Count == 0) return pdfBytes;

        using var document = PdfDocument.Open(pdfBytes);
        var builder = new PdfDocumentBuilder();

        foreach (var pageNumber in pageNumbers.OrderBy(n => n))
        {
            if (pageNumber > 0 && pageNumber <= document.NumberOfPages)
            {
                builder.AddPage(document, pageNumber);
            }
        }

        return builder.Build();
    }
}
