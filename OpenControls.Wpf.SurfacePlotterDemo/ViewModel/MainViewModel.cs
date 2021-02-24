using System.Collections.ObjectModel;

namespace OpenControls.Wpf.SurfacePlotterDemo.ViewModel
{
    internal class MainViewModel : OpenControls.Wpf.Utilities.ViewModel.BaseViewModel
    {
        public MainViewModel()
        {
            IConfigurationSerialiser = new SurfacePlot.Model.ConfigurationSerialiser();
            (IConfigurationSerialiser as SurfacePlot.Model.ConfigurationSerialiser).CurrentRegistryKey = OpenRegKey();

            IConfiguration = new OpenControls.Wpf.SurfacePlot.Model.Configuration();
            Speeds = new ObservableCollection<int>();
            for (int i = 5; i < 1001; i += 5)
            {
                Speeds.Add(i);
            }
            SelectedSpeed = 1;
        }

        private Microsoft.Win32.RegistryKey OpenRegKey()
        {
            string path = System.Environment.Is64BitOperatingSystem ? @"SOFTWARE\Wow6432Node\OpenControls.Wpf.SurfacePlotDemo" : @"SOFTWARE\OpenControls.Wpf.SurfacePlotDemo";
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path, true);
            if (key == null)
            {
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(path);
            }

            if (key == null)
            {
                return null;
            }

            Microsoft.Win32.RegistryKey touchHubKey = key.OpenSubKey("RawDataSettings", true);
            if (touchHubKey == null)
            {
                touchHubKey = key.CreateSubKey("RawDataSettings");
            }

            return touchHubKey;
        }

        public readonly OpenControls.Wpf.SurfacePlot.Model.IConfiguration IConfiguration;
        private readonly OpenControls.Wpf.SurfacePlot.Model.IConfigurationSerialiser IConfigurationSerialiser;

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
