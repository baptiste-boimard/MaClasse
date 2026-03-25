using System.Text;
using System.Text.Json;
using System.Net;
using System.Net.Http.Headers;
using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Lesson;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;


namespace MaClasse.Client.States;

public class LessonState
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly UserState _userState;
    private readonly SchedulerState _schedulerState;
    private readonly ISnackbar _snackbar;

    public LessonState(
        HttpClient httpClient,
        IConfiguration configuration,
        UserState userState,
        SchedulerState schedulerState,
        ISnackbar snackbar)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _userState = userState;
        _schedulerState = schedulerState;
        _snackbar = snackbar;
    }
    
    public event Action OnAppointmentSelected;
    public event Action OnChange;
    
    public Lesson Lesson { get; set; }
    public Lesson CopyLesson { get; set; } = new Lesson();
    public Appointment SelectedAppointment { get; set; }
    public bool IsReadOnly { get; set; } = false;
    public string? UserLessonDisplayed { get; set; } = null;

    
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
            IdSession = _userState.IdSession,
            UserLessonDisplayed = UserLessonDisplayed
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
        
        //* Il faut effacer les fichier sur cloudinary
        var deletionResponse = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/cloud/delete-files", newRequestLesson);

        if (deletionResponse.IsSuccessStatusCode)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/database/delete-lesson", newRequestLesson);

            if (response.IsSuccessStatusCode)
            {
                Lesson = new Lesson();
                SelectedAppointment = new Appointment();
            
                NotifyStateChanged();
            }
        }
    }

    public async void DeleteLessonAfterAppointmentDeletion(string idAppointment)
    {
        var newRequestLesson = new RequestLesson
        {
            IdSession = _userState.IdSession,
            IdAppointement = idAppointment,
            Lesson = Lesson
        };
        
        //* Il faut effacer les fichier sur cloudinary
        var deletionResponse = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/cloud/delete-files", newRequestLesson);

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
                    SelectedAppointment = new Appointment();
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
    
    public async Task<bool> UploadFileAsync(IBrowserFile file, IProgress<int>? progress = null)
    {
        if (Lesson.IdLesson == null)
        {
            var lessonSaved = await AddLesson(Lesson, SelectedAppointment);
            if (!lessonSaved)
            {
                return false;
            }
        }

        Stream? stream = null;
        try
        {
            stream = file.OpenReadStream(maxAllowedSize: 8 * 1024 * 1024);
        }
        catch (IOException ex)
        {
            if (ex.Message.Contains("exceeds the maximum"))
            {
                _snackbar.Add("Le fichier est trop volumineux (max 8 Mo).", Severity.Error);
            }

            return false;
        }

        using var content = new MultipartFormDataContent();
        var fileContent = new UploadProgressStreamContent(stream!, file.Size, progress);
        if (!string.IsNullOrWhiteSpace(file.ContentType))
        {
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
        }

        content.Add(fileContent, "file", file.Name);

        var request = new FileRequest
        {
            IdSession = _userState.IdSession,
        };
        var json = JsonSerializer.Serialize(request);
        content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "filerequest");

        var response = await _httpClient.PostAsync(
            $"{_configuration["Url:ApiGateway"]}/api/cloud/add-file", content);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var newDocument = await response.Content.ReadFromJsonAsync<Document>();
        if (newDocument is null)
        {
            return false;
        }

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
        progress?.Report(100);
        NotifyStateChanged();
        return true;
    }

    public async Task<bool> DeleteFileAsync(Document document)
    {
        var newRequestLesson = new RequestLesson
        {
            Lesson = Lesson,
            IdSession = _userState.IdSession,
            Document = document
        };
        
        var response =
            await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/cloud/delete-file", newRequestLesson);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        //* Confirmation du delete du fichier, BDD à mettre à jour
        var deleted = await DeleteDocumentInLessonAsync(document);
        NotifyStateChanged();
        return deleted;
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

    public async Task<bool> DeleteDocumentInLessonAsync(Document document)
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

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var deletedDocument = await response.Content.ReadFromJsonAsync<Document>();
        if (deletedDocument is null)
        {
            return false;
        }

        Lesson.Documents.RemoveAll(d => d.IdDocument == deletedDocument.IdDocument);
        NotifyStateChanged();
        return true;
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
    
    public void SetViewDashboard(string userId)
    {
        UserLessonDisplayed = userId;
        IsReadOnly = userId != _schedulerState.IdUser ? true : false;
    }
    
    public async Task<Document> GetDocument(string idUser, string idDocument)
    {
        var newDocument = new Document
        {
            IdDocument = idDocument,
        };
        
        var newFileRequestToDatabase = new FileRequestToDatabase
        {
            IdUser = idUser,
            Document = newDocument
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/get-document", newFileRequestToDatabase);

        if (response.IsSuccessStatusCode)
        {
            var document = await response.Content.ReadFromJsonAsync<Document>();

            return document;
        }

        return null;
    }
    
    public void ResetLessonState()
    {
        Lesson = new Lesson();
        CopyLesson = new Lesson();
        SelectedAppointment = new Appointment();
        UserLessonDisplayed = null;
        IsReadOnly = false;
        NotifyStateChanged();
    }
    
    public void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }

    private sealed class UploadProgressStreamContent : HttpContent
    {
        private readonly Stream _stream;
        private readonly long _size;
        private readonly IProgress<int>? _progress;
        private readonly int _bufferSize;

        public UploadProgressStreamContent(Stream stream, long size, IProgress<int>? progress, int bufferSize = 81920)
        {
            _stream = stream;
            _size = Math.Max(1, size);
            _progress = progress;
            _bufferSize = bufferSize;
        }

        protected override async Task SerializeToStreamAsync(Stream target, TransportContext? context)
        {
            var buffer = new byte[_bufferSize];
            long uploaded = 0;
            _progress?.Report(0);

            int read;
            while ((read = await _stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await target.WriteAsync(buffer, 0, read);
                uploaded += read;

                var percent = (int)Math.Round(uploaded * 100d / _size);
                _progress?.Report(Math.Clamp(percent, 0, 99));
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _size;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
