using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal interface ILayoutFactory
    {
        DocumentPanel MakeDocumentPanel();
        SplitterPane MakeSplitterPane(bool isHorizontal);
        DocumentPaneGroup MakeDocumentPaneGroup();
        ToolPaneGroup MakeToolPaneGroup();
        FloatingDocumentPaneGroup MakeFloatingDocumentPaneGroup();
        FloatingToolPaneGroup MakeFloatingToolPaneGroup();
        void MakeUnpinnedToolPaneGroup(WindowLocation windowLocation, ToolPaneGroup toolPaneGroup, string siblingGuid, bool isHorizontal, bool isFirst);

        string MakeDocumentKey(string contentId, string Url);

        void SetRootPane(Grid grid, out int row, out int column);
    }
}
