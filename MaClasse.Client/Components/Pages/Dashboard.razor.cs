using MaClasse.Client.States;
using MaClasse.Client.Components.DashboardContent.Files;
using MaClasse.Client.Components.DashboardContent.Lesson;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace MaClasse.Client.Components.Pages;

public partial class Dashboard : ComponentBase, IDisposable
{
    private readonly UserState _userState;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly SchedulerState _schedulerState;
    private readonly LessonState _lessonState;

    public Dashboard(
        UserState userState,
        AuthenticationStateProvider authenticationStateProvider,
        SchedulerState schedulerState,
        LessonState lessonState)
    {
        _userState = userState;
        _authenticationStateProvider = authenticationStateProvider;
        _schedulerState = schedulerState;
        _lessonState = lessonState;
    }

    private UserState? userInformation;
    private Appointment? _nextCourseAppointment;
    private bool _hasCurrentCourse;
    private string _nextCoursePreview = "Aucun cours à venir.";
    private int _weeklyLessonCount;
    private string _weeklyHoursLabel = "0h00";
    private System.Timers.Timer? _courseRefreshTimer;
    private ElementReference _nextCoursePreviewRef;
    private ElementReference _nextCourseTitleRef;
    private ElementReference _nextCourseTimeRef;
    private ElementReference _statsLessonsRef;
    private ElementReference _statsHoursRef;
    private FileExplorer? _dashboardFileExplorerRef;
    private LessonView? _lessonViewRef;

    protected override async Task OnInitializedAsync()
    {
        _userState.OnChange += HandleUserStateChanged;
        _schedulerState.OnChange += HandleSchedulerStateChanged;

        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is { IsAuthenticated: true })
        {
            userInformation = _userState.GetUser();
        }

        RefreshDashboardMetrics();
        StartCourseRefreshTimer();
    }

    private void HandleUserStateChanged()
    {
        userInformation = _userState.GetUser();
        InvokeAsync(StateHasChanged);
    }

    private void HandleSchedulerStateChanged()
    {
        RefreshDashboardMetrics();
        InvokeAsync(StateHasChanged);
    }

    private void StartCourseRefreshTimer()
    {
        _courseRefreshTimer = new System.Timers.Timer(60000);
        _courseRefreshTimer.Elapsed += (_, _) =>
        {
            RefreshDashboardMetrics();
            InvokeAsync(StateHasChanged);
        };
        _courseRefreshTimer.AutoReset = true;
        _courseRefreshTimer.Start();
    }

    private void RefreshNextCourse()
    {
        var now = DateTime.Now;

        var normalizedAppointments = (_schedulerState.Appointments ?? new List<Appointment>())
            .Where(a => a != null && a.End > a.Start)
            .Select(a => new
            {
                Source = a,
                StartLocal = a.Start.ToLocalTime(),
                EndLocal = a.End.ToLocalTime()
            })
            .OrderBy(a => a.StartLocal)
            .ToList();

        var currentCourse = normalizedAppointments
            .FirstOrDefault(a => a.StartLocal <= now && now < a.EndLocal);

        var nextCourse = normalizedAppointments.FirstOrDefault(a => a.StartLocal > now);
        var selectedCourse = currentCourse ?? nextCourse;

        _hasCurrentCourse = currentCourse is not null;

        if (selectedCourse is null)
        {
            _nextCourseAppointment = null;
            _nextCoursePreview = "Aucun cours à venir";
            return;
        }

        _nextCourseAppointment = selectedCourse.Source;

        if (_hasCurrentCourse)
        {
            var remaining = selectedCourse.EndLocal - now;
            _nextCoursePreview = remaining.TotalMinutes >= 60
                ? $"Fini dans {(int)remaining.TotalHours}h{remaining.Minutes:00}"
                : $"Fini dans {Math.Max(1, (int)Math.Round(remaining.TotalMinutes))} min";
            return;
        }

        var delay = selectedCourse.StartLocal - now;
        if (delay.TotalMinutes < 60)
        {
        }            _nextCoursePreview = $"Démarre dans {Math.Max(1, (int)Math.Round(delay.TotalMinutes))} min";

        else
        {
            _nextCoursePreview = $"Prévu le {selectedCourse.StartLocal:dddd dd MMM}";
        }
    }

    private void RefreshWeeklyStats()
    {
        var referenceDate = _schedulerState.CurrentDisplayedDate;
        var daysSinceMonday = ((int)referenceDate.DayOfWeek + 6) % 7;
        var weekStart = referenceDate.Date.AddDays(-daysSinceMonday);
        var weekEnd = weekStart.AddDays(7);

        var normalizedAppointments = (_schedulerState.Appointments ?? new List<Appointment>())
            .Where(a => a != null && a.End > a.Start)
            .Where(a => !IsVacationAppointment(a))
            .Select(a => new
            {
                StartLocal = a.Start.ToLocalTime(),
                EndLocal = a.End.ToLocalTime()
            })
            .Where(a => a.EndLocal > weekStart && a.StartLocal < weekEnd)
            .ToList();

        _weeklyLessonCount = normalizedAppointments.Count;

        var totalMinutes = normalizedAppointments
            .Sum(a =>
            {
                var effectiveStart = a.StartLocal < weekStart ? weekStart : a.StartLocal;
                var effectiveEnd = a.EndLocal > weekEnd ? weekEnd : a.EndLocal;
                var minutes = (effectiveEnd - effectiveStart).TotalMinutes;
                return minutes > 0 ? (int)Math.Round(minutes) : 0;
            });

        var hours = totalMinutes / 60;
        var minutesRemainder = totalMinutes % 60;
        _weeklyHoursLabel = $"{hours}h{minutesRemainder:00}";
    }

    private static bool IsVacationAppointment(Appointment appointment)
    {
        var title = appointment.Text ?? string.Empty;
        return title.Contains("Vacance", StringComparison.OrdinalIgnoreCase);
    }

    private void RefreshDashboardMetrics()
    {
        RefreshNextCourse();
        RefreshWeeklyStats();
    }

    private void OpenNextCourseInLessonPanel()
    {
        if (_nextCourseAppointment is null)
        {
            return;
        }

        _lessonState.SetLessonSelected(_nextCourseAppointment);
    }

    private string GetButtonAriaLabel()
    {
        if (_nextCourseAppointment is null)
            return "Afficher le cours";

        var heading = _hasCurrentCourse ? "Cours démarré" : "Prochain cours";
        var courseTitle = _nextCourseAppointment.Text ?? "Cours sans titre";
        var start = _nextCourseAppointment.Start.ToLocalTime().ToString("HH:mm");
        var end = _nextCourseAppointment.End.ToLocalTime().ToString("HH:mm");
        return $"Afficher le cours : {heading}, {_nextCoursePreview}, {courseTitle}, {start} à {end}";
    }

    private string GetNextCourseCardStyle()
    {
        var rawColor = _nextCourseAppointment?.Color;
        var (r, g, b) = TryParseColor(rawColor ?? string.Empty, out var parsed) ? parsed : (126, 111, 216);

        var light1 = MixWithWhite(r, g, b, 0.84);
        var light2 = MixWithWhite(r, g, b, 0.92);
        var border = MixWithWhite(r, g, b, 0.56);

        return $"background: radial-gradient(circle at 9% 10%, rgba({r}, {g}, {b}, 0.24), transparent 30%), linear-gradient(165deg, rgb({light1.r}, {light1.g}, {light1.b}) 0%, rgb({light2.r}, {light2.g}, {light2.b}) 100%); border-color: rgb({border.r}, {border.g}, {border.b});";
    }

    private static (int r, int g, int b) MixWithWhite(int r, int g, int b, double ratio)
    {
        ratio = Math.Clamp(ratio, 0.0, 1.0);
        var nr = (int)Math.Round(r + (255 - r) * ratio);
        var ng = (int)Math.Round(g + (255 - g) * ratio);
        var nb = (int)Math.Round(b + (255 - b) * ratio);
        return (Math.Clamp(nr, 0, 255), Math.Clamp(ng, 0, 255), Math.Clamp(nb, 0, 255));
    }

    private static bool TryParseColor(string rawColor, out (int r, int g, int b) color)
    {
        color = (0, 0, 0);

        if (string.IsNullOrWhiteSpace(rawColor))
        {
            return false;
        }

        var value = rawColor.Trim();

        if (value.StartsWith("#"))
        {
            if (value.Length == 7 &&
                int.TryParse(value.AsSpan(1, 2), System.Globalization.NumberStyles.HexNumber, null, out var r) &&
                int.TryParse(value.AsSpan(3, 2), System.Globalization.NumberStyles.HexNumber, null, out var g) &&
                int.TryParse(value.AsSpan(5, 2), System.Globalization.NumberStyles.HexNumber, null, out var b))
            {
                color = (r, g, b);
                return true;
            }

            if (value.Length == 4 &&
                int.TryParse($"{value[1]}{value[1]}", System.Globalization.NumberStyles.HexNumber, null, out var sr) &&
                int.TryParse($"{value[2]}{value[2]}", System.Globalization.NumberStyles.HexNumber, null, out var sg) &&
                int.TryParse($"{value[3]}{value[3]}", System.Globalization.NumberStyles.HexNumber, null, out var sb))
            {
                color = (sr, sg, sb);
                return true;
            }
        }

        if (value.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase) && value.EndsWith(")"))
        {
            var inner = value[4..^1];
            var parts = inner.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3 &&
                int.TryParse(parts[0], out var rr) &&
                int.TryParse(parts[1], out var gg) &&
                int.TryParse(parts[2], out var bb))
            {
                color = (Math.Clamp(rr, 0, 255), Math.Clamp(gg, 0, 255), Math.Clamp(bb, 0, 255));
                return true;
            }
        }

        return false;
    }

    public void Dispose()
    {
        _userState.OnChange -= HandleUserStateChanged;
        _schedulerState.OnChange -= HandleSchedulerStateChanged;

        if (_courseRefreshTimer is not null)
        {
            _courseRefreshTimer.Stop();
            _courseRefreshTimer.Dispose();
            _courseRefreshTimer = null;
        }
    }
}
