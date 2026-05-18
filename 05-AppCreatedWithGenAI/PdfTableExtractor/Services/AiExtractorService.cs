using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using PdfTableExtractor.Models;
using System.Text.Json;

namespace PdfTableExtractor.Services;

public class AiExtractorService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;

    public AiExtractorService(IConfiguration configuration)
    {
        string modelId = configuration["OpenAI:ModelId"] ?? "gpt-4o";
        string apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;

        var builder = Kernel.CreateBuilder();
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            builder.AddOpenAIChatCompletion(modelId, apiKey);
        }
        else
        {
            builder.AddOpenAIChatCompletion(modelId, "MISSING_API_KEY");
        }
        _kernel = builder.Build();
        _chatService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<ExtractionResult> ExtractTablesAsync(string pdfText)
    {
        var history = new ChatHistory();
        history.AddSystemMessage(
            """
            Eres un experto en extracción de datos financieros y tablas.
            Tu tarea es identificar y extraer TODAS las tablas del contenido de texto del PDF proporcionado.
            Debes ser extremadamente preciso con las columnas y filas.
            Si una descripción ocupa varias líneas, únelas en una sola celda.

            Devuelve los datos como un objeto JSON con esta estructura:
            {
              "tables": [
                {
                  "tableName": "Descripción de la tabla (ej: Transacciones Bancarias)",
                  "headers": ["Fecha", "Descripción", "Referencia", "Cargo", "Abono", "Saldo"],
                  "rows": [
                    ["01/09", "DEPOSITO 002981215", "", "", "8,054.77", "25,528.44"],
                    ...
                  ]
                }
              ],
              "summary": "Breve resumen de los datos extraídos"
            }
            Asegúrate de que el JSON sea válido y solo devuelve el JSON. No incluyas explicaciones.
            """);

        history.AddUserMessage($"Extrae todas las tablas del siguiente contenido de texto de un PDF con la máxima precisión:\n\n{pdfText}");

        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = "json_object"
        };

        var response = await _chatService.GetChatMessageContentAsync(history, executionSettings);

        try
        {
            var result = JsonSerializer.Deserialize<ExtractionResult>(response.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new ExtractionResult();
        }
        catch (JsonException)
        {
            return new ExtractionResult { Summary = "Error al analizar la respuesta de la IA. El documento puede ser demasiado complejo." };
        }
    }
}
