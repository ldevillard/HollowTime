using Microsoft.AspNetCore.Components;
using System.Timers;

namespace HollowTime.Components
{
    public partial class Timer : ComponentBase
    {
        #region Public Events
        
        public event Action<TimeSpan> OnTimerEnded = delegate { };
        
        #endregion

        #region Private Enum
        
        enum TimerState
        {
            Stopped,
            Loading,
            Ready,
            Running,
        };
        
        #endregion
        
        #region Private Members

        TimerState currentState = TimerState.Stopped;
        MudBlazor.Color currentColor = MudBlazor.Color.Default;

        System.Timers.Timer timer = new();
        TimeSpan elapsedTime;
        DateTime startTime;

        System.Timers.Timer loadingTimer = new();

        #endregion
        
        #region Public Methods
        
        public void TryLoadTimer()
        {
            switch (currentState)
            {
                case TimerState.Stopped:
                    loadTimer();
                    break;
                case TimerState.Running:
                    stopTimer();
                    break;
                default:
                    break;
            }
        }

        public void TryStartTimer()
        {
            if (currentState == TimerState.Ready)
            {
                startTimer();
            }
            else
            {
                currentState = TimerState.Stopped;
                currentColor = MudBlazor.Color.Default;

                loadingTimer.Stop();
                loadingTimer.Dispose();
                StateHasChanged();
            }
        }
        
        #endregion
        
        #region Private Methods

        void loadTimer()
        {
            if (currentState == TimerState.Loading || currentState == TimerState.Ready)
                return;

            currentState = TimerState.Loading;
            currentColor = MudBlazor.Color.Error;

            startTime = DateTime.Now;
            loadingTimer = new System.Timers.Timer(400);
            loadingTimer.Elapsed += updateLoadingTimer;
            loadingTimer.Start();
            StateHasChanged();
        }

        void updateLoadingTimer(object? sender, ElapsedEventArgs e)
        {
            currentState = TimerState.Ready;
            currentColor = MudBlazor.Color.Success;

            loadingTimer.Stop();
            loadingTimer.Dispose();
            StateHasChanged();
        }

        void startTimer()
        {
            currentState = TimerState.Running;
            currentColor = MudBlazor.Color.Default;

            startTime = DateTime.Now;
            timer = new System.Timers.Timer(10);
            timer.Elapsed += updateTime;

            timer.Start();
            StateHasChanged();
        }

        void stopTimer()
        {
            currentState = TimerState.Stopped;

            OnTimerEnded?.Invoke(elapsedTime);

            timer.Stop();
            timer.Dispose();
        }

        void updateTime(object? sender, ElapsedEventArgs e)
        {
            elapsedTime = DateTime.Now - startTime;
            InvokeAsync(StateHasChanged);
        }
        
        #endregion
    }
}