using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using OpenControls.Wpf.DockManager.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal delegate DockPane DelegateCreateDockPane();

    internal interface IDockPaneManager
    {
        void RegisterDockPane(DockPane dockPane);
        
        SelectablePane FindElementOfType(Type type, Grid parentGrid);

        DockPane ExtractDockPane(DockPane dockPane, out FrameworkElement frameworkElement);

        bool UngroupDockPane(DockPane dockPane, int index, double paneWidth);

        ISelectablePane FindSelectablePane(Grid grid, Point pointOnScreen);

        void Unfloat(FloatingPane floatingPane, SelectablePane selectedPane, WindowLocation windowLocation);

        void PinToolPane(UnpinnedToolData unpinnedToolData, WindowLocation defaultWindowLocation);

        void UnpinToolPane(ToolPaneGroup toolPaneGroup, out UnpinnedToolData unpinnedToolData, out WindowLocation toolListBoxLocation);

        void CreateDefaultLayout(List<UserControl> documentViews, List<UserControl> toolViews);

        void ValidateDockPanes(Grid grid, Dictionary<IViewModel, List<string>> viewModels, Type paneType);
    }
}
