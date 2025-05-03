using System.Text.Json;
using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Api;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.Extensions.Azure;

public class HolidaysService
{
    private readonly HttpClient _httpClient;

    public HolidaysService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    

    public async Task<List<Appointment>> GetZoneBVacationsAsync(UserProfile user)
    {
        
        //* Prise des infos pour 2025-2026
        string zone = user.Zone[user.Zone.Length - 1].ToString();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var apiUrl =
            $"https://data.education.gouv.fr/api/explore/v2.1/catalog/datasets/fr-en-calendrier-scolaire/records?select=description%2C%20start_date%2C%20end_date%2C%20zones%2C%20annee_scolaire&where=zones%3A%22Zone%20{zone}%22&group_by=description%2C%20start_date%2C%20end_date%2C%20zones%2C%20annee_scolaire&order_by=start_date&limit=20&refine=annee_scolaire%3A%222025-2026%22&refine=annee_scolaire%3A%222024-2025%22";
        
        using var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        
        var response = await _httpClient.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<ApiResultResponse>(options);
        
        //* Il faut éliminer un doublon spécifique de la liste
        var distinctVacations = result.Results
            .Where(r => r.EndDate.Date != new DateTime(2025, 8, 29))
            .ToList();     
        
        //* Une fois la liste des vacances obtenue il faut la mapper sur nos appointments
        var appointments = new List<Appointment>();

        foreach (var appointment in distinctVacations )
        {
            var appt = new Appointment();
            
            if (appointment.Description.Contains("Début des Vacances d'Été"))
            {
                //* Ajout de la date de fin non indiquée dans l'api
                string dateReprise2026 = "2026-08-31T22:00:00+00:00";

                appt = new Appointment
                {
                    Id = Guid.NewGuid().ToString(),
                    Start = appointment.StartDate,
                    End = DateTime.Parse(dateReprise2026),
                    Text = "Vacances d'Été",
                    Color = "#7cd9fd",
                    Recurring = false,
                    IdRecurring = String.Empty
                };
            }
            else
            {
                appt = new Appointment
                {
                    Id = Guid.NewGuid().ToString(),
                    Start = appointment.StartDate,
                    End = appointment.EndDate,
                    Text = appointment.Description,
                    Color = "#7cd9fd",
                    Recurring = false,
                    IdRecurring = String.Empty
                };
                
            }
            appointments.Add(appt);
        }
        return appointments;
    }
}