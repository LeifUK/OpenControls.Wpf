using System.Collections.ObjectModel;

namespace OpenControls.Wpf.SurfacePlotterDemo.ViewModel
{
    internal class MainViewModel : OpenControls.Wpf.Utilities.ViewModel.BaseViewModel
    {
        public MainViewModel()
        {
            OpenControls.Wpf.Serialisation.RegistryItemSerialiser registryItemSerialiser = new OpenControls.Wpf.Serialisation.RegistryItemSerialiser(RegKey());
            IConfigurationSerialiser = registryItemSerialiser;
            if (!registryItemSerialiser.OpenKey())
            {
                registryItemSerialiser.CreateKey();
            }

            IConfiguration = new OpenControls.Wpf.SurfacePlot.Model.Configuration();
            Speeds = new ObservableCollection<int>();
            for (int i = 5; i < 1001; i += 5)
            {
                Speeds.Add(i);
            }
            SelectedSpeed = 1;
        }

        private string RegKey()
        {
            return System.Environment.Is64BitOperatingSystem ? @"SOFTWARE\Wow6432Node\OpenControls.Wpf.SurfacePlotDemo" : @"SOFTWARE\OpenControls.Wpf.SurfacePlotDemo";
        }

        public readonly OpenControls.Wpf.SurfacePlot.Model.IConfiguration IConfiguration;
        private readonly OpenControls.Wpf.Serialisation.IConfigurationSerialiser IConfigurationSerialiser;

        public void Load()
        {
            IConfiguration.Load(IConfigurationSerialiser);
        }

        public void Save()
        {
            IConfiguration.Save(IConfigurationSerialiser);
        }

        private ObservableCollection<int> _speeds;
        public ObservableCollection<int> Speeds
        {
            get
            {
                return _speeds;
            }
            set
            {
                _speeds = value;
                NotifyPropertyChanged("Speeds");
            }
        }

        private int _selectedSpeed;
        public int SelectedSpeed
        {
            get
            {
                return _selectedSpeed;
            }
            set
            {
                _selectedSpeed = value;
                NotifyPropertyChanged("SelectedSpeed");
            }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
                NotifyPropertyChanged("IsRunning");
            }
        }
    }
}
