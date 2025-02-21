using HollowTime.Data;
using Microsoft.AspNetCore.Components;

namespace HollowTime.Components.Scramble
{
    public partial class CubeScramble : ComponentBase
    {
        public event Action<ScrambleData> OnScrambleChanged = delegate { };
        
        ScrambleData currentScramble = new ScrambleData();
        
        public async void RefreshScramble() 
        {
            if (currentScramble.SubsetType != string.Empty) 
            {
                currentScramble.Scramble = await JS.InvokeAsync<string>("scrambleGenerator.getDefaultScramble", [currentScramble.SubsetType]);
            }
            else 
            {
                currentScramble.Scramble = await JS.InvokeAsync<string>("scrambleGenerator.getDefaultScramble", [currentScramble.EventType]);
            }
            OnScrambleChanged?.Invoke(currentScramble);
            StateHasChanged();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                RefreshScramble();
            }
        }

        void onEventTypeSelected(string eventType, string eventName)
        {
            currentScramble.EventName = eventName;
            currentScramble.EventType = eventType;
            currentScramble.SubsetType = String.Empty;

            RefreshScramble();
        }

        void onSubsetSelected(string subset)
        {
            currentScramble.SubsetType = subset;
            RefreshScramble();
        }
    }
}