namespace OpenControls.Wpf.SurfacePlot.ViewModel
{
    public class ConfigurationViewModel
    {
        public ConfigurationViewModel(Model.IConfiguration iConfiguration)
        {
            IConfiguration = iConfiguration;
        }

        public Model.IConfiguration IConfiguration;
    }
}
