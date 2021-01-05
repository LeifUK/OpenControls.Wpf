namespace OpenControls.Wpf.Utilities.ViewModel
{
    internal class ChooseStringViewModel : BaseViewModel
    {
        public ChooseStringViewModel()
        {
            Strings = new System.Collections.ObjectModel.ObservableCollection<string>();
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<string> _strings;
        public System.Collections.ObjectModel.ObservableCollection<string> Strings
        {
            get
            {
                return _strings;
            }
            set
            {
                _strings = value;
                NotifyPropertyChanged("Strings");
            }
        }

        private string _selectedString;
        public string SelectedString
        {
            get
            {
                return _selectedString;
            }
            set
            {
                _selectedString = value;
                NotifyPropertyChanged("SelectedString");
            }
        }
    }
}
