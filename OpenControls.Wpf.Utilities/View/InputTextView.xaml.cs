using System.Windows;

namespace OpenControls.Wpf.Utilities.View
{
    /// <summary>
    /// Interaction logic for InputTextView.xaml
    /// </summary>
    public partial class InputTextView : Window
    {
        public InputTextView()
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

        public static bool ShowDialog(Window owner, string title, string label, ref string text)
        {
            OpenControls.Wpf.Utilities.View.InputTextView inputTextView = new OpenControls.Wpf.Utilities.View.InputTextView();
            OpenControls.Wpf.Utilities.ViewModel.InputTextViewModel inputTextViewModel = new OpenControls.Wpf.Utilities.ViewModel.InputTextViewModel();
            inputTextView.DataContext = inputTextViewModel;
            inputTextView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            inputTextView.Owner = owner;
            inputTextViewModel.Title = title;
            inputTextViewModel.Label = label;
            inputTextViewModel.Text = text;
            bool success = (inputTextView.ShowDialog() == true);
            if (success)
            {
                text = inputTextViewModel.Text;
            }
            return success;
        }
    }
}
