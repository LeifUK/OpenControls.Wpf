using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms;

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
            _comboBoxSelectedTabBackground.ItemsSource = typeof(Colors).GetProperties();
            _comboBoxUnselectedTabBackground.ItemsSource = typeof(Colors).GetProperties();
            ViewModel.MainViewModel mainViewModel = new ViewModel.MainViewModel();
            DataContext = mainViewModel;
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
            System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();
            int i = 0;
            foreach (var item in tabHeaderControl.Items)
            {
                ViewModel.TabHeaderItem tabHeaderItem = item as ViewModel.TabHeaderItem;
                System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem();
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

        private void SetFont(FontDialog fontDialog, OpenControls.Wpf.TabHeaderControl.TabHeaderControl tabHeaderControl)
        {
            tabHeaderControl.FontFamily = new FontFamily(fontDialog.Font.Name);
            tabHeaderControl.FontSize = fontDialog.Font.Size * 96.0 / 72.0;
            tabHeaderControl.FontWeight = fontDialog.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
            tabHeaderControl.FontStyle = fontDialog.Font.Italic ? FontStyles.Italic : FontStyles.Normal;

        }
        private void _buttonChooseFont_Click(object sender, RoutedEventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
            if (_tabHeader1.FontWeight == FontWeights.Bold)
            {
                fontStyle |= System.Drawing.FontStyle.Bold;
            }
            if (_tabHeader1.FontStyle == FontStyles.Italic)
            {
                fontStyle |= System.Drawing.FontStyle.Italic;
            }

            fontDialog.Font = new System.Drawing.Font(_tabHeader1.FontFamily.ToString(), (float)(_tabHeader1.FontSize * 72.0  / 96.0), fontStyle);

            var result = fontDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SetFont(fontDialog, _tabHeader1);
                SetFont(fontDialog, _tabHeader2);
                SetFont(fontDialog, _tabHeader3);
            }
        }
    }
}
