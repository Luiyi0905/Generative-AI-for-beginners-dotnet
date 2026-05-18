using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Text;

namespace PdfTableExtractor.Services;

public class PdfService
{
    public int GetPageCount(Stream pdfStream)
    {
        using var document = PdfDocument.Open(pdfStream);
        return document.NumberOfPages;
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

    public byte[] GetPageAsImage(Stream pdfStream, int pageNumber)
    {
        // Note: PdfPig doesn't natively render pages to images easily without extra deps like SkiaSharp or ImageSharp.
        // For this POC/App, we will focus on text-based table extraction which GPT-4o is very good at.
        // If images were strictly required for OCR of scanned PDFs, we'd add SkiaSharp.
        return Array.Empty<byte>();
    }
}
