using Microsoft.AspNetCore.Mvc;
using Service.MCPServer.Services;
using System.Text.Json;
using MaClasse.Shared.Models.Lesson;
using Service.MCPServer.Interfaces;

namespace Service.MCPServer.Controllers;

[ApiController]
[Route("api")]   
public class McpController : ControllerBase
{
    private readonly McpDispatcher _dispatcher;
    private readonly ILogger<McpController> _logger;
    private readonly GetAllDocumentsSummaryService _getAllDocumentsSummaryService;
    private readonly IOpenAIService _openAiService;

    public McpController(
        McpDispatcher dispatcher,
        ILogger<McpController> logger,
        GetAllDocumentsSummaryService getAllDocumentsSummaryService,
        IOpenAIService openAiService)
    {
        _dispatcher = dispatcher;
        _logger = logger;
        _getAllDocumentsSummaryService = getAllDocumentsSummaryService;
        _openAiService = openAiService;
    }

    [HttpPost]
    [Route("create_summary")] 
    public async Task<IActionResult> HandleMcpRequest([FromBody] JsonElement mcpMessage)
    {
        // Log de sécurité pour voir si la requête touche le serveur
        _logger.LogInformation("Requête reçue sur /mcp. Payload: {RawJson}", mcpMessage.GetRawText());

        try
        {
            // 1. Validation du format JSON-RPC
            if (!mcpMessage.TryGetProperty("method", out var methodElement))
            {
                _logger.LogWarning("Propriété 'method' manquante dans le JSON.");
                return BadRequest(new { error = "Method property is required" });
            }

            var method = methodElement.GetString();
            
            // On récupère l'ID (important pour que le client sache quelle requête est traitée)
            mcpMessage.TryGetProperty("id", out var id);
            
            // On récupère les params (@params dans le client)
            var parameters = mcpMessage.TryGetProperty("params", out var p) ? p : default;

            _logger.LogInformation("Exécution de la méthode MCP: {Method}", method);

            // 2. Appel du Dispatcher (qui appelle ensuite OpenAI)
            var result = await _dispatcher.Dispatch(method!, parameters);

            // 3. Réponse formatée JSON-RPC 2.0
            return Ok(new
            {
                jsonrpc = "2.0",
                id = id,
                result = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur critique lors du traitement MCP");
            return StatusCode(500, new { 
                jsonrpc = "2.0", 
                error = new { message = ex.Message } 
            });
        }
    }

    [HttpPost]
    [Route("advanced_search")]
    public async Task<IActionResult> AdvancedSearch([FromBody] RequestDocuments request)
    {
        // Récupérer TOUS les documents de l'utilisateur en BDD
        
        var allDocuments = await _getAllDocumentsSummaryService.GetAllDocuments(request);

        // 2. Demander à l'IA de choisir les meilleurs
        var matchingIds = await _openAiService.FindMatchingDocumentsAsync(request.AdvancedSearch, allDocuments);

        // 3. Filtrer la liste complète pour ne renvoyer que les objets Document complets
        var results = allDocuments.Where(d => matchingIds.Contains(d.IdDocument)).ToList();

        
        return Ok(results); }
    
}