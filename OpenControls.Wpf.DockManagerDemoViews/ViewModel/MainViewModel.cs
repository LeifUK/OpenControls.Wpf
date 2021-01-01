using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using OpenControls.Wpf.DockManager;
using ExampleDockManagerViews.ViewModel;

namespace ExampleDockManagerViews.ViewModel
{
    public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Tools = new ObservableCollection<IViewModel>();
            Tools.Add(new ToolOneViewModel());
            Tools.Add(new ToolTwoViewModel());
            Tools.Add(new ToolThreeViewModel());
            Tools.Add(new ToolFourViewModel());
            Tools.Add(new ToolFiveViewModel());

            Documents = new ObservableCollection<IViewModel>();
            Documents.Add(DocumentOne);
            Documents.Add(DocumentTwo);
            Documents.Add(DocumentThree);
            Documents.Add(DocumentFour);
            Documents.Add(DocumentFive);

            LayoutLoaded = false;
        }

        public readonly IViewModel DocumentOne = new DocumentOneViewModel() { URL = "C:\\Data\\File-1.txt", Title = "File-1.txt" };
        public readonly IViewModel DocumentTwo = new DocumentOneViewModel() { URL = "C:\\Data\\File-2.txt", Title = "File-2.txt" };
        public readonly IViewModel DocumentThree = new DocumentTwoViewModel() { URL = "C:\\Data\\Folder\\File-3.txt", Title = "File-3.txt" };
        public readonly IViewModel DocumentFour = new DocumentTwoViewModel() { URL = "D:\\Data\\Folder\\File-4.txt", Title = "File-4.txt" };
        public readonly IViewModel DocumentFive = new DocumentTwoViewModel() { URL = "D:\\Data\\Folder\\File-5.txt", Title = "File-5.txt" };

        private System.Collections.ObjectModel.ObservableCollection<IViewModel> _documents;
        public System.Collections.ObjectModel.ObservableCollection<IViewModel> Documents
        {
            get
            {
                return _documents;
            }
            set
            {
                if (value != Documents)
                {
                    _documents = value;
                    NotifyPropertyChanged("Documents");
                }
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<IViewModel> _tools;
        public System.Collections.ObjectModel.ObservableCollection<IViewModel> Tools
        {
            get
            {
                return _tools;
            }
            set
            {
                if (value != Tools)
                {
                    _tools = value;
                    NotifyPropertyChanged("Tools");
                }
            }
        }

        private bool _layoutLoaded;
        public bool LayoutLoaded
        {
            get
            {
                return _layoutLoaded;
            }
            set
            {
                _layoutLoaded = value;
                NotifyPropertyChanged("LayoutLoaded");
            }
        }

        public bool IsToolVisible(Type type)
        {
            return (Tools.Where(n => n.GetType() == type).Count() > 0);
        }

        public void ShowTool(bool show, Type type)
        {
            bool isVisible = IsToolVisible(type);
            if (isVisible == show)
            {
                return;
            }

            if (show == false)
            {
                var enumerator = Tools.Where(n => n.GetType() == type);
                Tools.Remove(enumerator.First());
            }
            else
            {
                IViewModel iViewModel = (IViewModel)Activator.CreateInstance(type);
                System.Diagnostics.Trace.Assert(iViewModel != null);
                Tools.Add(iViewModel);
            }
        }

        public bool ToolOneVisible
        {
            get
            {
                return (Tools.Where(n => n.GetType() == typeof(ExampleDockManagerViews.ViewModel.ToolOneViewModel)).Count() > 0);
            }
            set
            {
                ShowTool(value, typeof(ExampleDockManagerViews.ViewModel.ToolOneViewModel));
                NotifyPropertyChanged("ToolOneVisible");
            }
        }

        public bool ToolTwoVisible
        {
            get
            {
                return (Tools.Where(n => n.GetType() == typeof(ExampleDockManagerViews.ViewModel.ToolTwoViewModel)).Count() > 0);
            }
            set
            {
                ShowTool(value, typeof(ExampleDockManagerViews.ViewModel.ToolTwoViewModel));
                NotifyPropertyChanged("ToolTwoVisible");
            }
        }

        public bool ToolThreeVisible
        {
            get
            {
                return (Tools.Where(n => n.GetType() == typeof(ExampleDockManagerViews.ViewModel.ToolThreeViewModel)).Count() > 0);
            }
            set
            {
                ShowTool(value, typeof(ExampleDockManagerViews.ViewModel.ToolThreeViewModel));
                NotifyPropertyChanged("ToolThreeVisible");
            }
        }

        public bool ToolFourVisible
        {
            get
            {
                return (Tools.Where(n => n.GetType() == typeof(ExampleDockManagerViews.ViewModel.ToolFourViewModel)).Count() > 0);
            }
            set
            {
                ShowTool(value, typeof(ExampleDockManagerViews.ViewModel.ToolFourViewModel));
                NotifyPropertyChanged("ToolFourVisible");
            }
        }

        public bool ToolFiveVisible
        {
            get
            {
                return (Tools.Where(n => n.GetType() == typeof(ExampleDockManagerViews.ViewModel.ToolFiveViewModel)).Count() > 0);
            }
            set
            {
                ShowTool(value, typeof(ExampleDockManagerViews.ViewModel.ToolFiveViewModel));
                NotifyPropertyChanged("ToolFiveVisible");
            }
        }

        public bool IsDocumentVisible(IViewModel iViewModel)
        {
            return (Documents.Contains(iViewModel));
        }

        public void ShowDocument(bool show, IViewModel iViewModel)
        {
            bool isVisible = IsDocumentVisible(iViewModel);
            if (isVisible == show)
            {
                return;
            }

            if (show == false)
            {
                Documents.Remove(iViewModel);
            }
            else
            {
                Documents.Add(iViewModel);
            }
        }

        public bool DocumentOneVisible
        {
            get
            {
                return IsDocumentVisible(DocumentOne);
            }
            set
            {
                ShowDocument(value, DocumentOne);
                NotifyPropertyChanged("DocumentOneVisible");
            }
        }

        public bool DocumentTwoVisible
        {
            get
            {
                return IsDocumentVisible(DocumentTwo);
            }
            set
            {
                ShowDocument(value, DocumentTwo);
                NotifyPropertyChanged("DocumentTwoVisible");
            }
        }

        public bool DocumentThreeVisible
        {
            get
            {
                return IsDocumentVisible(DocumentThree);
            }
            set
            {
                ShowDocument(value, DocumentThree);
                NotifyPropertyChanged("DocumentThreeVisible");
            }
        }

        public bool DocumentFourVisible
        {
            get
            {
                return IsDocumentVisible(DocumentFour);
            }
            set
            {
                ShowDocument(value, DocumentFour);
                NotifyPropertyChanged("DocumentFourVisible");
            }
        }

        public bool DocumentFiveVisible
        {
            get
            {
                return IsDocumentVisible(DocumentFive);
            }
            set
            {
                ShowDocument(value, DocumentFive);
                NotifyPropertyChanged("DocumentFiveVisible");
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}
