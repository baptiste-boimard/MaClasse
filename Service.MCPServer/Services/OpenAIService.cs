using System.Reflection.Metadata;
using OpenAI.Chat;
using Service.MCPServer.Interfaces;
using UglyToad.PdfPig;

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

            // CAS IMAGE : On envoie l'URL Cloudinary directement à GPT-4o
            ChatMessage message = new UserChatMessage(
                ChatMessageContentPart.CreateTextPart("Peux-tu me faire un résumé détaillé en 250 caractères maximum de ce que tu vois sur cette image ?"),
                ChatMessageContentPart.CreateImagePart(new Uri(args.Url))
            );

            ChatCompletion completion = await _client.CompleteChatAsync(message);
            return completion.Content[0].Text;
        }
        else if (args.FileType.ToLower() == "pdf")
        {
            if (string.IsNullOrEmpty(args.Base64Content)) return "Erreur : Contenu PDF (Base64) manquant.";

            // CAS PDF : Extraction du texte depuis le Base64 reçu du microservice
            string pdfText = ExtractTextFromBase64(args.Base64Content);
            
            ChatMessage message = new UserChatMessage(
                $"Voici le contenu d'un document PDF nommé {args.Name}. Peux-tu en faire un résumé structuré ?\n\nContenu :\n{pdfText}");

            ChatCompletion completion = await _client.CompleteChatAsync(message);
            return completion.Content[0].Text;
        }

        return "Type de fichier non supporté.";
    }
    
    private string ExtractTextFromBase64(string base64Content)
    {
        try 
        {
            // Décodage du Base64 envoyé par le contrôleur Cloudinary
            byte[] pdfBytes = Convert.FromBase64String(base64Content);

            // Lecture du PDF en mémoire via PdfPig
            using var document = PdfDocument.Open(pdfBytes);
            
            // Extraction du texte de toutes les pages
            var text = string.Join(" ", document.GetPages().Select(p => p.Text));

            // Protection : On limite à 12 000 caractères pour respecter le contexte de l'IA
            return text.Length > 12000 ? text[..12000] : text;
        }
        catch (Exception ex)
        {
            return $"Erreur lors de l'extraction du texte PDF : {ex.Message}";
        }
    }
    
    // public async Task<List<string>> FindMatchingDocumentsAsync(string userQuery, List<Document> allDocuments)
    // {
    //     // 1. On prépare une liste ultra-légère pour l'IA (ID + Résumé uniquement)
    //     var documentListForAi = allDocuments
    //         .Where(d => !string.IsNullOrWhiteSpace(d.Summary)) // Sécurité : on ignore les docs sans résumé
    //         .Select(d => new { d.IdDocument, d.Summary })
    //         .ToList();
    //
    //     if (!documentListForAi.Any()) return new List<string>();
    //
    //     // On convertit cette liste en une chaîne JSON compacte pour le prompt
    //     var jsonDocuments = JsonSerializer.Serialize(documentListForAi);
    //
    //     // 2. Construction du System Prompt pour "cadrer" l'IA
    //     string systemPrompt = @"Tu es un expert en recherche sémantique.
    //     Je vais te donner une requête utilisateur en langage naturel et une liste JSON de documents avec leurs résumés.
    //     Ton but est d'identifier les documents dont le résumé correspond le mieux au sens de la requête.
    //     Tu dois retourner UNIQUEMENT un tableau JSON contenant les IdDocument correspondants.
    //     Si aucun document ne correspond, retourne un tableau vide [].
    //     Ne réponds JAMAIS avec du texte d'explication, UNIQUEMENT le JSON : [""id1"", ""id2""]";
    //
    //     // 3. Construction du User Prompt avec les données
    //     string userPrompt = $"Requête de l'utilisateur : {userQuery} \n\n Liste des documents : {jsonDocuments}";
    //
    //     // 4. Appel de l'API OpenAI
    //     try 
    //     {
    //         var messages = new ChatMessage[]
    //         {
    //             new SystemChatMessage(systemPrompt),
    //             new UserChatMessage(userPrompt)
    //         };
    //
    //         ChatCompletion completion = await _client.CompleteChatAsync(messages);
    //
    //         // 5. Extraction et nettoyage du JSON reçu
    //         string rawJson = completion.Content[0].Text.Trim();
    //
    //         // Parfois l'IA ajoute des blocs ```json ... ```, on les nettoie
    //         if (rawJson.StartsWith("```json")) rawJson = rawJson.Replace("```json", "").Replace("```", "").Trim();
    //
    //         // 6. Désérialisation pour récupérer les IDs
    //         return JsonSerializer.Deserialize<List<string>>(rawJson) ?? new List<string>();
    //     }
    //     catch (JsonException ex)
    //     {
    //         // Erreur de formatage de l'IA (rare avec un bon prompt)
    //         Console.WriteLine($"Erreur désérialisation OpenAI: {ex.Message}. Raw: {userPrompt}");
    //         return new List<string>();
    //     }
    //     catch (Exception ex)
    //     {
    //         // Autre erreur API
    //         Console.WriteLine($"Erreur API OpenAI: {ex.Message}");
    //         return new List<string>();
    //     }
    // }
}