using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenControls.Wpf.DockManager
{
    internal class ToolContainer : ViewContainer
    {
        public ToolContainer()
        {
            _rowDefinition_UserControl = new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) };
            _rowDefinition_Gap = new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) };
            double tabHeaderHeight = (double)FindResource("ToolPaneTabHeaderHeight");
            _rowDefinition_TabHeader = new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) };
            RowDefinitions.Add(_rowDefinition_UserControl);
            RowDefinitions.Add(_rowDefinition_Gap);
            RowDefinitions.Add(_rowDefinition_TabHeader);

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });

            CreateTabControl(2, 0);
            Grid.SetZIndex(TabHeaderControl, 1);
            TabHeaderControl.ItemContainerStyle = FindResource("ToolPaneTabItem") as Style;
            TabHeaderControl.ListBox.VerticalAlignment = VerticalAlignment.Top;
            TabHeaderControl.VerticalAlignment = VerticalAlignment.Top;

            _gap = new Border();
            _gap.SetResourceReference(Border.HeightProperty, "ToolPaneContentBarHeight");
            _gap.SetResourceReference(Border.BackgroundProperty, "ToolPaneContentBarBrush");
            Children.Add(_gap);
            Grid.SetRow(_gap, 1);
            Grid.SetColumn(_gap, 0);
            Grid.SetColumnSpan(_gap, 4);

            _border = new Border();
            Children.Add(_border);
            Grid.SetRow(_border, 2);
            Grid.SetColumn(_border, 0);
            Grid.SetColumnSpan(_border, 4);
            Grid.SetZIndex(_border, -1);
            _border.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            _border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            _border.Background = System.Windows.Media.Brushes.Transparent;

            _listButton = new Button();
            _listButton.VerticalAlignment = VerticalAlignment.Center;
            Children.Add(_listButton);
            Grid.SetRow(_listButton, 2);
            Grid.SetColumn(_listButton, 2);
            _listButton.Click += delegate { Helpers.DisplayItemsMenu(_items, TabHeaderControl, _selectedUserControl); };
            _listButton.SetResourceReference(StyleProperty, "ToolPaneListButtonStyle");

            Style style = TryFindResource("ToolPaneScrollIndicatorStyle") as Style;
            if (style != null)
            {
                TabHeaderControl.ArrowStyle = style;
            }
            TabHeaderControl.ActiveArrowBrush = FindResource("ToolPaneActiveScrollIndicatorBrush") as Brush;
            TabHeaderControl.InactiveArrowBrush = FindResource("ToolPaneInactiveScrollIndicatorBrush") as Brush;
        }

        protected override System.Windows.Forms.DialogResult UserConfirmClose(string documentTitle)
        {
            // Tools have nothing to save
            return System.Windows.Forms.DialogResult.No;
        }

        private RowDefinition _rowDefinition_UserControl;
        private RowDefinition _rowDefinition_Gap;
        private RowDefinition _rowDefinition_TabHeader;
        private Border _border;

        protected override void SetSelectedUserControlGridPosition()
        {
            Grid.SetRow(_selectedUserControl, 0);
            Grid.SetColumn(_selectedUserControl, 0);
            Grid.SetColumnSpan(_selectedUserControl, 99);
        }

        protected override void CheckTabCount()
        {
            if (_items.Count == 1)
            {
                _rowDefinition_Gap.Height = new GridLength(0);
                _rowDefinition_TabHeader.Height = new GridLength(0);
            }
            else
            {
                _rowDefinition_Gap.Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto);
                double tabHeaderHeight = (double)FindResource("ToolPaneTabHeaderHeight");
                _rowDefinition_TabHeader.Height = new System.Windows.GridLength(tabHeaderHeight, System.Windows.GridUnitType.Pixel);
            }
        }
    }
}
