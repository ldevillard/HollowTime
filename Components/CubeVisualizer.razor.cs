using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace HollowTime.Components
{
    public partial class CubeVisualizer : ComponentBase
    {
        [Parameter]
        public string CurrentScramble { get; set; } = string.Empty;

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

        void onButtonClicked()
        {
            is3D = !is3D;
        }
    }
}
