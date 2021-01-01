using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenControls.Wpf.DockManager
{
    /// <summary>
    /// Interaction logic for EdgeLocationPane.xaml
    /// </summary>
    public partial class SideLocationPane : Window
    {
        public SideLocationPane()
        {
            InitializeComponent();
        }

        public WindowLocation TrySelectIndicator(Point cursorPositionOnScreen)
        {
            if (_buttonTopEdge.InputHitTest(_buttonTopEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.TopSide;
            }

            if (_buttonLeftEdge.InputHitTest(_buttonLeftEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.LeftSide;
            }

            if (_buttonRightEdge.InputHitTest(_buttonRightEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.RightSide;
            }

            if (_buttonBottomEdge.InputHitTest(_buttonBottomEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.BottomSide;
            }

            return WindowLocation.None;
        }
    }
}
