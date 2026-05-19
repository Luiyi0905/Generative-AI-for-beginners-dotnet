using ClosedXML.Excel;
using PdfTableExtractor.Models;
using System.Globalization;

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
                string sheetName = CleanSheetName(table.TableName);

                int counter = 1;
                string originalName = sheetName;
                while (workbook.Worksheets.Contains(sheetName))
                {
                    sheetName = $"{originalName}_{counter++}";
                    if (sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);
                }

                var worksheet = workbook.Worksheets.Add(sheetName);
                worksheet.Columns().Width = 15;

                // Add Headers
                for (int i = 0; i < table.Headers.Count; i++)
                {
                    var cell = worksheet.Cell(1, i + 1);
                    string headerText = table.Headers[i];
                    cell.Value = headerText;
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.LightGray;

                    if (IsDescriptionColumn(headerText))
                    {
                        worksheet.Column(i + 1).Width = 35;
                    }
                }

                // Add Rows
                for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    var rowData = table.Rows[rowIndex];
                    for (int colIndex = 0; colIndex < rowData.Count; colIndex++)
                    {
                        var cell = worksheet.Cell(rowIndex + 2, colIndex + 1);
                        string cellValue = rowData[colIndex];
                        string headerText = table.Headers.Count > colIndex ? table.Headers[colIndex] : "";

                        if (IsNumericColumn(headerText) && TryParseNumeric(cellValue, out double numericValue))
                        {
                            cell.SetValue(numericValue);
                            cell.Style.NumberFormat.Format = "#,##0.00";
                        }
                        else
                        {
                            cell.SetValue(cellValue);
                        }
                    }
                }

                worksheet.SheetView.FreezeRows(1);
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private string CleanSheetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Tabla";
        var invalidChars = new[] { ':', '\\', '/', '?', '*', '[', ']' };
        string clean = name;
        foreach (var c in invalidChars) clean = clean.Replace(c, '_');
        return clean.Length > 30 ? clean.Substring(0, 30) : clean;
    }

    private bool IsDescriptionColumn(string header) =>
        header.Contains("Descripción", StringComparison.OrdinalIgnoreCase) ||
        header.Contains("Description", StringComparison.OrdinalIgnoreCase) ||
        header.Contains("Concepto", StringComparison.OrdinalIgnoreCase);

    private bool IsNumericColumn(string header) =>
        header.Contains("Cargo", StringComparison.OrdinalIgnoreCase) ||
        header.Contains("Abono", StringComparison.OrdinalIgnoreCase) ||
        header.Contains("Saldo", StringComparison.OrdinalIgnoreCase) ||
        header.Contains("Debit", StringComparison.OrdinalIgnoreCase) ||
        header.Contains("Credit", StringComparison.OrdinalIgnoreCase) ||
        header.Contains("Balance", StringComparison.OrdinalIgnoreCase) ||
        header.Contains("Monto", StringComparison.OrdinalIgnoreCase) ||
        header.Contains("Importe", StringComparison.OrdinalIgnoreCase) ||
        header.Contains("Amount", StringComparison.OrdinalIgnoreCase);

    private bool TryParseNumeric(string value, out double result)
    {
        result = 0;
        if (string.IsNullOrWhiteSpace(value)) return false;

        // Limpiar valor: quitar símbolos de moneda, espacios y comas de miles
        string cleanedValue = value.Replace("$", "").Replace("€", "").Replace(" ", "").Replace(",", "").Trim();

        // Si hay una coma pero no hay punto, asumimos que es el separador decimal
        if (value.Contains(",") && !value.Contains("."))
        {
            cleanedValue = value.Replace(",", ".").Replace("$", "").Replace("€", "").Replace(" ", "").Trim();
        }

        if (double.TryParse(cleanedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
        {
            return true;
        }

        return false;
    }
}
