using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace HollowTime.Pages
{
    public partial class Home : ComponentBase
    {
        HollowTime.Components.Timer? timer;
        HollowTime.Components.TimeStats? currentStats;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender && timer is not null)
            {
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
            currentStats?.RecordTime(elapsedTime);
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
