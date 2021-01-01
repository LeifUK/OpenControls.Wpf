using System.Windows.Media;

namespace OpenControls.Wpf.DockManager
{
    internal interface IFloatingPane : ISelectablePane
    {
        IViewContainer IViewContainer { get; }
        void Close();
    }
}
