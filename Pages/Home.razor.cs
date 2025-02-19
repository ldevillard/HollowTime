using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace HollowTime.Pages
{
    public partial class Home : ComponentBase
    {
        HollowTime.Components.Timer? timer;
        HollowTime.Components.TimeStats? currentStats;

        string currentScramble = String.Empty;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender && timer is not null)
            {
                refreshScramble();
                timer.OnTimerEnded += onTimerEnded;
            }
        }

        void onKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == " ")
                timer?.TryLoadTimer();
        }

        void onKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == " ")
                timer?.TryStartTimer();
        }

        void onTimerEnded(TimeSpan elapsedTime)
        {
            refreshScramble();
            currentStats?.RecordTime(elapsedTime);
        }

        async void refreshScramble() 
        {
            currentScramble = await JS.InvokeAsync<string>("scrambleGenerator.getDefaultScramble");
            StateHasChanged();
        }

        public void Dispose()
        {
            if (timer is not null) 
            {
                timer.OnTimerEnded -= onTimerEnded;
            }
        }
    }
}
