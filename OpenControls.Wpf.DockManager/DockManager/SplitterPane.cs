using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal class SplitterPane : Grid
    {
        public SplitterPane(bool isHorizontal)
        {
            Tag = Guid.NewGuid();
            IsHorizontal = isHorizontal;

            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Margin = new Thickness(0);

            _gridSplitter = new GridSplitter();
            _gridSplitter.SetResourceReference(BackgroundProperty, "SplitterBrush");
            _gridSplitter.BorderThickness = new Thickness(0);
            Children.Add(_gridSplitter);

            RowDefinitions.Add(new RowDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());

            if (IsHorizontal)
            {
                RowDefinition rowDefinition = new RowDefinition();
                RowDefinitions.Add(rowDefinition);
                rowDefinition.Height = new GridLength(1, GridUnitType.Auto);

                RowDefinitions.Add(new RowDefinition());

                ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);

                _gridSplitter.VerticalAlignment = VerticalAlignment.Center;
                _gridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                _gridSplitter.SetResourceReference(HeightProperty, "SplitterWidth");
                Grid.SetRow(_gridSplitter, 1);
                Grid.SetColumn(_gridSplitter, 0);
            }
            else
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
                ColumnDefinitions.Add(columnDefinition);

                ColumnDefinitions.Add(new ColumnDefinition());

                RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);

                _gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                _gridSplitter.HorizontalAlignment = HorizontalAlignment.Center;
                _gridSplitter.SetResourceReference(WidthProperty, "SplitterWidth");
                Grid.SetRow(_gridSplitter, 0);
                Grid.SetColumn(_gridSplitter, 1);
            }
        }

        private readonly GridSplitter _gridSplitter;

        public readonly bool IsHorizontal;

        public void AddChild(FrameworkElement frameworkElement, bool isFirst)
        {
            Children.Add(frameworkElement);
            int row = 0;
            int column = 0;
            if (!isFirst)
            {
                if (IsHorizontal)
                {
                    row = 2;
                }
                else
                {
                    column = 2;
                }
            }
            Grid.SetRow(frameworkElement, row);
            Grid.SetColumn(frameworkElement, column);
        }

        public double SplitterWidth
        {
            set
            {
                System.Diagnostics.Trace.Assert(_gridSplitter != null);

                if (IsHorizontal)
                {
                    _gridSplitter.Height = value;
                }
                else
                {
                    _gridSplitter.Width = value;
                }
            }
        }

        public Brush SplitterBrush
        {
            set
            {
                System.Diagnostics.Trace.Assert(_gridSplitter != null);

                _gridSplitter.Background = value;
            }
        }
    }
}
