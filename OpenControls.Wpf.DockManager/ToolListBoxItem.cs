namespace OpenControls.Wpf.DockManager
{
    internal class ToolListBoxItem : Controls.IToolListBoxItem
    {
        public int Index { get; set; }
        public IViewContainer IViewContainer { get; set; }
        public string Title
        {
            get
            {
                System.Diagnostics.Trace.Assert(IViewContainer != null);

                IViewModel iViewModel = IViewContainer.GetIViewModel(Index);
                System.Diagnostics.Trace.Assert(iViewModel != null);

                return iViewModel.Title;
            }
        }
        public double Height { get; set; }
        public double Width { get; set; }

        public bool Equals(ToolListBoxItem other)
        {
            return
                (IViewContainer == other.IViewContainer) &&
                (Index == other.Index);
        }

        public IViewModel IViewModel
        {
            get
            {
                return IViewContainer.GetIViewModel(Index);
            }
        }
    }
}
