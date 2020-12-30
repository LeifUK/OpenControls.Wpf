
namespace OpenControls.Wpf.RotaryControlDemo.ViewModel
{
    public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {

        private int _temperature = 20;
        public int Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
                NotifyPropertyChanged("Temperature");
            }
        }

        private double _volume = 1;
        public double Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = ((double)(long)(value * 100)) / 100.0;
                NotifyPropertyChanged("Volume");
            }
        }

        private int _output = 20;
        public int Output
        {
            get
            {
                return _output;
            }
            set
            {
                _output = value;
                NotifyPropertyChanged("Output");
            }
        }

        private double _rotaryControlDialValue;
        public double RotaryControlDialValue
        {
            get
            {
                return _rotaryControlDialValue;
            }
            set
            {
                _rotaryControlDialValue = value;
                NotifyPropertyChanged("RotaryControlDialValue");
            }
        }

        
        private double _pressure = 2;
        public double Pressure
        {
            get
            {
                return _pressure;
            }
            set
            {
                _pressure = value;
                NotifyPropertyChanged("Pressure");
            }
        }

        #region INotifyPropertyChanged

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged
    }
    }
