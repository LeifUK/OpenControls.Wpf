using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenControls.Wpf.DockManager
{
    [System.Flags]
    public enum WindowLocation
    {
        None = 0x000,
        Middle = 0x001,
        Right = 0x002,
        Left = 0x004,
        Top = 0x008,
        Bottom = 0x010,

        RightSide = 0x020,
        LeftSide = 0x040,
        TopSide = 0x080,
        BottomSide = 0x100,
    }
}
