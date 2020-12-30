using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections;

namespace OpenControls.Wpf.Carousel
{
    public class CarouselControl : Canvas, ICarouselControl
    {
        public CarouselControl()
        {
            _timer.Tick += TimerTick;
            _timer.Interval = TimeSpan.FromMilliseconds(10);

            LayoutUpdated += CarouselControl_LayoutUpdated;

            _canvas = new Canvas();
            _canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            _canvas.VerticalAlignment = VerticalAlignment.Stretch;

            Children.Add(_canvas);
        }

        ~CarouselControl()
        {
            _timer.Tick -= TimerTick;
        }

        /*
         * Use a canvas to host the carousel items.
         */
        private Canvas _canvas;

        private void CarouselControl_LayoutUpdated(object sender, EventArgs e)
        {
            SetElementPositions();
        }

        private FrameworkElement GetChild(int index)
        {
            if ((_canvas.Children.Count == 0) || (index >= _canvas.Children.Count))
            {
                return null;
            }

            FrameworkElement element = _canvas.Children[index] as FrameworkElement;

            if (element == null)
            {
                throw new NotSupportedException("Carousel only supports children that are Framework elements");
            }

            return element;
        }

        /*
         * Find the index of the specified item
         */
        private int GetItemIndex(FrameworkElement targetElement)
        {
            if ((_canvas.Children.Count == 0) || (targetElement == null))
            {
                return 0;
            }

            int targetIndex = 0;
            if (targetElement != null)
            {
                for (int index = 0; index < _canvas.Children.Count; index++)
                {
                    FrameworkElement element = _canvas.Children[index] as FrameworkElement;
                    if (element.DataContext == targetElement)
                    {
                        targetIndex = index;
                        break;
                    }
                }
            }

            return targetIndex;
        }

        #region dependency properties 

        #region CarouselItemTemplate dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty CarouselItemTemplateProperty = DependencyProperty.Register("CarouselItemTemplate", typeof(ControlTemplate), typeof(CarouselControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCarouselItemTemplateChanged)));

        public ControlTemplate CarouselItemTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(CarouselItemTemplateProperty);
            }
            set
            {
                SetValue(CarouselItemTemplateProperty, value);
            }
        }

        private static void OnCarouselItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CarouselControl)d).OnCarouselItemTemplateChanged(e);
        }

        protected virtual void OnCarouselItemTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                CarouselItemTemplate = (ControlTemplate)e.NewValue;
            }
        }

        #endregion

        #region SelectedItem dependency property

        /*
         * The data context of the selected framework element
         */

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(Object), typeof(CarouselControl), new FrameworkPropertyMetadata((FrameworkElement)null, new PropertyChangedCallback(OnSelectedItemChanged)));
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
            ((CarouselControl)d).OnSelectedItemChanged(e);
        }

        protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                for (int index = 0; index < _canvas.Children.Count; index++)
                {
                    FrameworkElement element = GetChild(index);
                    if (element.DataContext == e.NewValue)
                    {
                        SelectElement(element);
                        return;
                    }
                }
            }
        }

        #endregion

        #region ItemsSource dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(CarouselControl), new FrameworkPropertyMetadata((IEnumerable)null, new PropertyChangedCallback(OnItemsSourceChanged)));

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
            ((CarouselControl)d).OnItemsSourceChanged(e);
        }

        private void PrepareItemsSource(IEnumerable itemsSource)
        {
            _canvas.Children.Clear();

            foreach (var item in itemsSource)
            {
                var itemControl = new Control();
                itemControl.Template = CarouselItemTemplate;

                itemControl.DataContext = item;
                _canvas.Children.Add(itemControl);
            }

            AddMouseLeftButtonDownHandlers();
            bool showRotation = ShowRotation;
            ShowRotation = false;
            ResetLayout();
            ShowRotation = showRotation;
        }

        protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                PrepareItemsSource(e.NewValue as IEnumerable);

                if (ItemsSource is System.Collections.Specialized.INotifyCollectionChanged)
                {
                    (ItemsSource as System.Collections.Specialized.INotifyCollectionChanged).CollectionChanged += CarouselControl_CollectionChanged;
                }
            }
        }

        private void CarouselControl_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PrepareItemsSource(ItemsSource);
        }

        #endregion

        #region AutoSizeToParent dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty AutoSizeToParentProperty = DependencyProperty.Register("AutoSizeToParent", typeof(bool), typeof(CarouselControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAutoSizeToParentChanged)));

        public bool AutoSizeToParent
        {
            get
            {
                return (bool)GetValue(AutoSizeToParentProperty);
            }
            set
            {
                SetValue(AutoSizeToParentProperty, value);
            }
        }

        private static void OnAutoSizeToParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CarouselControl)d).OnAutoSizeToParentChanged(e);
        }

        protected virtual void OnAutoSizeToParentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                AutoSizeToParent = (bool)e.NewValue;
            }
        }

        #endregion

        #region TiltInDegrees dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty TiltInDegreesProperty = DependencyProperty.Register("TiltInDegrees", typeof(double), typeof(CarouselControl), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnTiltInDegreesChanged)));

        private const double constMinimumLookdownOffset = -100;
        private const double constMaximumLookdownOffset = 100;

        public double TiltInDegrees
        {
            get
            {
                return (double)GetValue(TiltInDegreesProperty);
            }
            set
            {
                SetValue(TiltInDegreesProperty, Math.Min(Math.Max(value, constMinimumLookdownOffset), constMaximumLookdownOffset));
            }
        }

        private static void OnTiltInDegreesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CarouselControl)d).OnTiltInDegreesChanged(e);
        }

        protected virtual void OnTiltInDegreesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                TiltInDegrees = (double)e.NewValue;
            }
        }

        #endregion

        #region RotationSpeed dependency property

        private const double constMinimumRotationSpeed = 1;
        private const double constMaximumRotationSpeed = 1000;

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty RotationSpeedProperty = DependencyProperty.Register("RotationSpeed", typeof(double), typeof(CarouselControl), new FrameworkPropertyMetadata(constMaximumRotationSpeed, new PropertyChangedCallback(OnRotationSpeedChanged)));

        public double RotationSpeed
        {
            get
            {
                return (double)GetValue(RotationSpeedProperty);
            }
            set
            {
                SetValue(RotationSpeedProperty, Math.Min(Math.Max(value, constMinimumRotationSpeed), constMaximumRotationSpeed));
            }
        }

        private static void OnRotationSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CarouselControl)d).OnRotationSpeedChanged(e);
        }

        protected virtual void OnRotationSpeedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                RotationSpeed = (double)e.NewValue;
            }
        }

        #endregion

        #region Fade dependency property

        private const double constMinimumFade = 0;
        private const double constMaximumFade = 1;

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty FadeProperty = DependencyProperty.Register("Fade", typeof(double), typeof(CarouselControl), new FrameworkPropertyMetadata(0.5, new PropertyChangedCallback(OnFadeChanged)));

        public double Fade
        {
            get
            {
                return (double)GetValue(FadeProperty);
            }
            set
            {
                SetValue(FadeProperty, Math.Min(Math.Max(value, constMinimumFade), constMaximumFade));
            }
        }

        private static void OnFadeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CarouselControl)d).OnFadeChanged(e);
        }

        protected virtual void OnFadeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Fade = (double)e.NewValue;
            }
        }

        #endregion

        #region Scale dependency property

        private const double constMinimumScale = 0;
        private const double constMaximumScale = 1;

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("ScaleProperty", typeof(double), typeof(CarouselControl), new FrameworkPropertyMetadata(0.5, new PropertyChangedCallback(OnScaleChanged)));

        public double Scale
        {
            get
            {
                return (double)GetValue(ScaleProperty);
            }
            set
            {
                SetValue(ScaleProperty, Math.Min(Math.Max(value, constMinimumScale), constMaximumScale));
            }
        }

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CarouselControl)d).OnScaleChanged(e);
        }

        protected virtual void OnScaleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Scale = (double)e.NewValue;
            }
        }

        #endregion

        #region VerticalOrientation dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty VerticalOrientationProperty = DependencyProperty.Register("VerticalOrientationProperty", typeof(bool), typeof(CarouselControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnVerticalOrientationChanged)));

        public bool VerticalOrientation
        {
            get
            {
                return (bool)GetValue(VerticalOrientationProperty);
            }
            set
            {
                SetValue(VerticalOrientationProperty, value);
            }
        }

        private static void OnVerticalOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CarouselControl)d).OnVerticalOrientationChanged(e);
        }

        protected virtual void OnVerticalOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                VerticalOrientation = (bool)e.NewValue;
                ResetLayout();
            }
        }

        #endregion

        #endregion

        // Event handlers
        public delegate void SelectionChangedEventHandler(FrameworkElement selectedElement);
        public event SelectionChangedEventHandler SelectionChanged;

        private DateTime _previousTime;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            AddMouseLeftButtonDownHandlers();
            ResetLayout();
        }

        private void AddMouseLeftButtonDownHandlers()
        {
            foreach (FrameworkElement element in _canvas.Children)
            {
                element.MouseLeftButtonDown += element_MouseLeftButtonDown;
                element.Unloaded += delegate { MouseLeftButtonDown -= element_MouseLeftButtonDown; };
                element.Cursor = Cursors.Hand;
            }

            MouseWheel += CarouselControl_MouseWheel;
        }

        private void CarouselControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            StartRotation(e.Delta / 2);
        }

        private void ResetLayout()
        {
            _previousTime = DateTime.Now;

            if (SelectedItem == null)
            {
                SelectElement(GetChild(0));
            }

            SetElementPositions();
        }

        private void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RotateToElement(sender as FrameworkElement);
        }

        private void StartRotation(double numberOfDegrees)
        {
            _rotationToGo = numberOfDegrees;
            _previousTime = DateTime.Now;
            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }
        }

        private double GetDegreesNeededToPlaceElementInFront(int targetIndex)
        {
            double rawDegrees = _currentRotation - 180.0 + (360.0 * (double)targetIndex) / (double)_canvas.Children.Count;

            if (rawDegrees > 180.0)
            {
                return -(360.0 - rawDegrees);
            }

            return rawDegrees;
        }

        private void RotateToElement(FrameworkElement element)
        {
            SelectedItem = element.DataContext;
            int targetIndex = _canvas.Children.IndexOf(element);

            double degreesToRotate = GetDegreesNeededToPlaceElementInFront(targetIndex);

            if (!ShowRotation)
            {
                _currentRotation = ClampDegrees(_currentRotation - degreesToRotate);
                SetElementPositions();
            }
            else
            {
                StartRotation(degreesToRotate);
            }
        }

        private DispatcherTimer _timer = new DispatcherTimer();
        private double _rotationToGo = 0;
        private double _currentRotation = 0;

        private void TimerTick(object sender, EventArgs e)
        {
            DateTime timeNow = DateTime.Now;
            double rotationAmount = (timeNow - _previousTime).TotalSeconds * RotationSpeed;
            _previousTime = timeNow;

            if (Math.Abs(_rotationToGo) < rotationAmount)
            {
                rotationAmount = Math.Abs(_rotationToGo);
                _timer.Stop();
            }

            if (_rotationToGo < 0.0)
            {
                rotationAmount = -rotationAmount;
            }

            _rotationToGo -= rotationAmount;
            _currentRotation = ClampDegrees(_currentRotation - rotationAmount);

            SetElementPositions();
        }

        private double ClampDegrees(double rawDegrees)
        {
            if (rawDegrees > 360.0)
            {
                return rawDegrees - 360.0;
            }

            if (rawDegrees < 0.0)
            {
                return rawDegrees + 360.0;
            }

            return rawDegrees;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private double RadiansToDegrees(double radians)
        {
            return (360.0 * radians) / (Math.PI + Math.PI );
        }

        private double GetCoefficient(double degrees)
        {
            return (1.0 - Math.Cos(DegreesToRadians(degrees))) / 2.0;
        }

        private void SetOpacity(FrameworkElement element, double degrees)
        {
            element.Opacity = (1.0 - Fade) + Fade * GetCoefficient(degrees);
        }

        private double GetScaledSize(double degrees)
        {
            return (1.0 - Scale) + Scale * GetCoefficient(degrees);
        }

        private int GetZValue(double degrees)
        {
            return (int)(360 * GetCoefficient(degrees));
        }

        private void SetElementPositions()
        {
            if (_canvas.Children.Count <= 0)
            {
                return;
            }

            FrameworkElement element = GetChild(0);

            double elementHalfWidth = element.ActualWidth / 2.0;
            double elementHalfHeight = element.ActualHeight / 2.0;
            double canvasHalfWidth = VerticalOrientation ? 0 : ActualWidth / 2.0 - elementHalfWidth;
            double canvasHalfHeight = VerticalOrientation ? canvasHalfHeight = ActualHeight / 2.0 - elementHalfHeight : 0;

            for (int index = 0; index < _canvas.Children.Count; index++)
            {
                double degrees = (360.0 * (double)index ) / (double)_canvas.Children.Count + _currentRotation;
                double cosineAngle = Math.Cos(DegreesToRadians(degrees));
                double sineAngle = Math.Sin(DegreesToRadians(degrees));

                element = GetChild(index);

                double x = -canvasHalfWidth * sineAngle - (double.IsNaN(canvasHalfHeight) ? 0.0 : canvasHalfHeight / 100.0) * cosineAngle * TiltInDegrees;
                Canvas.SetLeft(element, x + ActualWidth / 2.0 - elementHalfWidth);

                double y = canvasHalfHeight * sineAngle - (double.IsNaN(canvasHalfWidth) ? 0.0 : canvasHalfWidth / 100.0) * cosineAngle * TiltInDegrees;
                Canvas.SetTop(element, y + ActualHeight / 2.0 - elementHalfHeight);

                ScaleTransform scale = element.RenderTransform as ScaleTransform;
                if (scale == null)
                {
                    scale = new ScaleTransform();
                    element.RenderTransform = scale;
                }

                scale.CenterX = elementHalfWidth;
                scale.CenterY = elementHalfHeight;
                double scaledSize = GetScaledSize(degrees);
                scale.ScaleX = scale.ScaleY = scaledSize * scaledSize;
                Canvas.SetZIndex(element, GetZValue(degrees));

                SetOpacity(element, degrees);
            }
        }

        public double GetMinimumRotationToPlaceElementInFront(bool leftToRightRotation, out FrameworkElement nearestElement)
        {
            nearestElement = null;
            double minimumDegrees = leftToRightRotation ? 360 : -360;
            for (int index = 0; index < _canvas.Children.Count; ++index)
            {
                double degrees = GetDegreesNeededToPlaceElementInFront(index);
                if (leftToRightRotation)
                {
                    if (degrees > 0.0)
                    {
                        if (degrees < minimumDegrees)
                        {
                            nearestElement = _canvas.Children[index] as FrameworkElement;
                            minimumDegrees = degrees;
                        }
                    }
                }
                else
                {
                    if (degrees < 0.0)
                    {
                        if (degrees > minimumDegrees)
                        {
                            nearestElement = _canvas.Children[index] as FrameworkElement;
                            minimumDegrees = degrees;
                        }
                    }
                }
            }

            return minimumDegrees;
        }

        private void SelectElement(FrameworkElement element)
        {
            if (element != null)
            {
                _previousTime = DateTime.Now;
                RotateToElement(element);
                SelectedItem = element.DataContext;
                if (SelectionChanged != null)
                {
                    SelectionChanged(element);
                }
            }
        }

        #region ICarouselControl

        public bool ShowRotation{ get; set; }

        /*
         * The user has moved a finger from startScreenX to endScreenX along the x-axis => rotate the elements
         * 
         * The items are placed equidistant from each other around a circle
         * 
         */
        public double Rotate(double startXInScreenCoordinates, double endXInScreenCoordinates)
        {
            System.Windows.Point pt1 = PointFromScreen(new System.Windows.Point(startXInScreenCoordinates, 0));
            System.Windows.Point pt2 = PointFromScreen(new System.Windows.Point(endXInScreenCoordinates, 0));

            double R = Width / 2.0;

            // δ θ = asin((width/2 – x1)/R) - asin((width/2 – x2)/R)

            double deltaTheta = Math.Asin((Width / 2.0 - pt1.X) / R) - Math.Asin((Width / 2.0 - pt2.X) / R);

            if (deltaTheta.CompareTo(double.NaN) == 0)
            {
                return 0;
            }

            double degreesToRotate = RadiansToDegrees(deltaTheta);

            _currentRotation = ClampDegrees(_currentRotation + degreesToRotate);

            SetElementPositions();

            // Note: Do not set the current element as that causes it to rotate into position causing jerky rotation

            return degreesToRotate;
        }

        /*
         * Ensure that the front most element is selected and snapped into position
         */
        public void SelectFrontMostElement()
        {
            FrameworkElement frameworkElement = null;
            foreach (var child in (this.Children[0] as System.Windows.Controls.Canvas).Children)
            {
                if (frameworkElement == null)
                {
                    frameworkElement = child as FrameworkElement;
                }
                else
                {
                    if (Canvas.GetZIndex(child as FrameworkElement) > Canvas.GetZIndex(frameworkElement))
                    {
                        frameworkElement = child as FrameworkElement;
                    }
                }
            }

            if (SelectedItem != frameworkElement.DataContext)
            {
                SelectedItem = frameworkElement.DataContext;
            }
        }

        public void Rotate(double angleInDegrees)
        {
            _currentRotation = ClampDegrees(_currentRotation + angleInDegrees);

            SetElementPositions();
        }

        public void RotateIncrement(int increment)
        {
            int requiredIndex = 1;
            if (SelectedItem != null)
            {
                for (int index = 0; index < _canvas.Children.Count; index++)
                {
                    FrameworkElement element = GetChild(index);
                    if (element.DataContext == SelectedItem)
                    {
                        requiredIndex = index + increment;
                        break;
                    }
                }
            }

            while (requiredIndex >= _canvas.Children.Count)
            {
                requiredIndex -= _canvas.Children.Count;
            }
            while (requiredIndex < 0)
            {
                requiredIndex += _canvas.Children.Count;
            }

            double degreesToRotate = (360.0 * (double)increment) / (double)_canvas.Children.Count;

            SelectedItem = GetChild(requiredIndex).DataContext;
            StartRotation(degreesToRotate);
        }

        public void RotateRight()
        {
            RotateIncrement(-1);
        }

        public void RotateLeft()
        {
            RotateIncrement(1);
        }

        #endregion ICarouselControl
    }
}
