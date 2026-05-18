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
            // Placeholder for when API Key is not yet configured, to avoid crash on startup
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
            You are an expert data extractor. Your task is to identify and extract all tables from the provided text content of a PDF.
            Return the data as a JSON object that matches this structure:
            {
              "tables": [
                {
                  "tableName": "Description of the table",
                  "headers": ["Column1", "Column2", ...],
                  "rows": [
                    ["Value1", "Value2", ...],
                    ["Value1", "Value2", ...]
                  ]
                }
              ],
              "summary": "Short summary of the extracted data"
            }
            If no tables are found, return an empty list of tables.
            Ensure the JSON is valid and only return the JSON, no markdown formatting.
            """);

        history.AddUserMessage($"Extract tables from the following PDF content:\n\n{pdfText}");

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
            return new ExtractionResult { Summary = "Error parsing AI response." };
        }
    }
}
