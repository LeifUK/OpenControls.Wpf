using System.Windows.Controls;
using System.Windows.Media;

namespace OpenControls.Wpf.DockManager
{
    abstract class SelectablePane : Grid, ISelectablePane
    {
        public SelectablePane()
        {
            Tag = System.Guid.NewGuid();
            HighlightBrush = FindResource("SelectedPaneBrush") as Brush;
        }

        abstract public bool IsHighlighted { get; set; }

        public Brush HighlightBrush { get; private set; }
    }
}
