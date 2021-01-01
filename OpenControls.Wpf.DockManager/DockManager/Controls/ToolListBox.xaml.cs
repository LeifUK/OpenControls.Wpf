using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace OpenControls.Wpf.DockManager.Controls
{
    /// <summary>
    /// Interaction logic for ToolListControl.xaml
    /// </summary>
    public partial class ToolListBox : UserControl, IToolListBox
    {
        public ToolListBox()
        {
            InitializeComponent();
        }

        internal event Events.ItemClickEventHandler ItemClick;

        public WindowLocation WindowLocation { get; set; }

        #region Dependency properties

        #region ItemContainerStyle dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(ToolListBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemContainerStyleChanged)));

        public Style ItemContainerStyle
        {
            get
            {
                return (Style)GetValue(ItemContainerStyleProperty);
            }
            set
            {
                if ((value != ItemContainerStyle) && (value != null))
                {
                    SetValue(ItemContainerStyleProperty, value);
                    _listBox.ItemContainerStyle = value;
                }
            }
        }

        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToolListBox)d).OnItemContainerStyleChanged(e);
        }

        protected virtual void OnItemContainerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Style)e.NewValue != ItemContainerStyle)
            {
                _listBox.ItemContainerStyle = e.NewValue as Style;
            }
        }

        #endregion

        #region ItemsSource dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(System.Collections.ObjectModel.ObservableCollection<IToolListBoxItem>), typeof(ToolListBox), new FrameworkPropertyMetadata((System.Collections.ObjectModel.ObservableCollection<IToolListBoxItem>)null, new PropertyChangedCallback(OnItemsSourceChanged)));

        public System.Collections.ObjectModel.ObservableCollection<IToolListBoxItem> ItemsSource
        {
            get
            {
                return (System.Collections.ObjectModel.ObservableCollection<IToolListBoxItem>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToolListBox)d).OnItemsSourceChanged(e);
        }

        private void PrepareItemsSource(System.Collections.ObjectModel.ObservableCollection<IToolListBoxItem> itemsSource)
        {
            _listBox.Items.Clear();
            foreach (var item in itemsSource)
            {
                _listBox.Items.Add(item);
            }
            if (_listBox.Items.Count > 0)
            {
                _listBox.SelectedIndex = 0;
            }
        }

        protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                PrepareItemsSource(e.NewValue as System.Collections.ObjectModel.ObservableCollection<IToolListBoxItem>);

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

        #region SelectedIndex dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(ToolListBox), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexChanged)));

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
            ((ToolListBox)d).OnSelectedIndexChanged(e);
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

        #region DisplayMemberPath dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(ToolListBox), new FrameworkPropertyMetadata((FrameworkElement)null, new PropertyChangedCallback(OnDisplayMemberPathChanged)));

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
            ((ToolListBox)d).OnDisplayMemberPathChanged(e);
        }

        protected virtual void OnDisplayMemberPathChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _listBox.DisplayMemberPath = (string)e.NewValue;
            }
        }

        #endregion

        #region IsHorizontal dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty IsHorizontalProperty = DependencyProperty.Register("IsHorizontal", typeof(bool), typeof(ToolListBox), new FrameworkPropertyMetadata(true, OnIsHorizontalChanged));

        public bool IsHorizontal
        {
            get
            {
                return (bool)GetValue(IsHorizontalProperty);
            }
            set
            {
                if (value != IsHorizontal)
                {
                    SetValue(IsHorizontalProperty, value);
                }
            }
        }

        private static void OnIsHorizontalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToolListBox)d).OnIsHorizontalChanged(e);
        }

        protected virtual void OnIsHorizontalChanged(DependencyPropertyChangedEventArgs e)
        {
            _rotation.Angle = (bool)e.NewValue ? 0.0 :90.0;
        }

        #endregion

        #region TextAngle dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty TextAngleProperty = DependencyProperty.Register("TextAngle", typeof(double), typeof(ToolListBox), new FrameworkPropertyMetadata(0.0, null));

        public double TextAngle
        {
            get
            {
                return (double)GetValue(TextAngleProperty);
            }
            set
            {
                if (value != TextAngle)
                {
                    SetValue(TextAngleProperty, value);
                }
            }
        }

        #endregion

        #endregion Dependency properties

        private void _listBox_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_listBox.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                return;
            }

            // SelectedIndex is yet to be set so we must find it outselves ... 
            Point cursorScreenPosition = OpenControls.Wpf.DockManager.Controls.Utilities.GetCursorPosition();

            for (int index = 0; index < _listBox.Items.Count; ++index)
            {
                ListBoxItem item = _listBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;

                Point cursorItemPosition = item.PointFromScreen(cursorScreenPosition);
                if ((cursorItemPosition.X >= 0) && (cursorItemPosition.Y >= 0) && (cursorItemPosition.X <= item.ActualWidth) && (cursorItemPosition.Y <= item.ActualHeight))
                {
                    ItemClick?.Invoke(item.DataContext, new Events.ItemClickEventArgs() { ToolListBox = this });
                    return;
                }
            }
        }
    }
}
