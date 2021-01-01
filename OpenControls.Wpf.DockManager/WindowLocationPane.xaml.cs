using System.Windows;

namespace OpenControls.Wpf.DockManager
{
    /// <summary>
    /// Interaction logic for IndicatorPane.xaml
    /// </summary>
    public partial class WindowLocationPane : Window
    {
        public WindowLocationPane()
        {
            InitializeComponent();
        }

        public WindowLocation TrySelectIndicator(Point cursorPositionOnScreen)
        {
            if (_buttonTop.InputHitTest(_buttonTop.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Top;
            }

            if (_buttonLeft.InputHitTest(_buttonLeft.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Left;
            }

            if (_buttonMiddle.InputHitTest(_buttonMiddle.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Middle;
            }

            if (_buttonRight.InputHitTest(_buttonRight.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Right;
            }

            if (_buttonBottom.InputHitTest(_buttonBottom.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Bottom;
            }

            return WindowLocation.None;
        }

        public void ShowIcons(WindowLocation windowLocations)
        {
            _buttonLeft.Visibility = windowLocations.HasFlag(WindowLocation.Left) ? Visibility.Visible : Visibility.Hidden;
            _buttonTop.Visibility = windowLocations.HasFlag(WindowLocation.Top) ? Visibility.Visible : Visibility.Hidden;
            _buttonRight.Visibility = windowLocations.HasFlag(WindowLocation.Right) ? Visibility.Visible : Visibility.Hidden;
            _buttonBottom.Visibility = windowLocations.HasFlag(WindowLocation.Bottom) ? Visibility.Visible : Visibility.Hidden;
            _buttonMiddle.Visibility = windowLocations.HasFlag(WindowLocation.Middle) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
