using System.Windows.Controls;
using System.Windows.Media;

namespace OpenControls.Wpf.SurfacePlot.View
{
    /// <summary>
    /// Interaction logic for RawDataSettingsView.xaml
    /// </summary>
    public partial class ConfigurationControl : UserControl
    {
        public ConfigurationControl()
        {
            InitializeComponent();
            _comboBoxGridColours.ItemsSource = typeof(Colors).GetProperties();
            _comboBoxFrameColours.ItemsSource = typeof(Colors).GetProperties();
            _comboBoxLabelColours.ItemsSource = typeof(Colors).GetProperties();
            _comboBoxBackground.ItemsSource = typeof(Colors).GetProperties();
        }
    }
}
