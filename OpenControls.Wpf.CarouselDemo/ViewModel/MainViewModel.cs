namespace OpenControls.Wpf.CarouselDemo.ViewModel
{
    public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public MainViewModel()
        {
            RadioStationsDAB = new System.Collections.ObjectModel.ObservableCollection<Model.RadioStation>();
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "BBC Radio 1", ShortName = "BBC R1", ImageSource = "/Resources/BBCRadio1.jpg", Text = "Clara Amfo: Christine and the Queens are in the Live Lounge!" });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "BBC Radio 1 Extra", ShortName = "BBC R1 Extra", ImageSource = "/Resources/BBCRadio1Extra.jpg", Text = "Ace: Swedish-Gambian R&B singer Seinabo Say." });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "BBC Radio 2", ShortName = "BBC R2", ImageSource = "/Resources/BBCRadio2.jpg", Text = "Ken Bruce: Roger Daltrey chooses the Tracks of My Years." });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "BBC Radio 3", ShortName = "BBC R3", ImageSource = "/Resources/BBCRadio3.jpg", Text = "The London Philharmonic play Motorhead." });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "BBC Radio 4", ShortName = "BBC R4", ImageSource = "/Resources/BBCRadio4.jpg", Text = "Melvin Bragg talks to some clever people." });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "BBC Radio 4 Extra", ShortName = "BBC R4 Extra", ImageSource = "/Resources/BBCRadio4Extra.jpg", Text = "Around the Horne." });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "BBC Radio 5 Live", ShortName = "BBC R5 Live", ImageSource = "/Resources/BBCRadio5Live.jpg", Text = "The Emma Barnett Show: cutting edge political debate." });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "BBC Radio 5 Live Sports", ShortName = "BBC R5 Live Sports", ImageSource = "/Resources/BBCRadio5LiveSports.jpg", Text = "Football chat with Steve Crossman and guests Leon Osman and Simon Grayson." });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "BBC Radio 6 Music", ShortName = "BBC R6 Music", ImageSource = "/Resources/BBCRadio6Music.jpg", Text = "Some music. What did you expect?" });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "BBC Radio Asian Network", ShortName = "BBC R Asian", ImageSource = "/Resources/BBCRadioAsianNetwork.jpg", Text = "Noreen Khan: Nadia Ali sits in." });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "Heart", ShortName = "Heart", ImageSource = "/Resources/Heart.png", Text = "Push the Button by Sugababes" });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "Jazz FM", ShortName = "Jazz FM", ImageSource = "/Resources/JazzFM.jpg", Text = "Kenneth Clarke presents A History Of Jazz" });
            RadioStationsDAB.Add(new Model.RadioStation() { Name = "Capital FM", ShortName = "Capital FM", ImageSource = "/Resources/CapitalFM.png", Text = "Bassman: Hear all the hottest tunes and freshest music news with The Bassman" });

            SelectedRadioStationDAB = RadioStationsDAB[0];
        }

        private System.Collections.ObjectModel.ObservableCollection<Model.RadioStation> _radioStationsDAB;
        public System.Collections.ObjectModel.ObservableCollection<Model.RadioStation> RadioStationsDAB
        {
            get
            {
                return _radioStationsDAB;
            }
            set
            {
                _radioStationsDAB = value;
                NotifyPropertyChanged("RadioStationsDAB");
            }
        }

        private Model.RadioStation _selectedRadioStationDAB;
        public Model.RadioStation SelectedRadioStationDAB
        {
            get
            {
                return _selectedRadioStationDAB;
            }
            set
            {
                _selectedRadioStationDAB = value;
                NotifyPropertyChanged("SelectedRadioStationDAB");
            }
        }

        // Delete the selected item
        public void Delete()
        {
            //System.Collections.Generic.List<Model.RadioStation> newItems = RadioStationsDAB.ToList();
            //newItems.Remove(SelectedRadioStationDAB);
            //RadioStationsDAB = new System.Collections.ObjectModel.ObservableCollection<Model.RadioStation>(newItems);
            RadioStationsDAB.Remove(SelectedRadioStationDAB);
            SelectedRadioStationDAB = RadioStationsDAB[0];
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
