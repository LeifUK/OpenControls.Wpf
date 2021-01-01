using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenControls.Wpf.DockManager
{
    internal interface IDockPaneHost
    {
        void Clear();
        void FrameworkElementRemoved(FrameworkElement frameworkElement);
        void RemoveViewModel(IViewModel iViewModel);
        Grid RootPane { get; set; }
        Grid RootGrid { get; }
        UIElementCollection Children { get; }
        List<UserControl> LoadToolViews(ObservableCollection<IViewModel> viewModels);
        List<UserControl> LoadDocumentViews(ObservableCollection<IViewModel> viewModels);
        void ActiveDocumentChanged(DocumentPaneGroup documentPaneGroup);
    }
}
