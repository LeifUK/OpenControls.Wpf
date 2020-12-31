using System.Windows;

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
            mainViewModel.TabHeader3Body = (_tabHeader3.SelectedItem as ViewModel.TabHeaderItem).HeaderText;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void ListBoxItem_PreviewMouseLeftButtonDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
