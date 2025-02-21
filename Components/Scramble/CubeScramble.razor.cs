using HollowTime.Data;
using Microsoft.AspNetCore.Components;

namespace HollowTime.Components.Scramble
{
    public partial class CubeScramble : ComponentBase
    {
        #region Public Events
        
        public event Action<ScrambleData> OnScrambleChanged = delegate { };
        
        #endregion
        
        #region Private Members
        
        ScrambleData currentScramble = new ScrambleData();
        
        #endregion
        
        #region Public Methods
        
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
        
        #endregion
        
        #region Protected Methods

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                RefreshScramble();
            }
        }
        
        #endregion
        
        #region Private Methods

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
        
        #endregion
    }
}