using PdfTableExtractor.Services;
using PdfTableExtractor.Models;
using System.Text;

namespace PdfTableExtractor.Tests;

public class ExcelExportServiceTests
{
    [Fact]
    public void ExportToExcel_WithData_ReturnsNonEmptyBytes()
    {
        // Arrange
        var service = new ExcelExportService();
        var data = new ExtractionResult
        {
            Tables = new List<ExtractedTable>
            {
                new ExtractedTable
                {
                    TableName = "TestTable",
                    Headers = new List<string> { "Col1", "Col2" },
                    Rows = new List<List<string>>
                    {
                        new List<string> { "V1", "V2" },
                        new List<string> { "V3", "V4" }
                    }
                }
            },
            Summary = "Test Summary"
        };

        // Act
        var result = service.ExportToExcel(data);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void ExportToExcel_EmptyData_ReturnsNonEmptyBytes()
    {
        // Arrange
        var service = new ExcelExportService();
        var data = new ExtractionResult();

        // Act
        var result = service.ExportToExcel(data);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }
}
