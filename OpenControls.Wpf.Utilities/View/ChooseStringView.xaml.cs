using System.Windows;
using System.Collections.Generic;

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

        public static bool ShowDialog(Window owner, string title, List<string> values, ref string selectedValue)
        {
            OpenControls.Wpf.Utilities.View.ChooseStringView chooseStringView = new OpenControls.Wpf.Utilities.View.ChooseStringView();
            OpenControls.Wpf.Utilities.ViewModel.ChooseStringViewModel chooseStringViewModel = new OpenControls.Wpf.Utilities.ViewModel.ChooseStringViewModel();
            chooseStringView.DataContext = chooseStringViewModel;
            chooseStringView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            chooseStringView.Owner = owner;
            chooseStringViewModel.Title = title;
            chooseStringViewModel.Strings = new System.Collections.ObjectModel.ObservableCollection<string>(values);
            bool success = (chooseStringView.ShowDialog() == true);
            if (success)
            {
                selectedValue = chooseStringViewModel.SelectedString;
            }
            return success;
        }
    }
}
