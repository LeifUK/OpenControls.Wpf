using System;
using System.Windows.Media;
using System.Windows;

namespace OpenControls.Wpf.DockManager
{
    /// <summary>
    /// Interaction logic for SidePane.xaml
    /// </summary>
    public partial class UnpinnedToolPane : Window
    {
        public UnpinnedToolPane()
        {
            InitializeComponent();
            _windowChrome.GlassFrameThickness = new Thickness(1);
            _toolPane.ShowAsUnPinned();
            _toolPane.UnPinClick += _toolPane_UnPinClick;
            _toolPane.CloseRequest += _toolPane_CloseRequest;

            _toolPane.Background = Brushes.Transparent;
        }

        private void _toolPane_CloseRequest(object sender, EventArgs e)
        {
            CloseRequest?.Invoke(this, null);
        }

        public event EventHandler PinClick;
        public event EventHandler CloseRequest;

        private void _toolPane_UnPinClick(object sender, System.EventArgs e)
        {
            PinClick?.Invoke(this, null);
        }

        internal WindowLocation WindowLocation
        {
            set
            {
                switch (value)
                {
                    case WindowLocation.TopSide:
                        _windowChrome.ResizeBorderThickness = new Thickness(0, 0, 0, 5);
                        break;
                    case WindowLocation.BottomSide:
                        _windowChrome.ResizeBorderThickness = new Thickness(0, 5, 0, 0);
                        break;
                    case WindowLocation.LeftSide:
                        _windowChrome.ResizeBorderThickness = new Thickness(0, 0, 5, 0);
                        break;
                    case WindowLocation.RightSide:
                        _windowChrome.ResizeBorderThickness = new Thickness(5, 0, 0, 0);
                        break;
                    default:
                        System.Diagnostics.Trace.Assert(false, "Unexpected WindowPosition");
                        break;
                }
            }
        }

        internal ToolPaneGroup ToolPane { get { return _toolPane; } }
    }
}
