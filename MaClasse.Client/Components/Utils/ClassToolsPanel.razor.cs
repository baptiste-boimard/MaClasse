using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace MaClasse.Client.Components.Utils;

public partial class ClassToolsPanel
{
    [Parameter] public string? Class { get; set; }
    [Parameter] public bool SplitCards { get; set; }
    [Parameter] public bool SoundFirst { get; set; }
    [Parameter] public bool ShowToolsCard { get; set; } = true;
    [Parameter] public bool ShowSoundCard { get; set; } = true;
    [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private string PanelClass
    {
        get
        {
            var classes = new List<string> { "class-tools-panel" };

            if (!string.IsNullOrWhiteSpace(Class))
            {
                classes.Add(Class);
            }

            if (SplitCards)
            {
                classes.Add("class-tools-panel--split");
            }

            if (SoundFirst)
            {
                classes.Add("class-tools-panel--sound-first");
            }

            return string.Join(" ", classes);
        }
    }

    private DotNetObjectReference<ClassToolsPanel>? _dotNetRef;
    private string? _syncSubscriptionId;
    private CancellationTokenSource? _toolsLoopCts;
    private bool _isInteractiveReady;
    private bool _stateLoaded;
    private bool _isApplyingExternalState;

    private ToolMode _activeTool = ToolMode.Timer;
    private bool _isTimerRunning;
    private DateTime _timerEndUtc;
    private TimeSpan _timerRemaining = TimeSpan.FromMinutes(5);
    private bool _isStopwatchRunning;
    private long _stopwatchElapsedMs;
    private DateTime _stopwatchStartedUtc;
    private bool _isSoundMeterActive;
    private int _soundLevel;
    private double _soundDbFs = -60d;
    private string _soundMeterError = string.Empty;

    protected override void OnInitialized()
    {
        StartToolsLoop();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _isInteractiveReady = true;
        _dotNetRef = DotNetObjectReference.Create(this);

        try
        {
            await LoadPersistedStateAsync();
            _syncSubscriptionId = await JsRuntime.InvokeAsync<string>("classToolsSync.subscribe", _dotNetRef);
            await PersistStateAsync();

            if (ShowToolsCard && ShowSoundCard)
            {
                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireTabRedirectFromSelf",
                    "class-tools-card-entry",
                    "class-sound-card-entry");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireShiftTabRedirectFromSelf",
                    "class-sound-card-entry",
                    "class-tools-card-entry");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireTabRedirectFromSelf",
                    "class-tools-stopwatch",
                    "class-sound-card-entry");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireDocumentTabRedirect",
                    "class-tools-stopwatch-reset",
                    "class-sound-card-entry");
            }

            if (ShowSoundCard)
            {
                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireTabRedirectFromSelf",
                    "class-sound-card-entry",
                    "top-menu-entry");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireEnterSpaceRedirectFromSelf",
                    "class-sound-card-entry",
                    "class-sound-activate");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireDocumentShiftTabRedirect",
                    "class-sound-activate",
                    "class-sound-card-entry");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireTabSequenceByIds",
                    new[] { "class-sound-activate", "class-sound-desactivate" });

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireDocumentTabRedirect",
                    "class-sound-desactivate",
                    "top-menu-entry");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireKeyActivateToInnerButtonClick",
                    "class-sound-activate");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireKeyActivateToInnerButtonClick",
                    "class-sound-desactivate");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.keepInnerButtonTabIndexNegative",
                    "class-sound-activate");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.keepInnerButtonTabIndexNegative",
                    "class-sound-desactivate");
            }

            if (ShowToolsCard)
            {
                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireShiftTabRedirectFromSelf",
                    "class-tools-card-entry",
                    "dashboard-scheduler-card-entry");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireEnterSpaceRedirectFromSelf",
                    "class-tools-card-entry",
                    "class-tools-timer");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireTabRedirectFromSelf",
                    "class-tools-timer",
                    "class-tools-stopwatch");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireShiftTabRedirectFromSelf",
                    "class-tools-timer",
                    "class-tools-card-entry");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireTabButtonEnterToPanel",
                    "class-tools-timer",
                    "class-tools-timer-show");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireDocumentShiftTabRedirect",
                    "class-tools-timer-show",
                    "class-tools-timer");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireTabSequenceByIds",
                    new[]
                    {
                        "class-tools-timer-show",
                        "class-tools-timer-add-1",
                        "class-tools-timer-add-5",
                        "class-tools-timer-add-30",
                        "class-tools-timer-action-start",
                        "class-tools-timer-action-stop",
                        "class-tools-timer-action-reset"
                    });

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireDocumentTabRedirect",
                    "class-tools-timer-action-reset",
                    "class-tools-stopwatch");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireKeyActivateToInnerButtonClick",
                    "class-tools-timer-action-start");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireKeyActivateToInnerButtonClick",
                    "class-tools-timer-action-stop");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.keepInnerButtonTabIndexNegative",
                    "class-tools-timer-action-start");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.keepInnerButtonTabIndexNegative",
                    "class-tools-timer-action-stop");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireTabButtonEnterToPanel",
                    "class-tools-stopwatch",
                    "class-tools-stopwatch-show");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireDocumentShiftTabRedirect",
                    "class-tools-stopwatch-show",
                    "class-tools-timer");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireTabSequenceByIds",
                    new[]
                    {
                        "class-tools-stopwatch-show",
                        "class-tools-stopwatch-start",
                        "class-tools-stopwatch-stop",
                        "class-tools-stopwatch-reset"
                    });

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireKeyActivateToInnerButtonClick",
                    "class-tools-stopwatch-start");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.wireKeyActivateToInnerButtonClick",
                    "class-tools-stopwatch-stop");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.keepInnerButtonTabIndexNegative",
                    "class-tools-stopwatch-start");

                await JsRuntime.InvokeVoidAsync(
                    "focusHelpers.keepInnerButtonTabIndexNegative",
                    "class-tools-stopwatch-stop");
            }
        }
        catch
        {
            // Ignore JS bootstrap errors; tools remain usable in current tab.
        }
    }

    private void SelectTool(ToolMode tool)
    {
        _activeTool = tool;
        _ = PersistStateAsync();
    }

    private void AddMinutesToTimer(int minutes)
    {
        if (minutes <= 0)
        {
            return;
        }

        _timerRemaining += TimeSpan.FromMinutes(minutes);
        if (_isTimerRunning)
        {
            _timerEndUtc = _timerEndUtc.AddMinutes(minutes);
        }

        _ = PersistStateAsync();
    }

    private void StartTimer()
    {
        if (_timerRemaining <= TimeSpan.Zero || _isTimerRunning)
        {
            return;
        }

        _timerEndUtc = DateTime.UtcNow.Add(_timerRemaining);
        _isTimerRunning = true;
        _ = PersistStateAsync();
    }

    private void StopTimer()
    {
        _isTimerRunning = false;
        _ = PersistStateAsync();
    }

    private void ResetTimer()
    {
        _isTimerRunning = false;
        _timerRemaining = TimeSpan.Zero;
        _ = PersistStateAsync();
    }

    private void StartStopwatch()
    {
        if (_isStopwatchRunning)
        {
            return;
        }

        _stopwatchStartedUtc = DateTime.UtcNow;
        _isStopwatchRunning = true;
        _ = PersistStateAsync();
    }

    private void StopStopwatch()
    {
        if (!_isStopwatchRunning)
        {
            return;
        }

        _stopwatchElapsedMs = GetStopwatchElapsed().Ticks / TimeSpan.TicksPerMillisecond;
        _isStopwatchRunning = false;
        _ = PersistStateAsync();
    }

    private void ResetStopwatch()
    {
        _isStopwatchRunning = false;
        _stopwatchElapsedMs = 0;
        _ = PersistStateAsync();
    }

    private async Task StartSoundMeterAsync()
    {
        _soundMeterError = string.Empty;

        if (!_isInteractiveReady)
        {
            _soundMeterError = "Initialisation en cours, réessayez dans un instant.";
            return;
        }

        try
        {
            var started = await JsRuntime.InvokeAsync<bool>("soundMeter.start");
            _isSoundMeterActive = started;
            if (!started)
            {
                _soundMeterError = "Accès micro refusé ou indisponible.";
            }
        }
        catch
        {
            _isSoundMeterActive = false;
            _soundMeterError = "Impossible d'activer le micro.";
        }

        await PersistStateAsync();
    }

    private async Task StopSoundMeterAsync()
    {
        _isSoundMeterActive = false;
        _soundLevel = 0;
        _soundDbFs = -60d;
        _soundMeterError = string.Empty;

        if (!_isInteractiveReady)
        {
            return;
        }

        try
        {
            await JsRuntime.InvokeVoidAsync("soundMeter.stop");
        }
        catch
        {
            // Ignore JS stop errors to keep UI responsive.
        }

        await PersistStateAsync();
    }

    private string GetTimerDisplay()
    {
        var hours = (int)_timerRemaining.TotalHours;
        var minutes = _timerRemaining.Minutes;
        var seconds = _timerRemaining.Seconds;
        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    private string GetStopwatchDisplay()
    {
        var elapsed = GetStopwatchElapsed();
        return $"{(int)elapsed.TotalHours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";
    }

    private TimeSpan GetStopwatchElapsed()
    {
        var elapsed = TimeSpan.FromMilliseconds(_stopwatchElapsedMs);
        if (_isStopwatchRunning)
        {
            elapsed += DateTime.UtcNow - _stopwatchStartedUtc;
        }

        return elapsed < TimeSpan.Zero ? TimeSpan.Zero : elapsed;
    }

    private Color GetSoundColor()
    {
        var displayDb = GetSoundDisplayDb();
        return displayDb switch
        {
            <= 15 => Color.Success,
            <= 30 => Color.Warning,
            <= 42 => Color.Secondary,
            _ => Color.Error
        };
    }

    private string GetSoundDbLabel()
    {
        return $"{GetSoundDisplayDb():0.0} dB";
    }

    private double GetSoundDisplayDb()
    {
        return Math.Clamp(_soundDbFs + 60d, 0d, 60d);
    }

    private void StartToolsLoop()
    {
        _toolsLoopCts = new CancellationTokenSource();
        _ = RunToolsLoopAsync(_toolsLoopCts.Token);
    }

    private async Task RunToolsLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var shouldRender = false;
            var shouldPersist = false;

            if (_isTimerRunning)
            {
                var remaining = _timerEndUtc - DateTime.UtcNow;
                if (remaining <= TimeSpan.Zero)
                {
                    _timerRemaining = TimeSpan.Zero;
                    _isTimerRunning = false;
                    shouldPersist = true;
                }
                else
                {
                    _timerRemaining = remaining;
                }

                shouldRender = true;
            }

            if (_isStopwatchRunning)
            {
                shouldRender = true;
            }

            if (_isSoundMeterActive && _isInteractiveReady)
            {
                try
                {
                    var dbFs = await JsRuntime.InvokeAsync<double>("soundMeter.getDbfs");
                    dbFs = Math.Clamp(dbFs, -60d, 0d);
                    var normalizedLevel = (int)Math.Round(((dbFs + 60d) / 60d) * 100d);
                    normalizedLevel = Math.Clamp(normalizedLevel, 0, 100);

                    if (_soundLevel != normalizedLevel || Math.Abs(_soundDbFs - dbFs) > 0.05d)
                    {
                        _soundDbFs = dbFs;
                        _soundLevel = normalizedLevel;
                        shouldRender = true;
                    }
                }
                catch
                {
                    _isSoundMeterActive = false;
                    _soundLevel = 0;
                    _soundDbFs = -60d;
                    _soundMeterError = "Mesure sonore interrompue.";
                    shouldPersist = true;
                    shouldRender = true;
                }
            }

            if (shouldPersist)
            {
                await PersistStateAsync();
            }

            if (shouldRender)
            {
                await InvokeAsync(StateHasChanged);
            }

            try
            {
                await Task.Delay(250, token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private async Task PersistStateAsync()
    {
        if (!_isInteractiveReady || !_stateLoaded || _isApplyingExternalState)
        {
            return;
        }

        var payload = new SyncState
        {
            ActiveTool = _activeTool == ToolMode.Stopwatch ? "stopwatch" : "timer",
            IsTimerRunning = _isTimerRunning,
            TimerEndUtc = _isTimerRunning ? _timerEndUtc : null,
            TimerRemainingMs = (long)Math.Max(0, _timerRemaining.TotalMilliseconds),
            IsStopwatchRunning = _isStopwatchRunning,
            StopwatchElapsedMs = _stopwatchElapsedMs,
            StopwatchStartedUtc = _isStopwatchRunning ? _stopwatchStartedUtc : null,
            IsSoundMeterActive = _isSoundMeterActive
        };

        try
        {
            await JsRuntime.InvokeVoidAsync("classToolsSync.setState", payload);
        }
        catch
        {
            // Ignore persistence errors to keep tools responsive.
        }
    }

    private async Task LoadPersistedStateAsync()
    {
        try
        {
            var state = await JsRuntime.InvokeAsync<SyncState?>("classToolsSync.getState");
            if (state is not null)
            {
                ApplyState(state);
                await ApplySoundMeterActivationAsync();
            }
        }
        finally
        {
            _stateLoaded = true;
            await InvokeAsync(StateHasChanged);
        }
    }

    [JSInvokable]
    public async Task OnExternalStateChanged()
    {
        if (!_isInteractiveReady)
        {
            return;
        }

        try
        {
            var state = await JsRuntime.InvokeAsync<SyncState?>("classToolsSync.getState");
            if (state is null)
            {
                return;
            }

            _isApplyingExternalState = true;
            ApplyState(state);
            await ApplySoundMeterActivationAsync();
            await InvokeAsync(StateHasChanged);
        }
        finally
        {
            _isApplyingExternalState = false;
        }
    }

    private void ApplyState(SyncState state)
    {
        _activeTool = string.Equals(state.ActiveTool, "stopwatch", StringComparison.OrdinalIgnoreCase)
            ? ToolMode.Stopwatch
            : ToolMode.Timer;

        _isTimerRunning = state.IsTimerRunning;
        _timerEndUtc = state.TimerEndUtc ?? DateTime.UtcNow;
        _timerRemaining = TimeSpan.FromMilliseconds(Math.Max(0, state.TimerRemainingMs));
        if (_isTimerRunning)
        {
            var remaining = _timerEndUtc - DateTime.UtcNow;
            _timerRemaining = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
            if (_timerRemaining == TimeSpan.Zero)
            {
                _isTimerRunning = false;
            }
        }

        _isStopwatchRunning = state.IsStopwatchRunning;
        _stopwatchElapsedMs = Math.Max(0, state.StopwatchElapsedMs);
        _stopwatchStartedUtc = state.StopwatchStartedUtc ?? DateTime.UtcNow;
        _isSoundMeterActive = state.IsSoundMeterActive;
    }

    private async Task ApplySoundMeterActivationAsync()
    {
        if (!_isInteractiveReady)
        {
            return;
        }

        if (_isSoundMeterActive)
        {
            _soundMeterError = string.Empty;

            try
            {
                var started = await JsRuntime.InvokeAsync<bool>("soundMeter.start");
                if (!started)
                {
                    _isSoundMeterActive = false;
                    _soundLevel = 0;
                    _soundDbFs = -60d;
                    _soundMeterError = "Accès micro refusé ou indisponible.";
                    await PersistStateAsync();
                }
            }
            catch
            {
                _isSoundMeterActive = false;
                _soundLevel = 0;
                _soundDbFs = -60d;
                _soundMeterError = "Impossible d'activer le micro.";
                await PersistStateAsync();
            }

            return;
        }

        try
        {
            await JsRuntime.InvokeVoidAsync("soundMeter.stop");
        }
        catch
        {
            // Ignore JS stop errors while reconciling state.
        }

        _soundLevel = 0;
        _soundDbFs = -60d;
    }

    public async ValueTask DisposeAsync()
    {
        _toolsLoopCts?.Cancel();
        _toolsLoopCts?.Dispose();
        _toolsLoopCts = null;

        if (_syncSubscriptionId is not null)
        {
            try
            {
                await JsRuntime.InvokeVoidAsync("classToolsSync.unsubscribe", _syncSubscriptionId);
            }
            catch
            {
                // Ignore cleanup JS errors.
            }
        }

        _dotNetRef?.Dispose();
        _dotNetRef = null;
    }

    private sealed class SyncState
    {
        public string ActiveTool { get; set; } = "timer";
        public bool IsTimerRunning { get; set; }
        public DateTime? TimerEndUtc { get; set; }
        public long TimerRemainingMs { get; set; }
        public bool IsStopwatchRunning { get; set; }
        public long StopwatchElapsedMs { get; set; }
        public DateTime? StopwatchStartedUtc { get; set; }
        public bool IsSoundMeterActive { get; set; }
    }

    private enum ToolMode
    {
        Timer,
        Stopwatch
    }
}
