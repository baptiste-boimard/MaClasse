using System.Net;
using System.Text;
using System.Text.Json;
using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Lesson;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Components.Forms;

namespace MaClasse.Client.States;

public class LessonState
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly UserState _userState;

    public LessonState(
        HttpClient httpClient,
        IConfiguration configuration,
        UserState userState)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _userState = userState;
    }
    
    public event Action OnAppointmentSelected;
    public event Action OnChange;
    
    public Lesson Lesson { get; set; }
    public Lesson CopyLesson { get; set; } = new Lesson();
    public Appointment SelectedAppointment { get; set; }

    public async void SetLessonSelected(Appointment appointment)
    {
        //* Je récupére l'appointment selectionnée
        SelectedAppointment = appointment;
        
        //* Je vais voir en base de données si j'ai une Lesson liée
        var lesson = await GetLessonFromAppointment();

        Lesson = lesson;

        Lesson.IdAppointment = SelectedAppointment.Id;
        
        NotifyStateChanged();
    }

    public async Task<Lesson> GetLessonFromAppointment()
    {
        var lessonRequest = new LessonRequest
        {
            Appointment = SelectedAppointment,
            IdSession = _userState.IdSession
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/get-lesson", lessonRequest);

        if (response.IsSuccessStatusCode)
        {
            var lesson = await response.Content.ReadFromJsonAsync<Lesson>();
            return lesson;
        }
        
        return new Lesson();
    }

    public async Task<bool> AddLesson(Lesson lesson, Appointment appointment)
    {
        lesson.IdAppointment = appointment.Id;

        var newRequestLesson = new RequestLesson
        {
            Lesson = lesson,
            IdSession = _userState.IdSession
        };
        
        var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/database/add-lesson", newRequestLesson);

        if (response.IsSuccessStatusCode)
        {
            Lesson = await response.Content.ReadFromJsonAsync<Lesson>();
            NotifyStateChanged();
            return true;
        }

        return false;
    }

    public async void DeleteLesson(Lesson lesson)
    {
        var newRequestLesson = new RequestLesson
        {
            Lesson = lesson,
            IdSession = _userState.IdSession
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/delete-lesson", newRequestLesson);

        if (response.IsSuccessStatusCode)
        {
            Lesson = new Lesson();
            NotifyStateChanged();
        }
    }

    public async void DeleteLessonAfterAppointmentDeletion(string idAppointment)
    {
        var newRequestLesson = new RequestLesson
        {
            IdSession = _userState.IdSession,
            IdAppointement = idAppointment
        };
        
        //* Il faut effacer les fichier sur cloudinary
        var deletionResponse = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/cloud/delete-files", Lesson.Documents);

        if (deletionResponse.IsSuccessStatusCode)
        {
            //* Ensuite on supprime la lesson de la base de données
            var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/database/delete-lesson", newRequestLesson);

            if (response.IsSuccessStatusCode)
            {
                //* Si la lesson affiché correspond à l'appointment supprimé
                if (Lesson.IdAppointment == idAppointment)
                {
                    Lesson = new Lesson();
                }
                
                NotifyStateChanged();
            }
        }
    }

    public async void UpdateSelectedAppointment(List<Appointment> appointments)
    {
        var updated = appointments.FirstOrDefault(a => a.Id == SelectedAppointment.Id);

        if (updated != null)
        {
            SelectedAppointment = updated;
            NotifyStateChanged();
        }
    }
    
    public async void SetCopyLesson(Lesson lesson)
    {
        lesson.IdLesson = null;
        CopyLesson = lesson;
    }

    public Lesson GetCopyLesson()
    {
        foreach (var prop in typeof(Lesson).GetProperties())
        {
            if (prop.Name == "IdLesson") continue;

            var value = prop.GetValue(CopyLesson);
            prop.SetValue(Lesson, value);
        }
        
        CopyLesson = new Lesson();
        NotifyStateChanged();
        return Lesson;
    }
    
    public async void UploadFile(IBrowserFile file)
    {
        //* Si demande d'ajout de fichiers mais que IdLesson n'est pas encore définit
        if (Lesson.IdLesson == null)
        {
            //* Il faut sauvegarder la Lesson pour avoir son id
            await AddLesson(Lesson, SelectedAppointment);
        }
    
        var content = new MultipartFormDataContent();
    
        //* Ajout du fichier 
        var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
        content.Add(new StreamContent(stream), "file", file.Name);
    
        //* Ajouter les métadonnées (JSON sous forme de StringContent)
        var request = new FileRequest
        {
            IdSession = _userState.IdSession,
        };
        var json = JsonSerializer.Serialize(request);
        content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "filerequest");
    
        var response =
            await _httpClient.PostAsync(
                $"{_configuration["Url:ApiGateway"]}/api/cloud/add-file", content);

        if (response.IsSuccessStatusCode)
        {
            var newDocument = await response.Content.ReadFromJsonAsync<Document>();
            
            // 🔍 LOG COMPLET de newDocument
            var logjson = JsonSerializer.Serialize(newDocument, new JsonSerializerOptions
            {
              WriteIndented = true, // joli format
              IgnoreNullValues = false // montre les nulls
            });
            Console.WriteLine("🔍 Contenu de newDocument :\n" + logjson);
            
            //* Mise a jour de la Lesson avec le nouveau documents
            Lesson.Documents.Add(newDocument);
            
            // 🔍 Log de toute la liste des documents
            var logList = JsonSerializer.Serialize(Lesson.Documents, new JsonSerializerOptions
            {
              WriteIndented = true,
              IgnoreNullValues = false
            });
            Console.WriteLine("📚 Liste complète des documents dans Lesson :\n" + logList);

            await AddLesson(Lesson, SelectedAppointment);
            
            NotifyStateChanged();
        }
    }

    public async void DeleteFile(Document document)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/cloud/delete-file", document);

        if (response.IsSuccessStatusCode)
        {
            //* Confirmation du delete du fichier, BDD à mettre à jour
            DeleteDocumentInLesson(document);
        }
        
        NotifyStateChanged();
    }

    public async void RenameFile(Document document)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/cloud/rename-file", document);

        if (response.IsSuccessStatusCode)
        {
            var updatedDocument = await response.Content.ReadFromJsonAsync<Document>();
            //* Confirmation de l'update du fichier, BDD à mettre à jour
            UploadDocumentInLesson(updatedDocument);
        }
    }

    public async void DeleteDocumentInLesson(Document document)
    {
        var newRequestLesson = new RequestLesson
        {
            Lesson = Lesson,
            IdSession = _userState.IdSession,
            Document = document
        };
        
        var response =
            await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/database/delete-document-in-lesson", newRequestLesson);

        if (response.IsSuccessStatusCode)
        {
            var deletedDocument = await response.Content.ReadFromJsonAsync<Document>();

            Lesson.Documents.RemoveAll(d => d.IdDocument == deletedDocument.IdDocument);
            
            NotifyStateChanged();
        }
    }

    public async void UploadDocumentInLesson(Document document)
    {
        var newRequestLesson = new RequestLesson
        {
            Lesson = Lesson,
            IdSession = _userState.IdSession,
            Document = document
        };
        
        var response =
            await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/database/upload-document-in-lesson", newRequestLesson);

        if (response.IsSuccessStatusCode)
        {
            var updatedDocument = await response.Content.ReadFromJsonAsync<Document>();

            var index = Lesson.Documents.FindIndex(d => d.IdDocument == updatedDocument.IdDocument);

            if (index != -1)
            {
                Lesson.Documents[index] = updatedDocument;
                NotifyStateChanged();
            }
        }
    }
    
    public void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}