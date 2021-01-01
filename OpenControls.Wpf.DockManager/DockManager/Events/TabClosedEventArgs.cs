using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager.Events
{
    internal class TabClosedEventArgs : System.EventArgs
    {
        public UserControl UserControl { get; set; }
    }
}
