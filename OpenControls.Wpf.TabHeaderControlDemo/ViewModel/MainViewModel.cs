
namespace OpenControls.Wpf.TabHeaderControlDemo.ViewModel
{
    public class TabHeaderItem
    {
        public string Label { get; set; }
        public int ID { get; set; }

        public string HeaderText
        {
            get
            {
                return Label + " : " + ID;
            }
        }
    }

    public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public MainViewModel()
        {
            ListBoxItems = new System.Collections.ObjectModel.ObservableCollection<TabHeaderItem>();
            ListBoxItems.Add(new TabHeaderItem() { ID = 0, Label = "Short item" });
            ListBoxItems.Add(new TabHeaderItem() { ID = 1, Label = "A longer item" });
            ListBoxItems.Add(new TabHeaderItem() { ID = 2, Label = "A quite long item" });
            ListBoxItems.Add(new TabHeaderItem() { ID = 3, Label = "A really quite long item" });
            SelectedTabForeground = System.Windows.Media.Brushes.Black;
            SelectedTabBackground = System.Windows.Media.Brushes.AliceBlue;
            SelectedTabBorderBrush = System.Windows.Media.Brushes.Black;
            SelectedTabBorderThickness = new System.Windows.Thickness(2);
            UnselectedTabForeground = System.Windows.Media.Brushes.White;
            UnselectedTabBackground = System.Windows.Media.Brushes.CornflowerBlue;
            UnselectedTabBorderBrush = System.Windows.Media.Brushes.White;
            UnselectedTabBorderThickness = new System.Windows.Thickness(0);
        }

        private System.Collections.ObjectModel.ObservableCollection<TabHeaderItem> _listBoxItems;
        public System.Collections.ObjectModel.ObservableCollection<TabHeaderItem> ListBoxItems
        {
            get
            {
                return _listBoxItems;
            }
            set
            {
                _listBoxItems = value;
                NotifyPropertyChanged("ListBoxItems");
            }
        }

        public System.Windows.Media.Brush _selectedTabBackground;
        public System.Windows.Media.Brush SelectedTabBackground
        {
            get
            {
                return _selectedTabBackground;
            }
            set
            {
                _selectedTabBackground = value;
                NotifyPropertyChanged("SelectedTabBackground");
            }
        }

        public System.Windows.Media.Brush _selectedTabForeground;
        public System.Windows.Media.Brush SelectedTabForeground
        {
            get
            {
                return _selectedTabForeground;
            }
            set
            {
                _selectedTabForeground = value;
                NotifyPropertyChanged("SelectedTabForeground");
            }
        }

        private System.Windows.Media.Brush _selectedTabBorderBrush;
        public System.Windows.Media.Brush SelectedTabBorderBrush
        {
            get
            {
                return _selectedTabBorderBrush;
            }
            set
            {
                _selectedTabBorderBrush = value;
                NotifyPropertyChanged("SelectedTabBorderBrush");
            }
        }

        private System.Windows.Thickness _selectedTabBorderThickness;
        public System.Windows.Thickness SelectedTabBorderThickness
        {
            get
            {
                return _selectedTabBorderThickness;
            }
            set
            {
                _selectedTabBorderThickness = value;
                NotifyPropertyChanged("SelectedTabBorderThickness");
            }
        }

        public System.Windows.Media.Brush _unselectedTabBackground;
        public System.Windows.Media.Brush UnselectedTabBackground
        {
            get
            {
                return _unselectedTabBackground;
            }
            set
            {
                _unselectedTabBackground = value;
                NotifyPropertyChanged("UnselectedTabBackground");
            }
        }

        public System.Windows.Media.Brush _unselectedTabForeground;
        public System.Windows.Media.Brush UnselectedTabForeground
        {
            get
            {
                return _unselectedTabForeground;
            }
            set
            {
                _unselectedTabForeground = value;
                NotifyPropertyChanged("UnselectedTabForeground");
            }
        }

        private System.Windows.Media.Brush _unselectedTabBorderBrush;
        public System.Windows.Media.Brush UnselectedTabBorderBrush
        {
            get
            {
                return _unselectedTabBorderBrush;
            }
            set
            {
                _unselectedTabBorderBrush = value;
                NotifyPropertyChanged("UnselectedTabBorderBrush");
            }
        }

        private System.Windows.Thickness _unselectedTabBorderThickness;
        public System.Windows.Thickness UnselectedTabBorderThickness
        {
            get
            {
                return _unselectedTabBorderThickness;
            }
            set
            {
                _unselectedTabBorderThickness = value;
                NotifyPropertyChanged("UnselectedTabBorderThickness");
            }
        }

        private string _tabHeader1Body;
        public string TabHeader1Body
        {
            get
            {
                return _tabHeader1Body;
            }
            set
            {
                _tabHeader1Body = value;
                NotifyPropertyChanged("TabHeader1Body");
            }
        }

        private string _tabHeader2Body;
        public string TabHeader2Body
        {
            get
            {
                return _tabHeader2Body;
            }
            set
            {
                _tabHeader2Body = value;
                NotifyPropertyChanged("TabHeader2Body");
            }
        }

        private string _tabHeader3Body;
        public string TabHeader3Body
        {
            get
            {
                return _tabHeader3Body;
            }
            set
            {
                _tabHeader3Body = value;
                NotifyPropertyChanged("TabHeader3Body");
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
