using System.Collections.Generic;

namespace OpenControls.Wpf.DockManager
{
    internal interface IFloatingPaneManager
    {
        void Shutdown();
        void Clear();
        void CancelSelection();
        void Show(FloatingDocumentPaneGroup floatingDocumentPaneGroup);
        void Show(FloatingToolPaneGroup floatingToolPaneGroup);
        void ValidateFloatingToolPanes(Dictionary<IViewModel, List<string>> viewModelUrlDictionary);
        void ValidateFloatingDocumentPanes(Dictionary<IViewModel, List<string>> viewModelUrlDictionary);
        List<IFloatingPane> FloatingToolPaneGroups { get; }
        List<IFloatingPane> FloatingDocumentPaneGroups { get; }
    }
}
