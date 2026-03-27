namespace Service.MCPServer.Interfaces;

public interface IOpenAIService
{
  Task<string> GenerateSummaryAsync(string prompt);
  Task<string> GetDocFromSummaryAsync(string prompt);
  Task<string> AnalyzeWithGptAsync(McpAnalysisRequest args);
}