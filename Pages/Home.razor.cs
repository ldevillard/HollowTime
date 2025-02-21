using HollowTime.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace HollowTime.Pages
{
    public partial class Home : ComponentBase
    {
        Components.Timer timer = new();
        Components.TimeStats currentStats = new();
        Components.Scramble.CubeScramble scramble = new();
        Components.CubeVisualizer visualizer = new();
        
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                timer.OnTimerEnded += onTimerEnded;
                scramble.OnScrambleChanged += onScrambleChanged;
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
            scramble?.RefreshScramble();
            currentStats?.RecordTime(elapsedTime);
        }

        void onScrambleChanged(ScrambleData scrambleData)
        {
            visualizer?.RefreshVisualizer(scrambleData);
        }

        public void Dispose()
        {
            timer.OnTimerEnded -= onTimerEnded;
            scramble.OnScrambleChanged -= onScrambleChanged;
        }
    }
}
