
namespace OpenControls.Wpf.DatabaseDialogs.ViewModel
{
    public class BaseViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}
