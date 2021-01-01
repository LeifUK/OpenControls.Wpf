using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Xml;
using System.Windows.Input;
using System.Collections.ObjectModel;
using OpenControls.Wpf.DockManager.Controls;

namespace OpenControls.Wpf.DockManager
{
    public class LayoutManager : System.Windows.Controls.Grid, ILayoutFactory, IDockPaneHost, IUnpinnedToolHost, IFloatingPaneHost
    {
        public LayoutManager() 
        {
            Tag = new Guid("3c81a424-ef66-4de7-a361-9968cd88071c");

            IDockPaneManager = new DockPaneManager(this, this);
            IUnpinnedToolManager = new UnpinnedToolManager(this);
            IFloatingPaneManager = new FloatingPaneManager(this, this);
        }

        public void Initialise()
        {
            System.Diagnostics.Trace.Assert(Theme != null);

            UseLayoutRounding = true;

            ResourceDictionary dictionary = new ResourceDictionary() { Source = Theme.Uri };
            Application.Current.Resources.MergedDictionaries.Add(dictionary);

            //FloatingToolPaneGroups = new List<IFloatingPane>();
            //FloatingDocumentPaneGroups = new List<IFloatingPane>();

            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            CreateToolListBoxes();

            Application.Current.MainWindow.LocationChanged += MainWindow_LocationChanged;
            PreviewMouseDown += LayoutManager_PreviewMouseDown;
            SizeChanged += LayoutManager_SizeChanged;
        }

        private void LayoutManager_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_root == null)
            {
                return;
            }

            Point cursorPoint = Utilities.GetCursorPosition();
            Point topLeftPoint = _root.PointToScreen(new Point(0, 0));
            if (
                    (cursorPoint.X > topLeftPoint.X) &&
                    (cursorPoint.Y > topLeftPoint.Y) &&
                    (cursorPoint.X < (topLeftPoint.X + _root.ActualWidth)) &&
                    (cursorPoint.Y < (topLeftPoint.Y + _root.ActualHeight))
                )
            {
                IUnpinnedToolManager.CloseUnpinnedToolPane();
            }
        }

        ~LayoutManager()
        {
            Shutdown();
        }

        private ILayoutFactory ILayoutFactory
        {
            get
            {
                return this;
            }
        }
        private IDockPaneHost IDockPaneHost
        {
            get
            {
                return this;
            }
        }
        private readonly IDockPaneManager IDockPaneManager;
        private readonly IUnpinnedToolManager IUnpinnedToolManager;
        private readonly IFloatingPaneManager IFloatingPaneManager;

        public void Shutdown()
        {
            IFloatingPaneManager.Shutdown();
        }

        public DataTemplate ToolPaneTitleTemplate { get; set; }
        public DataTemplate ToolPaneHeaderTemplate { get; set; }
        public DataTemplate DocumentPaneTitleTemplate { get; set; }
        public DataTemplate DocumentPaneHeaderTemplate { get; set; }

        private Dictionary<WindowLocation, Controls.ToolListBox> _dictToolListBoxes;

        private Grid _root;

        private Dictionary<IViewModel, List<string>> CreateViewModelUrlDictionary(ObservableCollection<IViewModel> viewModels)
        {
            // The string list contains each URL for the given view model
            Dictionary<IViewModel, List<string>> mapViewModels = new Dictionary<IViewModel, List<string>>();

            foreach (var iViewModel in viewModels)
            {
                if (!mapViewModels.ContainsKey(iViewModel))
                {
                    mapViewModels.Add(iViewModel, new List<string>());
                }
                mapViewModels[iViewModel].Add(iViewModel.URL);
            }

            return mapViewModels;
        }

        private void ValidateToolPanes()
        {
            Dictionary<IViewModel, List<string>> viewModelUrlDictionary = CreateViewModelUrlDictionary(ToolsSource);

            IUnpinnedToolManager.Validate(viewModelUrlDictionary);
            IFloatingPaneManager.ValidateFloatingToolPanes(viewModelUrlDictionary);
            IDockPaneManager.ValidateDockPanes(_root, viewModelUrlDictionary, typeof(ToolPaneGroup));
        }

        private void ValidateDocumentPanes()
        {
            Dictionary<IViewModel, List<string>> viewModelUrlDictionary = CreateViewModelUrlDictionary(DocumentsSource);
            IFloatingPaneManager.ValidateFloatingDocumentPanes(viewModelUrlDictionary);
            IDockPaneManager.ValidateDockPanes(_root, viewModelUrlDictionary, typeof(DocumentPaneGroup));
        }

        #region dependency properties 

        #region Theme

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register("Theme", typeof(Themes.Theme), typeof(LayoutManager), new FrameworkPropertyMetadata(null, null));

        public Themes.Theme Theme
        {
            get
            {
                return (Themes.Theme)GetValue(ThemeProperty);
            }
            set
            {
                if (value != Theme)
                {
                    SetValue(ThemeProperty, value);
                }
            }
        }

        #endregion

        #region DocumentsSource dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DocumentsSourceProperty = DependencyProperty.Register("DocumentsSource", typeof(ObservableCollection<IViewModel>), typeof(LayoutManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDocumentsSourceChanged)));

        private void LayoutManager_DocumentsSourceCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ValidateDocumentPanes();
        }

        public ObservableCollection<IViewModel> DocumentsSource
        {
            get
            {
                return (ObservableCollection<IViewModel>)GetValue(DocumentsSourceProperty);
            }
            set
            {
                SetValue(DocumentsSourceProperty, value);
                (value as System.Collections.Specialized.INotifyCollectionChanged).CollectionChanged += LayoutManager_DocumentsSourceCollectionChanged;
            }
        }

        private static void OnDocumentsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnDocumentsSourceChanged(e);
        }

        protected virtual void OnDocumentsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DocumentsSource = (ObservableCollection<IViewModel>)e.NewValue;
            }
        }

        #endregion

        #region ToolsSource dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ToolsSourceProperty = DependencyProperty.Register("ToolsSource", typeof(ObservableCollection<IViewModel>), typeof(LayoutManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnToolsSourceChanged)));

        private void LayoutManager_ToolsSourceCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ValidateToolPanes();
        }

        public ObservableCollection<IViewModel> ToolsSource
        {
            get
            {
                return (ObservableCollection<IViewModel>)GetValue(ToolsSourceProperty);
            }
            set
            {
                SetValue(ToolsSourceProperty, value);
                (value as System.Collections.Specialized.INotifyCollectionChanged).CollectionChanged += LayoutManager_ToolsSourceCollectionChanged;
            }
        }

        private static void OnToolsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnToolsSourceChanged(e);
        }

        protected virtual void OnToolsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ToolsSource = e.NewValue as ObservableCollection<IViewModel>;
            }
        }

        #endregion

        #region DocumentTemplates dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DocumentTemplatesProperty = DependencyProperty.Register("DocumentTemplates", typeof(List<DataTemplate>), typeof(LayoutManager), new FrameworkPropertyMetadata(new List<DataTemplate>(), new PropertyChangedCallback(OnDocumentTemplatesChanged)));

        public List<DataTemplate> DocumentTemplates
        {
            get
            {
                return (List<DataTemplate>)GetValue(DocumentTemplatesProperty);
            }
            set
            {
                SetValue(DocumentTemplatesProperty, value);
            }
        }

        private static void OnDocumentTemplatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnDocumentTemplatesChanged(e);
        }

        protected virtual void OnDocumentTemplatesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DocumentTemplates = (List<DataTemplate>)e.NewValue;
            }
        }

        #endregion

        #region ToolTemplates dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ToolTemplatesProperty = DependencyProperty.Register("ToolTemplates", typeof(System.Collections.Generic.List<DataTemplate>), typeof(LayoutManager), new FrameworkPropertyMetadata(new List<DataTemplate>(), new PropertyChangedCallback(OnToolTemplatesChanged)));

        public System.Collections.Generic.List<DataTemplate> ToolTemplates
        {
            get
            {
                return (System.Collections.Generic.List<DataTemplate>)GetValue(ToolTemplatesProperty);
            }
            set
            {
                SetValue(ToolTemplatesProperty, value);
            }
        }

        private static void OnToolTemplatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnToolTemplatesChanged(e);
        }

        protected virtual void OnToolTemplatesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ToolTemplates = (System.Collections.Generic.List<DataTemplate>)e.NewValue;
            }
        }

        #endregion

        #endregion

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            IUnpinnedToolManager.ProcessMoveResize();
        }

        private void LayoutManager_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            IUnpinnedToolManager.ProcessMoveResize();
        }

        private void ToolListBox_ItemClick(object sender, Events.ItemClickEventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is ToolListBoxItem);
            System.Diagnostics.Trace.Assert((e != null) && (e.ToolListBox != null));

            IUnpinnedToolManager.ShowUnpinnedToolPane(sender as ToolListBoxItem, e.ToolListBox);
        }

        private void ToolPane_UnPinClick(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is ToolPaneGroup);

            IUnpinnedToolManager.Unpin(sender as ToolPaneGroup);
        }

        private void CreateToolListBox(int row, int column, bool isHorizontal, WindowLocation windowLocation)
        {
            ObservableCollection<Controls.IToolListBoxItem> items = new ObservableCollection<Controls.IToolListBoxItem>();
            Controls.ToolListBox toolListBox = new Controls.ToolListBox();
            toolListBox.TextAngle = ((windowLocation == WindowLocation.TopSide) || (windowLocation == WindowLocation.RightSide)) ? 180.0 : 0.0;
            toolListBox.WindowLocation = windowLocation;
            toolListBox.ItemsSource = items;
            toolListBox.IsHorizontal = isHorizontal;
            toolListBox.DisplayMemberPath = "Title";
            toolListBox.ItemContainerStyle = FindResource("SideToolItemStyle") as Style;
            toolListBox.ItemClick += ToolListBox_ItemClick;
            Children.Add(toolListBox);
            Grid.SetRow(toolListBox, row);
            Grid.SetColumn(toolListBox, column);

            _dictToolListBoxes.Add(windowLocation, toolListBox);
        }

        private void CreateToolListBoxes()
        {
            _dictToolListBoxes = new Dictionary<WindowLocation, Controls.ToolListBox>();
            CreateToolListBox(1, 0, false, WindowLocation.LeftSide);
            CreateToolListBox(1, 2, false, WindowLocation.RightSide);
            CreateToolListBox(0, 1, true, WindowLocation.TopSide);
            CreateToolListBox(2, 1, true, WindowLocation.BottomSide);
        }

        public void Clear()
        {
            Children.Clear();
            IUnpinnedToolManager.Clear();
            CreateToolListBoxes();
            IFloatingPaneManager.Clear();
        }

        public List<UserControl> LoadViewsFromTemplates(List<DataTemplate> dataTemplates, ObservableCollection<IViewModel> viewModels)
        {
            List<UserControl> views = new List<UserControl>();

            if ((dataTemplates == null) || (dataTemplates.Count == 0) || (viewModels == null))
            {
                return views;
            }

            // First load the views and view models

            foreach (var viewModel in viewModels)
            {
                foreach (var item in dataTemplates)
                {
                    if (item is DataTemplate)
                    {
                        DataTemplate dataTemplate = item as DataTemplate;

                        if (viewModel.GetType() == (Type)dataTemplate.DataType)
                        {
                            UserControl view = (dataTemplate.LoadContent() as UserControl);
                            if (view != null)
                            {
                                view.DataContext = viewModel;
                                view.HorizontalAlignment = HorizontalAlignment.Stretch;
                                view.VerticalAlignment = VerticalAlignment.Stretch;

                                views.Add(view);
                            }
                        }
                    }
                }
            }

            return views;
        }

        #region IFloatingPaneHost

        Grid IFloatingPaneHost.RootPane 
        { 
            get
            {
                return IDockPaneHost.RootPane;
            }
        }

        Grid IFloatingPaneHost.RootGrid
        {
            get
            {
                return this;
            }
        }

        void IFloatingPaneHost.RemoveViewModel(IViewModel iViewModel)
        {
            if (DocumentsSource.Contains(iViewModel))
            {
                DocumentsSource.Remove(iViewModel);
            }
            else if (ToolsSource.Contains(iViewModel))
            {
                ToolsSource.Remove(iViewModel);
            }
        }
        
        ISelectablePane IFloatingPaneHost.FindSelectablePane(Point pointOnScreen)
        {
            return IDockPaneManager.FindSelectablePane(this, pointOnScreen);
        }
        
        void IFloatingPaneHost.Unfloat(FloatingPane floatingPane, SelectablePane selectedPane, WindowLocation windowLocation)
        {
            IDockPaneManager.Unfloat(floatingPane, selectedPane, windowLocation);
        }

        #endregion IFloatingPaneHost

        #region ILayoutFactory

        DocumentPanel ILayoutFactory.MakeDocumentPanel()
        {
            return new DocumentPanel();
        }

        SplitterPane ILayoutFactory.MakeSplitterPane(bool isHorizontal)
        {
            return new SplitterPane(isHorizontal);
        }

        DocumentPaneGroup ILayoutFactory.MakeDocumentPaneGroup()
        {
            DocumentPaneGroup documentPaneGroup = new DocumentPaneGroup();
            IDockPaneManager.RegisterDockPane(documentPaneGroup);
            return documentPaneGroup;
        }

        ToolPaneGroup ILayoutFactory.MakeToolPaneGroup()
        {
            ToolPaneGroup toolPaneGroup = new ToolPaneGroup();
            IDockPaneManager.RegisterDockPane(toolPaneGroup);
            toolPaneGroup.UnPinClick += ToolPane_UnPinClick;
            return toolPaneGroup;
        }

        FloatingDocumentPaneGroup ILayoutFactory.MakeFloatingDocumentPaneGroup()
        {
            FloatingDocumentPaneGroup floatingDocumentPaneGroup = new FloatingDocumentPaneGroup();
            IFloatingPaneManager.Show(floatingDocumentPaneGroup);
            return floatingDocumentPaneGroup;
        }

        FloatingToolPaneGroup ILayoutFactory.MakeFloatingToolPaneGroup()
        {
            FloatingToolPaneGroup floatingToolPaneGroup = new FloatingToolPaneGroup();
            IFloatingPaneManager.Show(floatingToolPaneGroup);
            return floatingToolPaneGroup;
        }

        void ILayoutFactory.MakeUnpinnedToolPaneGroup(WindowLocation windowLocation, ToolPaneGroup toolPaneGroup, string siblingGuid, bool isHorizontal, bool isFirst)
        {
            IUnpinnedToolManager.MakeUnpinnedToolPaneGroup(windowLocation, toolPaneGroup, siblingGuid, isHorizontal, isFirst);
        }

        string ILayoutFactory.MakeDocumentKey(string contentId, string Url)
        {
            return contentId + "," + Url;
        }

        void ILayoutFactory.SetRootPane(Grid grid, out int row, out int column)
        {
            IDockPaneHost.RootPane = grid;
            row = Grid.GetRow(grid);
            column = Grid.GetColumn(grid);
        }

        #endregion ILayoutFactory

        #region IDockPaneHost

        void IDockPaneHost.FrameworkElementRemoved(FrameworkElement frameworkElement)
        {
            IUnpinnedToolManager.FrameworkElementRemoved(frameworkElement);
        }

        void IDockPaneHost.RemoveViewModel(IViewModel iViewModel)
        {
            (this as IFloatingPaneHost).RemoveViewModel(iViewModel);
        }

        Grid IDockPaneHost.RootPane
        {
            get
            {
                return _root;
            }
            set
            {

                if ((_root != null) && Children.Contains(_root))
                {
                    Children.Remove(_root);
                }
                _root = value;
                Children.Add(_root);
                Grid.SetRow(_root, 1);
                Grid.SetColumn(_root, 1);
            }
        }

        Grid IDockPaneHost.RootGrid
        {
            get
            {
                return this;
            }
        }

        List<UserControl> IDockPaneHost.LoadToolViews(ObservableCollection<IViewModel> viewModels)
        {
            return LoadViewsFromTemplates(ToolTemplates, viewModels);
        }

        List<UserControl> IDockPaneHost.LoadDocumentViews(ObservableCollection<IViewModel> viewModels)
        {
            return LoadViewsFromTemplates(DocumentTemplates, viewModels);
        }

        #endregion IDockPaneTree

        #region IUnpinnedToolHost

        Grid IUnpinnedToolHost.RootPane
        {
            get
            {
                return _root;
            }
        }

        void IUnpinnedToolHost.ViewModelRemoved(IViewModel iViewModel)
        {
            System.Diagnostics.Trace.Assert(ToolsSource.Contains(iViewModel));

            ToolsSource.Remove(iViewModel);
        }

        IToolListBox IUnpinnedToolHost.GetToolListBox(WindowLocation windowLocation)
        {
            System.Diagnostics.Trace.Assert(_dictToolListBoxes.ContainsKey(windowLocation));

            return _dictToolListBoxes[windowLocation];
        }

        void IUnpinnedToolHost.PinToolPane(UnpinnedToolData unpinnedToolData, WindowLocation defaultWindowLocation)
        {
            IDockPaneManager.PinToolPane(unpinnedToolData, defaultWindowLocation);
        }

        void IUnpinnedToolHost.UnpinToolPane(ToolPaneGroup toolPaneGroup, out UnpinnedToolData unpinnedToolData, out WindowLocation toolListBoxLocation)
        {
            IDockPaneManager.UnpinToolPane(toolPaneGroup, out unpinnedToolData, out toolListBoxLocation);
        }

        #endregion IUnpinnedToolPaneOwner

        public bool SaveLayoutToFile(string fileNameAndPath)
        {
            XmlDocument xmlDocument = new XmlDocument();

            if (Children.Count == 0)
            {
                return false;
            }

            Serialisation.LayoutWriter.SaveLayout(
                xmlDocument,
                _root,
                IFloatingPaneManager.FloatingToolPaneGroups,
                IFloatingPaneManager.FloatingDocumentPaneGroups,
                IUnpinnedToolManager.GetUnpinnedToolData());

            xmlDocument.Save(fileNameAndPath);

            return true;
        }

        private void LoadDefaultLayout()
        {
            List<UserControl> documentViews = LoadViewsFromTemplates(DocumentTemplates, DocumentsSource);
            List<UserControl> toolViews = LoadViewsFromTemplates(ToolTemplates, ToolsSource);

            IDockPaneManager.CreateDefaultLayout(documentViews, toolViews);
            UpdateLayout();
            IFloatingPaneManager.CancelSelection();
        }

        private bool Load(XmlDocument xmlDocument)
        {
            if (xmlDocument.ChildNodes.Count == 0)
            {
                return false;
            }

            List<UserControl> documentViews = LoadViewsFromTemplates(DocumentTemplates, DocumentsSource);
            List<UserControl> toolViews = LoadViewsFromTemplates(ToolTemplates, ToolsSource);

            List<UserControl> views = new List<UserControl>();
            Dictionary<string, UserControl> viewsMap = new Dictionary<string, UserControl>();

            foreach (var item in documentViews)
            {
                viewsMap.Add(ILayoutFactory.MakeDocumentKey(item.Name, ((item.DataContext) as IViewModel).URL), item);
            }

            foreach (var item in toolViews)
            {
                viewsMap.Add(item.Name, item);
            }

            // Now load the views into the dock manager => one or more views might not be visible!

            Serialisation.LayoutReader.LoadNode(this, viewsMap, this, this, xmlDocument.DocumentElement, true);

            // Remove any view without a view model 

            ValidateToolPanes();
            ValidateDocumentPanes();
            IFloatingPaneManager.CancelSelection();

            return true;
        }

        public bool LoadLayout(string layout)
        {
            if (string.IsNullOrEmpty(layout))
            {
                LoadDefaultLayout();
                return true;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(layout);
            return Load(xmlDocument);
        }

        public bool LoadLayoutFromFile(string fileNameAndPath)
        {
            if (string.IsNullOrEmpty(fileNameAndPath))
            {
                LoadDefaultLayout();
                return true;
            }

            Clear();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileNameAndPath);
            return Load(xmlDocument);
        }
    }
}
