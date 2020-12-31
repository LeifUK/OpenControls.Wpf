using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media;

namespace OpenControls.Wpf.TabHeaderControl
{
    public partial class TabHeaderControl : UserControl, INotifyPropertyChanged
    {
        public TabHeaderControl()
        {
            InitializeComponent();
            SetButtonStates();

            _listBox.ItemsChanged += _listBox_ItemsChanged;
            _listBox.FloatTabRequest += _listBox_FloatTabRequest;
            _listBox.PreviewMouseDown += _listBox_MouseDown;

            CloseTabCommand = new OpenControls.Wpf.Utilities.Command(
                (parameter) => _buttonCloseTab_Click(parameter, null), 
                delegate { return true; });
        }

        private void _listBox_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TabMouseDown?.Invoke(this, null);
        }

        private void _listBox_FloatTabRequest(object sender, EventArgs e)
        {
            FloatTabRequest?.Invoke(this, null);
        }

        private void _listBox_ItemsChanged(object sender, EventArgs e)
        {
            SelectedIndex = _listBox.SelectedIndex;
            ItemsChanged?.Invoke(this, null);
        }

        public event EventHandler CloseTabRequest;
        public event EventHandler SelectionChanged;
        public event EventHandler ItemsChanged;
        public event EventHandler FloatTabRequest;
        public event EventHandler TabMouseDown;

        public ListBox ListBox { get { return _listBox; } }

        public ItemCollection Items 
        {
            get { return _listBox.Items; }
        }

        #region Dependency properties

        #region IsActive dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(TabHeaderControl), null);

        public bool IsActive
        {
            get
            {
                return (bool)GetValue(IsActiveProperty);
            }
            set
            {
                if (value != IsActive)
                {
                    SetValue(IsActiveProperty, value);
                }
            }
        }

        #endregion

        #region DisplayMemberPath dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(TabHeaderControl), new FrameworkPropertyMetadata((FrameworkElement)null, new PropertyChangedCallback(OnDisplayMemberPathChanged)));

        public string DisplayMemberPath
        {
            get
            {
                return (string)GetValue(DisplayMemberPathProperty);
            }
            set
            {
                if (value != DisplayMemberPath)
                {
                    SetValue(DisplayMemberPathProperty, value);
                }
            }
        }

        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnDisplayMemberPathChanged(e);
        }

        protected virtual void OnDisplayMemberPathChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _listBox.DisplayMemberPath = (string)e.NewValue;
            }
        }

        #endregion

        #region SelectedValuePath dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedValuePathProperty = DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(TabHeaderControl), new FrameworkPropertyMetadata((FrameworkElement)null, new PropertyChangedCallback(OnSelectedValuePathChanged)));

        public string SelectedValuePath
        {
            get
            {
                return (string)GetValue(SelectedValuePathProperty);
            }
            set
            {
                if (value != SelectedValuePath)
                {
                    SetValue(SelectedValuePathProperty, value);
                }
            }
        }

        private static void OnSelectedValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnSelectedValuePathChanged(e);
        }

        protected virtual void OnSelectedValuePathChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _listBox.SelectedValuePath = (string)e.NewValue;
            }
        }

        #endregion

        #region SelectedValue dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(Object), typeof(TabHeaderControl), new FrameworkPropertyMetadata((Object)null, new PropertyChangedCallback(OnSelectedValueChanged)));

        public Object SelectedValue
        {
            get
            {
                return GetValue(SelectedValueProperty);
            }
            set
            {
                if (value != SelectedItem)
                {
                    SetValue(SelectedValueProperty, value);
                }
            }
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnSelectedValueChanged(e);
        }

        protected virtual void OnSelectedValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _listBox.SelectedValue = e.NewValue;
            }
        }

        #endregion

        #region SelectedItem dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(Object), typeof(TabHeaderControl), new FrameworkPropertyMetadata((Object)null, new PropertyChangedCallback(OnSelectedItemChanged)));

        public Object SelectedItem
        {
            get
            {
                return GetValue(SelectedItemProperty);
            }
            set
            {
                if (value != SelectedItem)
                {
                    SetValue(SelectedItemProperty, value);
                }
            }
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnSelectedItemChanged(e);
        }

        protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _listBox.SelectedItem = e.NewValue;
            }
        }

        #endregion

        #region SelectedIndex dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(TabHeaderControl), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexChanged)));

        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                if (value != SelectedIndex)
                {
                    SetValue(SelectedIndexProperty, value);
                }
            }
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnSelectedIndexChanged(e);
        }

        protected virtual void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _listBox.SelectedIndex = (int)e.NewValue;
                _listBox.ScrollIntoView(_listBox.SelectedItem);
            }
        }

        #endregion

        #region ItemsSource dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(TabHeaderControl), new FrameworkPropertyMetadata((IEnumerable)null, new PropertyChangedCallback(OnItemsSourceChanged)));

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnItemsSourceChanged(e);
        }

        private void PrepareItemsSource(IEnumerable itemsSource)
        {
            _listBox.Items.Clear();
            foreach (var item in itemsSource)
            {
                _listBox.Items.Add(item);
            }

            SelectedIndex = 0;
        }

        protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                PrepareItemsSource(e.NewValue as IEnumerable);

                if (ItemsSource is System.Collections.Specialized.INotifyCollectionChanged)
                {
                    (ItemsSource as System.Collections.Specialized.INotifyCollectionChanged).CollectionChanged += TabeHeaderControl_CollectionChanged;
                }
            }
        }

        private void TabeHeaderControl_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PrepareItemsSource(ItemsSource);
        }

        #endregion

        #region SelectedTabBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedTabBackgroundProperty = DependencyProperty.Register("SelectedTabBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Gainsboro, null));

        public Brush SelectedTabBackground
        {
            get
            {
                return (Brush)GetValue(SelectedTabBackgroundProperty);
            }
            set
            {
                if (value != SelectedTabBackground)
                {
                    SetValue(SelectedTabBackgroundProperty, value);
                }
            }
        }

        #endregion

        #region SelectedTabForeground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedTabForegroundProperty = DependencyProperty.Register("SelectedTabForeground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Black, null));

        public Brush SelectedTabForeground
        {
            get
            {
                return (Brush)GetValue(SelectedTabForegroundProperty);
            }
            set
            {
                if (value != SelectedTabForeground)
                {
                    SetValue(SelectedTabForegroundProperty, value);
                }
            }
        }

        #endregion

        #region SelectedTabBorderThickness dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedTabBorderThicknessProperty = DependencyProperty.Register("SelectedTabBorderThickness", typeof(Thickness), typeof(TabHeaderControl), new FrameworkPropertyMetadata(new Thickness(1), null));

        public Thickness SelectedTabBorderThickness
        {
            get
            {
                return (Thickness)GetValue(SelectedTabBorderThicknessProperty);
            }
            set
            {
                if (value != SelectedTabBorderThickness)
                {
                    SetValue(SelectedTabBorderThicknessProperty, value);
                }
            }
        }

        #endregion

        #region SelectedTabBorderBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedTabBorderBrushProperty = DependencyProperty.Register("SelectedTabBorderBrush", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Black, null));

        public Brush SelectedTabBorderBrush
        {
            get
            {
                return (Brush)GetValue(SelectedTabBorderBrushProperty);
            }
            set
            {
                if (value != SelectedTabBorderBrush)
                {
                    SetValue(SelectedTabBorderBrushProperty, value);
                }
            }
        }

        #endregion

        #region UnselectedTabBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty UnselectedTabBackgroundProperty = DependencyProperty.Register("UnselectedTabBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Gainsboro, null));

        public Brush UnselectedTabBackground
        {
            get
            {
                return (Brush)GetValue(UnselectedTabBackgroundProperty);
            }
            set
            {
                if (value != UnselectedTabBackground)
                {
                    SetValue(UnselectedTabBackgroundProperty, value);
                }
            }
        }

        #endregion
        
        #region UnselectedTabForeground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty UnselectedTabForegroundProperty = DependencyProperty.Register("UnselectedTabForeground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Black, null));

        public Brush UnselectedTabForeground
        {
            get
            {
                return (Brush)GetValue(UnselectedTabForegroundProperty);
            }
            set
            {
                if (value != UnselectedTabForeground)
                {
                    SetValue(UnselectedTabForegroundProperty, value);
                }
            }
        }

        #endregion

        #region UnselectedTabBorderThickness dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty UnselectedTabBorderThicknessProperty = DependencyProperty.Register("UnselectedTabBorderThickness", typeof(Thickness), typeof(TabHeaderControl), new FrameworkPropertyMetadata(new Thickness(1), null));

        public Thickness UnselectedTabBorderThickness
        {
            get
            {
                return (Thickness)GetValue(UnselectedTabBorderThicknessProperty);
            }
            set
            {
                if (value != SelectedTabBorderThickness)
                {
                    SetValue(UnselectedTabBorderThicknessProperty, value);
                }
            }
        }

        #endregion

        #region UnselectedTabBorderBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty UnselectedTabBorderBrushProperty = DependencyProperty.Register("UnselectedTabBorderBrush", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Black, null));

        public Brush UnselectedTabBorderBrush
        {
            get
            {
                return (Brush)GetValue(UnselectedTabBorderBrushProperty);
            }
            set
            {
                if (value != UnselectedTabBorderBrush)
                {
                    SetValue(UnselectedTabBorderBrushProperty, value);
                }
            }
        }

        #endregion

        #region ActiveArrowBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ActiveArrowBrushProperty = DependencyProperty.Register("ActiveArrowBrush", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.White, null));

        public Brush ActiveArrowBrush
        {
            get
            {
                return (Brush)GetValue(ActiveArrowBrushProperty);
            }
            set
            {
                if (value != ActiveArrowBrush)
                {
                    SetValue(ActiveArrowBrushProperty, value);
                }
            }
        }

        #endregion

        #region InactiveArrowBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty InactiveArrowBrushProperty = DependencyProperty.Register("InactiveArrowBrush", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Gainsboro, null));

        public Brush InactiveArrowBrush
        {
            get
            {
                return (Brush)GetValue(InactiveArrowBrushProperty);
            }
            set
            {
                if (value != InactiveArrowBrush)
                {
                    SetValue(InactiveArrowBrushProperty, value);
                }
            }
        }

        #endregion

        #region ArrowStyle dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ArrowStyleProperty = DependencyProperty.Register("ArrowStyle", typeof(Style), typeof(TabHeaderControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnArrowStyleChanged)));

        public Style ArrowStyle
        {
            get
            {
                return (Style)GetValue(ArrowStyleProperty);
            }
            set
            {
                if (value != ArrowStyle)
                {
                    SetValue(ArrowStyleProperty, value);
                }
            }
        }

        private static void OnArrowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnArrowStyleChanged(e);
        }

        protected virtual void OnArrowStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _buttonRight.Style = (Style)e.NewValue;
                _buttonLeft.Style = (Style)e.NewValue;
            }
        }

        #endregion

        #region ItemContainerStyle dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(TabHeaderControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemContainerStyleChanged)));

        public Style ItemContainerStyle
        {
            get
            {
                return (Style)GetValue(ItemContainerStyleProperty);
            }
            set
            {
                if (value != ItemContainerStyle)
                {
                    SetValue(ItemContainerStyleProperty, value);
                }
            }
        }

        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnItemContainerStyleChanged(e);
        }

        protected virtual void OnItemContainerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _listBox.ItemContainerStyle = (Style)e.NewValue;
            }
        }

        #endregion

        #endregion

        private void _listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetButtonStates();
            SelectedItem = _listBox.SelectedItem;
            SelectedIndex = _listBox.SelectedIndex;
            SelectionChanged?.Invoke(sender, null);
        }

        private void _buttonCloseTab_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if ((button == null) || (button.Parent == null))
            {
                return;
            }

            Grid grid = button.Parent as Grid;
            if (grid == null)
            {
                return;
            }

            foreach (var item in grid.Children)
            {
                if (item is Label)
                {
                    ContentPresenter contentPresenter = (item as Label).Content as ContentPresenter;
                    if (contentPresenter != null)
                    {
                        CloseTabRequest?.Invoke(contentPresenter.Content, null);
                    }
                }
            }
        }

        public OpenControls.Wpf.Utilities.Command CloseTabCommand { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged    

        private void SetButtonStates()
        {
            _buttonLeft.IsEnabled = false;
            _buttonRight.IsEnabled = false;

            if (_listBox.SelectedIndex > 0)
            {
                _buttonLeft.IsEnabled = true;
            }
            if (_listBox.SelectedIndex < (_listBox.Items.Count - 1))
            {
                _buttonRight.IsEnabled = true;
            }

            _buttonLeft.Visibility = (_listBox.Items.Count == 1) ? Visibility.Collapsed : Visibility.Visible;
            _buttonRight.Visibility = _buttonLeft.Visibility;
        }

        private void _buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            if (_listBox.SelectedIndex > 0)
            {
                --_listBox.SelectedIndex;
            }
        }

        private void _buttonRight_Click(object sender, RoutedEventArgs e)
        {
            if (_listBox.SelectedIndex < (_listBox.Items.Count - 1))
            {
                ++_listBox.SelectedIndex;
            }
        }
    }
}
