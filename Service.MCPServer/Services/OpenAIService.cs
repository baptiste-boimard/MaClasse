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

    public async Task<string> GenerateSummaryAsync(string prompt)
    {
        ChatCompletion completion = await _client.CompleteChatAsync(prompt);
        return completion.Content[0].Text;
    }

    public Task<string> GetDocFromSummaryAsync(string prompt)
    {
        throw new NotImplementedException();
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
}