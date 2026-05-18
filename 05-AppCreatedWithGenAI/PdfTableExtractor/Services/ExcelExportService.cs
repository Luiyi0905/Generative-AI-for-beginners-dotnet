using ClosedXML.Excel;
using PdfTableExtractor.Models;

namespace PdfTableExtractor.Services;

public class ExcelExportService
{
    public byte[] ExportToExcel(ExtractionResult data)
    {
        using var workbook = new XLWorkbook();

        if (data.Tables == null || data.Tables.Count == 0)
        {
            var worksheet = workbook.Worksheets.Add("Resumen");
            worksheet.Cell(1, 1).Value = "No se encontraron tablas para extraer.";
            worksheet.Cell(2, 1).Value = data.Summary;
        }
        else
        {
            foreach (var table in data.Tables)
            {
                string sheetName = string.IsNullOrWhiteSpace(table.TableName) ? "Tabla" : table.TableName;
                sheetName = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName;

                int counter = 1;
                string originalName = sheetName;
                while (workbook.Worksheets.Contains(sheetName))
                {
                    sheetName = $"{originalName}_{counter++}";
                    if (sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);
                }

                var worksheet = workbook.Worksheets.Add(sheetName);

                // Set default column widths to 15 as requested
                worksheet.Columns().Width = 15;

                // Add Headers
                for (int i = 0; i < table.Headers.Count; i++)
                {
                    var cell = worksheet.Cell(1, i + 1);
                    string headerText = table.Headers[i];
                    cell.Value = headerText;
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Specific width for Description column
                    if (headerText.Contains("Descripción", StringComparison.OrdinalIgnoreCase) ||
                        headerText.Contains("Description", StringComparison.OrdinalIgnoreCase))
                    {
                        worksheet.Column(i + 1).Width = 25;
                    }
                }

                // Add Rows
                for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    var rowData = table.Rows[rowIndex];
                    for (int colIndex = 0; colIndex < rowData.Count; colIndex++)
                    {
                        worksheet.Cell(rowIndex + 2, colIndex + 1).Value = rowData[colIndex];
                    }
                }
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
