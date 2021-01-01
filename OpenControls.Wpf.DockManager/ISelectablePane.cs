using System.Windows;
using System.Windows.Media;

namespace OpenControls.Wpf.DockManager
{
    interface ISelectablePane
    {
        bool IsHighlighted { get; set; }
        Brush HighlightBrush { get; }
        DependencyObject Parent { get; }
        double ActualWidth { get; }
        double ActualHeight { get; }
        Point PointToScreen(Point point);
    }
}
