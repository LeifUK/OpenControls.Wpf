namespace OpenControls.Wpf.Utilities.View
{
    public static class ExtensionMethods
    {
        private static System.Action EmptyDelegate = delegate () { };

        public static void Refresh(this System.Windows.UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            System.Threading.Thread.Sleep(1);
        }
    }
}
