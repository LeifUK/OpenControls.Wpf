using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace OpenControls.Wpf.SurfacePlot.View
{
    /// <summary>
    /// Interaction logic for ConfigurationViewNew.xaml
    /// </summary>
    public partial class ConfigurationView : Window
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }

        private void _buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void _buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        ViewModel.ConfigurationControlViewModel _configurationControlViewModel;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Trace.Assert(DataContext is ViewModel.ConfigurationViewModel);

            _configurationControlViewModel = new ViewModel.ConfigurationControlViewModel((DataContext as ViewModel.ConfigurationViewModel).IConfiguration);
            _configurationControl.DataContext = _configurationControlViewModel;
        }
    }
}
