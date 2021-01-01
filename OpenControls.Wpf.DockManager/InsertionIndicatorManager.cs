using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace OpenControls.Wpf.DockManager
{
    /*
     * The insertion indicator show the user where in a pane the dragged document will be placed
     */
    class InsertionIndicatorManager
    {
        public InsertionIndicatorManager(Grid parentGrid)
        {
            ParentGrid = parentGrid;
        }

        public readonly Grid ParentGrid;
        private Grid _insertionIndicator;
        
        public WindowLocation WindowLocation;

        public void HideInsertionIndicator()
        {
            if (_insertionIndicator != null)
            {
                if (ParentGrid.Children.Contains(_insertionIndicator))
                {
                    ParentGrid.Children.Remove(_insertionIndicator);
                }
                _insertionIndicator = null;
                WindowLocation = WindowLocation.None;
            }
        }
        
        public static Grid CreateInsertionIndicator(WindowLocation windowLocation)
        {
            Grid insertionIndicator = new Grid();

            insertionIndicator.ColumnDefinitions.Add(new ColumnDefinition());
            insertionIndicator.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            insertionIndicator.ColumnDefinitions.Add(new ColumnDefinition());
            insertionIndicator.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

            insertionIndicator.RowDefinitions.Add(new RowDefinition());
            insertionIndicator.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
            insertionIndicator.RowDefinitions.Add(new RowDefinition());
            insertionIndicator.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);

            Border border = new Border();
            border.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(160, System.Windows.Media.Colors.Gainsboro.R, System.Windows.Media.Colors.Gainsboro.G, System.Windows.Media.Colors.Gainsboro.B));
            border.VerticalAlignment = VerticalAlignment.Stretch;
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            insertionIndicator.Children.Add(border);

            if (windowLocation == WindowLocation.Middle)
            {
                Grid.SetRow(border, 0);
                Grid.SetRowSpan(border, 2);
                Grid.SetColumn(border, 0);
                Grid.SetColumnSpan(border, 2);
            }
            else
            {
                switch (windowLocation)
                {
                    case WindowLocation.Left:
                    case WindowLocation.LeftSide:
                        Grid.SetRow(border, 0);
                        Grid.SetRowSpan(border, 2);
                        Grid.SetColumn(border, 0);
                        break;
                    case WindowLocation.Right:
                    case WindowLocation.RightSide:
                        Grid.SetRow(border, 0);
                        Grid.SetRowSpan(border, 2);
                        Grid.SetColumn(border, 1);
                        break;
                    case WindowLocation.Top:
                    case WindowLocation.TopSide:
                        Grid.SetRow(border, 0);
                        Grid.SetColumn(border, 0);
                        Grid.SetColumnSpan(border, 2);
                        break;
                    case WindowLocation.Bottom:
                    case WindowLocation.BottomSide:
                        Grid.SetRow(border, 1);
                        Grid.SetColumn(border, 0);
                        Grid.SetColumnSpan(border, 2);
                        break;
                    default:
                        return null;
                }
            }

            return insertionIndicator;
        }

        public bool ShowInsertionIndicator(WindowLocation windowLocation)
        {
            if (windowLocation == WindowLocation.None)
            {
                HideInsertionIndicator();
                return true;
            }

            if (windowLocation == WindowLocation)
            {
                return true;
            }

            WindowLocation = windowLocation;

            if (_insertionIndicator != null)
            {
                ParentGrid.Children.Remove(_insertionIndicator);
                _insertionIndicator = null;
            }

            _insertionIndicator = CreateInsertionIndicator(windowLocation);

            if (_insertionIndicator != null)
            {
                Grid.SetRow(_insertionIndicator, 0);
                if (ParentGrid.RowDefinitions.Count() > 0)
                {
                    Grid.SetRowSpan(_insertionIndicator, ParentGrid.RowDefinitions.Count());
                }
                Grid.SetColumn(_insertionIndicator, 0);
                if (ParentGrid.ColumnDefinitions.Count() > 0)
                {
                    Grid.SetColumnSpan(_insertionIndicator, ParentGrid.ColumnDefinitions.Count());
                }
                ParentGrid.Children.Add(_insertionIndicator);
            }

            return false;
        }
    }
}
