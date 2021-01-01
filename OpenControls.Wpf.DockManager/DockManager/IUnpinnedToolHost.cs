using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal interface IUnpinnedToolHost
    {
        Grid RootPane { get;}
        void ViewModelRemoved(IViewModel iViewModel);
        Controls.IToolListBox GetToolListBox(WindowLocation windowLocation);
        void PinToolPane(UnpinnedToolData unpinnedToolData, WindowLocation defaultWindowLocation);
        void UnpinToolPane(ToolPaneGroup toolPaneGroup, out UnpinnedToolData unpinnedToolData, out WindowLocation toolListBoxLocation);
    }
}
