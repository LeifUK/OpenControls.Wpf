using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenControls.Wpf.DockManager
{
    internal abstract class DockPane : SelectablePane
    {
        public DockPane(IViewContainer iViewContainer)
        {
            IViewContainer = iViewContainer;
            IViewContainer.TabClosed += IViewContainer_TabClosed;
            IViewContainer.FloatTabRequest += IViewContainer_FloatTabRequest;
            Children.Add(iViewContainer as UIElement);
            Border = new Border();
            Border.Background = Brushes.Transparent;
            Border.IsHitTestVisible = false;
            Grid.SetRow(Border, 0);
            Grid.SetRowSpan(Border, 99);
            Grid.SetColumn(Border, 0);
            Grid.SetColumnSpan(Border, 99);
            Grid.SetZIndex(Border, -1);
            Children.Add(Border);
        }

        private void IViewContainer_FloatTabRequest(object sender, EventArgs e)
        {
            FloatTabRequest?.Invoke(this, e);
        }

        private void IViewContainer_TabClosed(object sender, Events.TabClosedEventArgs e)
        {
            TabClosed?.Invoke(this, e);
        }

        public event EventHandler CloseRequest;
        public event Events.FloatEventHandler Float;
        public event Events.TabClosedEventHandler TabClosed;
        public event EventHandler FloatTabRequest;
        public event EventHandler UngroupCurrent;
        public event EventHandler Ungroup;

        public Border Border;

        protected void FireCloseRequest()
        {
            CloseRequest?.Invoke(this, null);
        }

        protected void FireFloat(bool drag)
        {
            Events.FloatEventArgs floatEventArgs = new Events.FloatEventArgs();
            floatEventArgs.Drag = drag;
            Float?.Invoke(this, floatEventArgs);
        }

        public readonly IViewContainer IViewContainer;

        protected void DisplayGeneralMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Float";
            menuItem.IsChecked = false;
            menuItem.Command = new Command(delegate { FireFloat(false); }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            int viewCount = IViewContainer.GetUserControlCount();
            if (viewCount > 2)
            {
                menuItem = new MenuItem();
                menuItem.Header = "Ungroup Current";
                menuItem.IsChecked = false;
                menuItem.Command = new Command(delegate { UngroupCurrent?.Invoke(this, null); }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            if (viewCount > 1)
            {
                menuItem = new MenuItem();
                menuItem.Header = "Ungroup";
                menuItem.IsChecked = false;
                menuItem.Command = new Command(delegate { Ungroup?.Invoke(this, null); }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            contextMenu.IsOpen = true;
        }
    }
}
