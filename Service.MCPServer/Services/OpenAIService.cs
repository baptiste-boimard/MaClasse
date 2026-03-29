using System.Text.Json;
using MaClasse.Shared.Models.Files;
using OpenAI.Chat;
using Service.MCPServer.Interfaces;

namespace Service.MCPServer.Services;

public class OpenAIService : IOpenAIService
{
    private readonly ChatClient _client;
    private readonly string _apiKey;

    public OpenAIService(IConfiguration configuration)
    {
        // Récupération de la clé depuis appsettings.json
        _apiKey = configuration["OpenAI:ApiKey"] 
                  ?? throw new ArgumentNullException("La clé OpenAI est manquante dans la configuration !");
        
        // Initialisation unique du client pour toute la durée de vie du service
        _client = new ChatClient(model: "gpt-4o", apiKey: _apiKey);
    }
    
    public async Task<string> AnalyzeWithGptAsync(McpAnalysisRequest args)
    {
        if (args.FileType.ToLower() == "image")
        {
            if (string.IsNullOrEmpty(args.Url)) return "Erreur : URL de l'image manquante.";

            // CAS IMAGE : analyse centrée sur la couverture (visuel principal + titre + thème)
            var messages = new ChatMessage[]
            {
                new SystemChatMessage(
                    "Tu analyses des couvertures de documents. " +
                    "Donne un résumé très concis du visuel principal (illustration, titre/texte lisible, thème). " +
                    "Réponds en français, sans liste, en une seule phrase, 250 caractères maximum."),
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart("Résume cette couverture en te concentrant sur ce qu'on voit dessus."),
                    ChatMessageContentPart.CreateImagePart(new Uri(args.Url)))
            };

            ChatCompletion completion = await _client.CompleteChatAsync(messages);
            var text = completion.Content[0].Text ?? string.Empty;
            return EnsureMaxLength(text, 250);
        }
        else if (args.FileType.ToLower() == "pdf")
        {
            if (string.IsNullOrEmpty(args.Url)) return "Erreur : URL du PDF manquante.";

            var messages = new ChatMessage[]
            {
                new SystemChatMessage(
                    "Tu résumes des documents pédagogiques. " +
                    "Le document provient de Lükla (visible sur la couverture). " +
                    "Lis attentivement le contenu fourni et produis un résumé de 250 caractères.\n" +
                    "Contraintes :\n- 1 seule phrase\n- inclure : sujet + objectifs + méthodes/outils + cadre\n" +
                    "- inclure explicitement le nom de l’organisation si présent (ex : Lükla)\n" +
                    "- style professionnel et naturel\n" +
                    "- éviter les répétitions\n" +
                    "- prioriser les infos clés"),
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart(
                        $"Analyse ce document PDF nommé {args.Name} via cette URL: {args.Url}. Fais un résumé très court."))
            };

            ChatCompletion completion = await _client.CompleteChatAsync(messages);
            var text = completion.Content[0].Text ?? string.Empty;
            return EnsureMaxLength(text, 250);
        }

        return "Type de fichier non supporté.";
    }

    private static string EnsureMaxLength(string text, int maxChars)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var normalized = text.Trim().Replace("\r", " ").Replace("\n", " ");
        normalized = string.Join(" ", normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        if (normalized.Length <= maxChars)
        {
            return normalized;
        }

        return normalized[..maxChars].TrimEnd();
    }
    
    public async Task<List<string>> FindMatchingDocumentsAsync(string userQuery, List<Document> allDocuments)
    {
        // 1. On prépare une liste ultra-légère pour l'IA (ID + Résumé uniquement)
        var documentListForAi = allDocuments
            .Where(d => !string.IsNullOrWhiteSpace(d.Summary)) // Sécurité : on ignore les docs sans résumé
            .Select(d => new { d.IdDocument, d.Summary })
            .ToList();
    
        if (!documentListForAi.Any()) return new List<string>();
    
        // On convertit cette liste en une chaîne JSON compacte pour le prompt
        var jsonDocuments = JsonSerializer.Serialize(documentListForAi);
    
        // 2. Construction du System Prompt pour "cadrer" l'IA
        string systemPrompt = @"Tu es un expert en recherche sémantique.
        Je vais te donner une requête utilisateur en langage naturel et une liste JSON de documents avec leurs résumés.
        Ton but est d'identifier les documents dont le résumé correspond le mieux au sens de la requête.
        Tu dois retourner UNIQUEMENT un tableau JSON contenant les IdDocument correspondants.
        Si aucun document ne correspond, retourne un tableau vide [].
        Ne réponds JAMAIS avec du texte d'explication, UNIQUEMENT le JSON : [""id1"", ""id2""]";
    
        // 3. Construction du User Prompt avec les données
        string userPrompt = $"Requête de l'utilisateur : {userQuery} \n\n Liste des documents : {jsonDocuments}";
    
        // 4. Appel de l'API OpenAI
        try 
        {
            var messages = new ChatMessage[]
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userPrompt)
            };
    
            ChatCompletion completion = await _client.CompleteChatAsync(messages);
    
            // 5. Extraction et nettoyage du JSON reçu
            string rawJson = completion.Content[0].Text.Trim();
    
            // Parfois l'IA ajoute des blocs ```json ... ```, on les nettoie
            if (rawJson.StartsWith("```json")) rawJson = rawJson.Replace("```json", "").Replace("```", "").Trim();
    
            // 6. Désérialisation pour récupérer les IDs
            return JsonSerializer.Deserialize<List<string>>(rawJson) ?? new List<string>();
        }
        catch (JsonException ex)
        {
            // Erreur de formatage de l'IA (rare avec un bon prompt)
            Console.WriteLine($"Erreur désérialisation OpenAI: {ex.Message}. Raw: {userPrompt}");
            return new List<string>();
        }
        catch (Exception ex)
        {
            // Autre erreur API
            Console.WriteLine($"Erreur API OpenAI: {ex.Message}");
            return new List<string>();
        }
    }

    public async Task<string> DetectAdvancedSearchIntentAsync(string userQuery)
    {
        if (string.IsNullOrWhiteSpace(userQuery))
        {
            return "none";
        }

        const string systemPrompt = @"Tu classes une requête de recherche de documents.
            Réponds UNIQUEMENT avec un JSON strict au format:
            {""intent"":""all_images|all_pdfs|none""}
            Règles:
            - all_images: l'utilisateur veut voir toutes les images
            - all_pdfs: l'utilisateur veut voir tous les PDF
            - none: tout autre cas.";

        try
        {
            var messages = new ChatMessage[]
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage($"Requête: {userQuery}")
            };

            ChatCompletion completion = await _client.CompleteChatAsync(messages);
            var raw = completion.Content[0].Text.Trim();

            if (raw.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            {
                raw = raw.Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("```", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();
            }

            using var jsonDoc = JsonDocument.Parse(raw);
            var intent = jsonDoc.RootElement.GetProperty("intent").GetString()?.Trim().ToLowerInvariant();

            return intent is "all_images" or "all_pdfs" ? intent : "none";
        }
        catch
        {
            return DetectAdvancedSearchIntentFallback(userQuery);
        }
    }

    private static string DetectAdvancedSearchIntentFallback(string userQuery)
    {
        var normalized = userQuery.Trim().ToLowerInvariant();

        if (normalized.Contains("image") &&
            (normalized.Contains("toutes") || normalized.Contains("tous") || normalized.Contains("tout")))
        {
            return "all_images";
        }

        if (normalized.Contains("pdf") &&
            (normalized.Contains("toutes") || normalized.Contains("tous") || normalized.Contains("tout")))
        {
            return "all_pdfs";
        }

        return "none";
    }
}
