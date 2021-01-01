using System.Windows;
using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal interface IFloatingPaneHost
    {
        Grid RootPane { get; }
        Grid RootGrid { get; }
        void RemoveViewModel(IViewModel iViewModel);
        ISelectablePane FindSelectablePane(Point pointOnScreen);
        void Unfloat(FloatingPane floatingPane, SelectablePane selectedPane, WindowLocation windowLocation);
        void ActiveDocumentChanged(FloatingDocumentPaneGroup floatingDocumentPaneGroup);
    }
}