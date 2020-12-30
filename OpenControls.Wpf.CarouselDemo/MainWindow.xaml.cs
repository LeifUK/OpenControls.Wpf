using System.Windows;

namespace OpenControls.Wpf.CarouselDemo
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

            _carouselDABRadioStations.SelectionChanged += _carouselDABRadioStations_SelectionChanged;
        }

        private void _carouselDABRadioStations_SelectionChanged(FrameworkElement selectedElement)
        {
            var viewModel = DataContext as ViewModel.MainViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.SelectedRadioStationDAB = selectedElement.DataContext as Model.RadioStation;
        }

        private void _buttonLeftArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselDABRadioStations.RotateRight();
        }

        private void _buttonRightArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselDABRadioStations.RotateLeft();
        }

        private void _checkBoxVerticalCarousel_Click(object sender, RoutedEventArgs e)
        {
            _carouselDABRadioStations.VerticalOrientation = _checkBoxVerticalCarousel.IsChecked.HasValue ? _checkBoxVerticalCarousel.IsChecked.Value : false;
        }

        private void _buttonLeftManyArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselDABRadioStations.RotateIncrement(-5);
        }

        private void _buttonRightManyArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselDABRadioStations.RotateIncrement(5);
        }

        private void _buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewModel.MainViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.Delete();
        }
    }
}
