using System.Windows;
using System.Windows.Controls;

namespace OpenControls.Wpf.TabHeaderControlDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.MainViewModel();
        }

        private void _tabHeader1_SelectionChanged(object sender, System.EventArgs e)
        {
            ViewModel.MainViewModel mainViewModel = DataContext as ViewModel.MainViewModel;
            mainViewModel.TabHeader1Body = (_tabHeader1.SelectedItem as ViewModel.TabHeaderItem).HeaderText;
        }

        private void _tabHeader2_SelectionChanged(object sender, System.EventArgs e)
        {
            ViewModel.MainViewModel mainViewModel = DataContext as ViewModel.MainViewModel;
            mainViewModel.TabHeader2Body = (_tabHeader2.SelectedItem as ViewModel.TabHeaderItem).HeaderText;
        }

        private void _tabHeader3_SelectionChanged(object sender, System.EventArgs e)
        {
            ViewModel.MainViewModel mainViewModel = DataContext as ViewModel.MainViewModel;
            if (_tabHeader3.SelectedItem == null)
            {
                if (_tabHeader3.Items.Count > 0)
                {
                    _tabHeader3.SelectedItem = _tabHeader3.Items[0];
                }
                else
                {
                    mainViewModel.TabHeader3Body = null;
                    return;
                }
            }
            mainViewModel.TabHeader3Body = (_tabHeader3.SelectedItem as ViewModel.TabHeaderItem).HeaderText;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);
            if (parentObject == null)
            {
                return null;
            }

            return (parentObject is T) ? parentObject as T : FindParent<T>(parentObject);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ListBoxItem listBoxItem = FindParent<System.Windows.Controls.ListBoxItem>(sender as DependencyObject);
            if (listBoxItem == null)
            {
                return;
            }

            ViewModel.TabHeaderItem tabHeaderItem = listBoxItem.DataContext as ViewModel.TabHeaderItem;
            _tabHeader3.Items.Remove(tabHeaderItem);
        }

        internal static void DisplayItemsMenu(OpenControls.Wpf.TabHeaderControl.TabHeaderControl tabHeaderControl)
        {
            ContextMenu contextMenu = new ContextMenu();
            int i = 0;
            foreach (var item in tabHeaderControl.Items)
            {
                ViewModel.TabHeaderItem tabHeaderItem = item as ViewModel.TabHeaderItem;
                MenuItem menuItem = new MenuItem();
                menuItem.Header = tabHeaderItem.HeaderText;
                menuItem.IsChecked = item == tabHeaderControl.SelectedItem;
                menuItem.CommandParameter = i;
                ++i;
                menuItem.Command = new OpenControls.Wpf.Utilities.Command(delegate { tabHeaderControl.SelectedIndex = (int)menuItem.CommandParameter; }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            contextMenu.IsOpen = true;
        }

        private void _button1_Click(object sender, RoutedEventArgs e)
        {
            DisplayItemsMenu(_tabHeader1);
        }

        private void _button2_Click(object sender, RoutedEventArgs e)
        {
            DisplayItemsMenu(_tabHeader2);
        }

        private void _button3_Click(object sender, RoutedEventArgs e)
        {
            DisplayItemsMenu(_tabHeader3);
        }
    }
}
