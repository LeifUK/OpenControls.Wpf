using System.Windows;

namespace OpenControls.Wpf.RotaryControlDemo
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

    }
}
