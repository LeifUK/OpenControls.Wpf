namespace ExampleDockManagerViews.ViewModel
{
    public class DummyViewModel : OpenControls.Wpf.DockManager.IViewModel
    {
        public string URL { get; set; }
        public string Title { get; set; }
        public string Tooltip 
        { 
            get
            {
                return URL;
            }
        }

        public bool CanClose
        {
            get
            {
                return true;
            }
        }

        public bool HasChanged
        {
            get
            {
                return true;
            }
        }

        public void Save()
        {
            // Do nowt!
        }

        public void Close()
        {
            // Do nowt!
        }
    }
}
