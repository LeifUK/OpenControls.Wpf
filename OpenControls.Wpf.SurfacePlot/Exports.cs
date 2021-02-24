using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenControls.Wpf.SurfacePlot
{
    public class Exports
    {
        public static void ShowConfigurationDialog(Model.IConfiguration iConfiguration)
        {
            View.ConfigurationView configurationView = new View.ConfigurationView();
            ViewModel.ConfigurationViewModel configurationViewModel = new ViewModel.ConfigurationViewModel(iConfiguration);
            configurationView.DataContext = configurationViewModel;
            configurationView.ShowDialog();
        }
    }
}
