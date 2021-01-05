namespace OpenControls.Wpf.Utilities.ViewModel
{
    internal class InputTextViewModel : BaseViewModel
    {
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

        private string _label;
        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                _label = value;
                NotifyPropertyChanged("Label");
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                /*
                 * Only allow alphanumeric and spaces, and cannot start with a digit
                 */
                if (!string.IsNullOrEmpty(value))
                {
                    if (char.IsDigit(value[0]))
                    {
                        return;
                    }
                }
                foreach (var val in value)
                {
                    if (!char.IsLetterOrDigit(val) && val != ' ')
                    {
                        return;
                    }
                }
                        
                _text = value;
                NotifyPropertyChanged("Text");
            }
        }
    }
}
