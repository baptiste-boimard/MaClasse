using MaClasse.Shared.Models.Files;

namespace Service.MCPServer.Interfaces;

public interface IOpenAIService
{
  Task<string> AnalyzeWithGptAsync(McpAnalysisRequest args);
  Task<List<string>> FindMatchingDocumentsAsync(string userQuery, List<Document> allDocuments);
  Task<string> DetectAdvancedSearchIntentAsync(string userQuery);
}
