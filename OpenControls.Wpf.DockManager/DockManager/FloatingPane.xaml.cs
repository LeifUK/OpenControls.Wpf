using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenControls.Wpf.DockManager
{
    /// <summary>
    /// Interaction logic for FloatingWindow.xaml
    /// </summary>
    internal partial class FloatingPane : Window, IFloatingPane
    {
        internal FloatingPane(IViewContainer iViewContainer)
        {
            Tag = System.Guid.NewGuid();
            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;
            _parentContainer.Children.Add(iViewContainer as UIElement);
            Grid.SetRow(iViewContainer as UIElement, 1);
            IViewContainer = iViewContainer;
            IViewContainer.FloatTabRequest += IViewContainer_FloatTabRequest;
            IViewContainer.TabClosed += IViewContainer_TabClosed;

            _gridHeader.SetResourceReference(Grid.BackgroundProperty, "FloatingPaneTitleBarBackground");
            _textBlockTitle.SetResourceReference(TextBlock.StyleProperty, "FloatingPaneTitleStyle");
            SetResourceReference(Window.BackgroundProperty, "FloatingPaneBackground");

            (IViewContainer as ViewContainer).Margin = (Thickness)FindResource("FloatingPanePadding");

            Style style = TryFindResource("FloatingPaneCloseButtonStyle") as Style;
            if (style != null)
            {
                _buttonClose.Style = style;
            }
            style = TryFindResource("FloatingPaneMaximiseButtonStyle") as Style;
            if (style != null)
            {
                _buttonMaximize.Style = style;
            }
            style = TryFindResource("FloatingPaneMinimizeButtonStyle") as Style;
            if (style != null)
            {
                _buttonMinimize.Style = style;
            }
            style = TryFindResource("FloatingPaneRestoreButtonStyle") as Style;
            if (style != null)
            {
                _buttonRestore.Style = style;
            }
        }

        private void IViewContainer_TabClosed(object sender, Events.TabClosedEventArgs e)
        {
            TabClosed?.Invoke(this, e);
        }

        private void IViewContainer_FloatTabRequest(object sender, EventArgs e)
        {
            FloatTabRequest?.Invoke(this, e);
        }

        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainWindowBorder.BorderThickness = new Thickness(8);
                _buttonRestore.Visibility = Visibility.Visible;
                _buttonMaximize.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
                _buttonRestore.Visibility = Visibility.Collapsed;
                _buttonMaximize.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public event Events.TabClosedEventHandler TabClosed;
        public event EventHandler FloatTabRequest;
        public event EventHandler UngroupCurrent;
        public event EventHandler Ungroup;
        public event EventHandler EndDrag;

        private void _buttonMenu_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = null;

            int count = IViewContainer.GetUserControlCount();

            if (count > 2)
            {
                menuItem = new MenuItem();
                menuItem.Header = "Ungroup Current";
                menuItem.IsChecked = false;
                menuItem.Command = new Command(delegate { UngroupCurrent?.Invoke(this, null); }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            if (count > 1)
            {
                menuItem = new MenuItem();
                menuItem.Header = "Ungroup";
                menuItem.IsChecked = false;
                menuItem.Command = new Command(delegate { Ungroup?.Invoke(this, null); }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            menuItem = new MenuItem();
            menuItem.Header = "Freeze Aspect Ratio";
            menuItem.IsChecked = false;
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        System.Threading.Tasks.Task _dragTask;

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (_dragTask == null)
            {
                // Use a task to detect when the drag ends
                _dragTask = new Task(delegate
                {
                    while (OpenControls.Wpf.DockManager.Controls.Utilities.IsLeftMouseButtonDown())
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    EndDrag?.Invoke(this, null);
                    _dragTask = null;
                });
                _dragTask.Start();
            }
        }

        public IViewContainer IViewContainer { get; private set; }

        #region ISelectablePane

        public bool IsHighlighted { get; set; }
        public Brush HighlightBrush { get; private set; }

        #endregion ISelectablePane
    }
}