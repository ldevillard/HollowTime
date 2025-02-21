using HollowTime.Components.Scramble;
using HollowTime.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace HollowTime.Components
{
    public partial class CubeVisualizer : ComponentBase
    {
        string currentScramble { get; set; } = string.Empty;
        string currentEvent { get; set; } = string.Empty;
        
        bool is3D
        {
            get => _is3D;
            set
            {
                _is3D = value;
                buttoncLabel = value ? "2D" : "3D";
            }
        }
        bool _is3D;
        
        string buttoncLabel = "3D";

        public void RefreshVisualizer(ScrambleData scrambleData)
        {
            currentScramble = scrambleData.Scramble;
            currentEvent = scrambleData.EventType;
            StateHasChanged();
        }
        
        void onButtonClicked()
        {
            is3D = !is3D;
        }
    }
}
