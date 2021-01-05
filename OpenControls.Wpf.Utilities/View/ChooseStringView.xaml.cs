using System.Windows;

namespace OpenControls.Wpf.Utilities.View
{
    /// <summary>
    /// Interaction logic for ChooseStringView.xaml
    /// </summary>
    public partial class ChooseStringView : Window
    {
        public ChooseStringView()
        {
            InitializeComponent();
        }

        private void _buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void _buttonOkay_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
