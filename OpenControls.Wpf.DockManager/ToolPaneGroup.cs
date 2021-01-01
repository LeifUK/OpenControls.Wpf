using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenControls.Wpf.DockManager
{
    internal class ToolPaneGroup : DockPane
    {
        public ToolPaneGroup() : base(new ToolContainer())
        {
            Border.SetResourceReference(Border.BackgroundProperty, "ToolPaneBackground");
            Border.SetResourceReference(Border.CornerRadiusProperty, "ToolPaneCornerRadius");
            Border.SetResourceReference(Border.BorderBrushProperty, "ToolPaneBorderBrush");
            Border.SetResourceReference(Border.BorderThicknessProperty, "ToolPaneBorderThickness");

            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(Border.BorderThickness.Left, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinitions.Add(columnDefinition);
            
            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(Border.BorderThickness.Right, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[0].Height = new GridLength(Border.BorderThickness.Top, GridUnitType.Pixel);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[3].Height = new GridLength(Border.BorderThickness.Bottom, GridUnitType.Pixel);

            HeaderBorder = new Border();
            HeaderBorder.VerticalAlignment = VerticalAlignment.Stretch;
            HeaderBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            HeaderBorder.SetResourceReference(Border.CornerRadiusProperty, "ToolPaneHeaderCornerRadius");
            HeaderBorder.SetResourceReference(Border.BorderBrushProperty, "ToolPaneHeaderBorderBrush");
            HeaderBorder.SetResourceReference(Border.BorderThicknessProperty, "ToolPaneHeaderBorderThickness");
            IsHighlighted = false;

            Grid.SetRow(HeaderBorder, 1);
            Grid.SetColumn(HeaderBorder, 1);
            Grid.SetColumnSpan(HeaderBorder, ColumnDefinitions.Count - 2);
            Children.Add(HeaderBorder);

            _titleLabel = new Label();
            _titleLabel.FontSize = 12;
            _titleLabel.Padding = new Thickness(4, 0, 0, 0);
            _titleLabel.VerticalAlignment = VerticalAlignment.Center;
            _titleLabel.Background = System.Windows.Media.Brushes.Transparent;
            _titleLabel.Foreground = System.Windows.Media.Brushes.White;
            Grid.SetRow(_titleLabel, 1);
            Grid.SetColumn(_titleLabel, 1);
            Children.Add(_titleLabel);

            _commandsButton = new Button();
            _commandsButton.VerticalAlignment = VerticalAlignment.Center;
            _commandsButton.SetResourceReference(StyleProperty, "ToolPaneCommandsButtonStyle");
            _commandsButton.Click += delegate { DisplayGeneralMenu(); };
            Grid.SetRow(_commandsButton, 1);
            Grid.SetColumn(_commandsButton, 4);
            Children.Add(_commandsButton);

            _pinButton = new Button();
            _pinButton.VerticalAlignment = VerticalAlignment.Center;
            _pinButton.LayoutTransform = new System.Windows.Media.RotateTransform();
            _pinButton.SetResourceReference(StyleProperty, "ToolPanePinButtonStyle");
            _pinButton.Click += PinButton_Click;
            Grid.SetRow(_pinButton, 1);
            Grid.SetColumn(_pinButton, 6);
            Children.Add(_pinButton);

            _closeButton = new Button();
            _closeButton.VerticalAlignment = VerticalAlignment.Center;
            _closeButton.SetResourceReference(StyleProperty, "ToolPaneCloseButtonStyle");
            Grid.SetRow(_closeButton, 1);
            Grid.SetColumn(_closeButton, 8);
            Panel.SetZIndex(_closeButton, 99);
            Children.Add(_closeButton);
            _closeButton.Click += delegate { FireCloseRequest(); };

            IViewContainer.SelectionChanged += DocumentContainer_SelectionChanged;
            Grid.SetRow(IViewContainer as System.Windows.UIElement, 2);
            Grid.SetColumn(IViewContainer as System.Windows.UIElement, 1);
            Grid.SetColumnSpan(IViewContainer as System.Windows.UIElement, ColumnDefinitions.Count - 2);

            _titleLabel.SetResourceReference(Label.FontSizeProperty, "ToolPaneHeaderFontSize");
            _titleLabel.SetResourceReference(Label.FontFamilyProperty, "ToolPaneHeaderFontFamily");
            _titleLabel.SetResourceReference(Label.PaddingProperty, "ToolPaneHeaderTitlePadding");
        }

        public void HideCommandsButton()
        {
            _commandsButton.Visibility = Visibility.Collapsed;
        }

        public Border HeaderBorder;

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
                    HeaderBorder.SetResourceReference(Border.BackgroundProperty, "SelectedPaneBrush");
                }
                else
                {
                    HeaderBorder.SetResourceReference(Border.BackgroundProperty, "ToolPaneHeaderBackground");
                }
            }
        }

        public void ShowAsUnPinned()
        {
            (_pinButton.LayoutTransform as System.Windows.Media.RotateTransform).Angle = 90.0;
            (_pinButton.LayoutTransform as System.Windows.Media.RotateTransform).CenterX = 0.5;
            (_pinButton.LayoutTransform as System.Windows.Media.RotateTransform).CenterY = 0.5;
        }

        public event EventHandler UnPinClick;

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            UnPinClick?.Invoke(this, null);
        }

        private void DocumentContainer_SelectionChanged(object sender, EventArgs e)
        {
            _titleLabel.Content = IViewContainer.Title;
        }

        protected Label _titleLabel;

        public string Title { get { return IViewContainer.Title; } }

        private Button _pinButton;
        private Button _closeButton;
        private Button _commandsButton;
        private Point _mouseDownPosition;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(this);
            if (pt.Y <= HeaderBorder.ActualHeight)
            {
                _mouseDownPosition = pt;
                System.Windows.Input.Mouse.Capture(this);
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            System.Windows.Input.Mouse.Capture(this, CaptureMode.None);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (System.Windows.Input.Mouse.Captured == this)
            {
                Point mousePosition = e.GetPosition(this);
                double xdiff = mousePosition.X - _mouseDownPosition.X;
                double ydiff = mousePosition.Y - _mouseDownPosition.Y;
                if ((xdiff * xdiff + ydiff * ydiff) > 200)
                {

                    FireFloat(true);
                    System.Windows.Input.Mouse.Capture(this, CaptureMode.None);
                }
            }
        }
    }
}
