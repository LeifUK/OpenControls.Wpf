using System;
using System.Windows;
using System.Windows.Media;

namespace OpenControls.Wpf.DockManager
{
    internal class FloatingDocumentPaneGroup : FloatingPane
    {
        internal FloatingDocumentPaneGroup() : base(new DocumentContainer())
        {
            IViewContainer.SelectionChanged += IViewContainer_SelectionChanged;
            (IViewContainer as DocumentContainer).HideCommandsButton();
        }

        private void IViewContainer_SelectionChanged(object sender, EventArgs e)
        {
            FloatingViewModel floatingViewModel = DataContext as FloatingViewModel;
            System.Diagnostics.Trace.Assert(floatingViewModel != null);

            floatingViewModel.Title = Application.Current.MainWindow.Title + " - " + IViewContainer.URL;
        }
    }
}
