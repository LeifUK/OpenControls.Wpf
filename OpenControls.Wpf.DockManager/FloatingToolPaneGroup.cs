using System;
using System.Windows;
using System.Windows.Media;

namespace OpenControls.Wpf.DockManager
{
    internal class FloatingToolPaneGroup : FloatingPane
    {
        internal FloatingToolPaneGroup() : base(new ToolContainer())
        {
            IViewContainer.SelectionChanged += IViewContainer_SelectionChanged;
        }

        private void IViewContainer_SelectionChanged(object sender, EventArgs e)
        {
            FloatingViewModel floatingViewModel = DataContext as FloatingViewModel;
            System.Diagnostics.Trace.Assert(floatingViewModel != null);

            floatingViewModel.Title = Application.Current.MainWindow.Title + " - " + IViewContainer.Title;
        }
    }
}
