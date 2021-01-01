using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal class DocumentPaneGroup : DockPane, IActiveDocument
    {
        public DocumentPaneGroup() : base(new DocumentContainer())
        {
            Border.SetResourceReference(Border.CornerRadiusProperty, "DocumentPaneCornerRadius");
            Border.SetResourceReference(Border.BorderBrushProperty, "DocumentPaneBorderBrush");
            Border.SetResourceReference(Border.BorderThicknessProperty, "DocumentPaneBorderThickness");

            (IViewContainer as DocumentContainer).DisplayGeneralMenu = DisplayGeneralMenu;

            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(Border.BorderThickness.Left, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(Border.BorderThickness.Right, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[0].Height = new GridLength(Border.BorderThickness.Top, GridUnitType.Pixel);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[2].Height = new GridLength(Border.BorderThickness.Bottom, GridUnitType.Pixel);

            IViewContainer.SelectionChanged += DocumentContainer_SelectionChanged;
            Grid.SetRow(IViewContainer as System.Windows.UIElement, 1);
            Grid.SetColumn(IViewContainer as System.Windows.UIElement, 1);
            Grid.SetColumnSpan(IViewContainer as System.Windows.UIElement, ColumnDefinitions.Count - 2);

            IsHighlighted = false;
            IsActive = false;
        }

        private bool _isHighlighted;
        public override bool IsHighlighted
        {
            get
            {
                return _isHighlighted;
            }
            set
            {
                _isHighlighted = value;
                if (value)
                {
                    Border.SetResourceReference(Border.BackgroundProperty, "SelectedPaneBrush");
                }
                else
                {
                    Border.SetResourceReference(Border.BackgroundProperty, "DocumentPaneBackground");
                }
            }
        }

        public bool IsActive
        {
            get
            {
                return IViewContainer.IsActive;
            }
            set
            {
                IViewContainer.IsActive = value;
            }
        }

        private void DocumentContainer_SelectionChanged(object sender, EventArgs e)
        {
            // Nothing to do!
        }
    }
}
