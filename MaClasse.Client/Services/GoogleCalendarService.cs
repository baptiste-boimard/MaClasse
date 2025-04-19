using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using MaClasse.Client.States;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

public class GoogleCalendarService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserState _userState;

    public GoogleCalendarService(
        IHttpContextAccessor httpContextAccessor,
        UserState userState)
    {
        _httpContextAccessor = httpContextAccessor;
        _userState = userState;
    }

    public async Task<IList<Google.Apis.Calendar.v3.Data.Event>> GetUpcomingEventsAsync()
    {
        var token = _userState.AccessToken;

        var credential = GoogleCredential.FromAccessToken(token);

        var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "BlazorCalendarApp",
        });

        var request = service.Events.List("primary");
        request.TimeMin = DateTime.Now;
        request.MaxResults = 10;
        request.SingleEvents = true;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        var events = await request.ExecuteAsync();
        return events.Items;
    }
}