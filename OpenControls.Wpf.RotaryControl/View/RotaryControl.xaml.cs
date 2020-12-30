using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Input;

namespace OpenControls.Wpf.RotaryControl.View
{
    /*
     *  Represents an arc or ring
     */
    public class RotaryControlArc
    {
        public Point Centre { get; set; }
        public double Radius { get; set; }
        public double StartAngleInDegrees { get; set; }
        public double AngleInDegrees { get; set; }
        public double Thickness { get; set; }
        public Brush Fill { get; set; }
        public Brush Stroke { get; set; }
        public double StrokeThickness { get; set; }

        public RotaryControlArc()
        {
            Fill = Brushes.White;
            Stroke = Brushes.Black;
            StrokeThickness = 0;
            Thickness = 10;
        }

        public RotaryControlArc Clone()
        {
            RotaryControlArc arc = new RotaryControlArc();
            arc.Centre = Centre;
            arc.AngleInDegrees = AngleInDegrees;
            arc.StartAngleInDegrees = StartAngleInDegrees;
            arc.Radius = Radius;
            arc.StrokeThickness = 0;
            arc.Thickness = Thickness;
            arc.Fill = Fill;

            return arc;
        }

        public static Point ComputeCartesianCoordinate(double angle, double radius)
        {
            // convert to radians
            double angleRad = (Math.PI / 180.0) * (angle - 90);

            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }

        public Path CreateArcSegment()
        {
            Point outerArcStartPoint = RotaryControlArc.ComputeCartesianCoordinate(StartAngleInDegrees, Radius);
            outerArcStartPoint.Offset(Centre.X, Centre.Y);

            Point outerArcEndPoint = RotaryControlArc.ComputeCartesianCoordinate(StartAngleInDegrees + AngleInDegrees, Radius);
            outerArcEndPoint.Offset(Centre.X, Centre.Y);

            bool largeArc = AngleInDegrees > 180.0;
            Size outerArcSize = new Size(Radius, Radius);

            double insideRadius = Radius - Thickness;

            Point innerArcStartPoint = RotaryControlArc.ComputeCartesianCoordinate(StartAngleInDegrees, insideRadius);
            innerArcStartPoint.Offset(Centre.X, Centre.Y);

            Point innerArcEndPoint = RotaryControlArc.ComputeCartesianCoordinate(StartAngleInDegrees + AngleInDegrees, insideRadius);
            innerArcEndPoint.Offset(Centre.X, Centre.Y);

            Size innerArcSize = new Size(insideRadius, insideRadius);

            Path path = new Path();
            path.Fill = Fill;
            path.Stroke = Stroke;
            path.StrokeThickness = StrokeThickness;

            PathGeometry pathGeometry = new PathGeometry();
            path.Data = pathGeometry;

            PathFigure pathFigure = new PathFigure();
            pathFigure.IsFilled = true;
            pathFigure.StartPoint = outerArcStartPoint;
            pathGeometry.Figures.Add(pathFigure);

            ArcSegment arcSegment = new ArcSegment();
            arcSegment.Point = outerArcEndPoint;
            arcSegment.Size = outerArcSize;
            arcSegment.SweepDirection = SweepDirection.Clockwise;
            arcSegment.IsLargeArc = largeArc;
            pathFigure.Segments.Add(arcSegment);

            LineSegment lineSegment = new LineSegment();
            lineSegment.Point = innerArcEndPoint;
            pathFigure.Segments.Add(lineSegment);

            arcSegment = new ArcSegment();
            arcSegment.Point = innerArcStartPoint;
            arcSegment.Size = innerArcSize;
            arcSegment.SweepDirection = SweepDirection.Counterclockwise;
            arcSegment.IsLargeArc = largeArc;
            pathFigure.Segments.Add(arcSegment);

            lineSegment = new LineSegment();
            lineSegment.Point = outerArcStartPoint;
            pathFigure.Segments.Add(lineSegment);

            return path;
        }
    }

    // Represents a coloured segment 

    public class RotaryControlSegment
    {
        public Brush Fill { get; set; }
        public int AngleInDegrees { get; set; }
    }

    /*
     * Modifications: 
     * 
     *      SegmentRadius, SegmentThickness:       To do
     *      InsideDialWidth:                        Done
     *      OutsideDialWidth, OutsideDialBorder:    Done 
     *      TickDialWidth, TickDialThickness        To do
     *      
     *      Create the components in this order to ensure required visibility: 
     *      
     *          Outside dial
     *          Inside dial
     *          Segments
     *          Ticks
     *      
     *      Label positions: 
     *          Outside the outer dial
     *          On the outer dial
     *          On the inner dial
     *          
     *      Start and end angles ... 
     */

    /// <summary>
    /// Interaction logic for CustomRotaryControl.xaml
    /// </summary>
    public partial class RotaryControl : UserControl
    {
        const double OneTwentyDegreesInRadians = (System.Math.PI + System.Math.PI) / 3;
        const double ThirtyDegreesInRadians = System.Math.PI / 6;

        private double StartAngleInRadians
        {
            get
            {
                return (StartAngleInDegrees * Math.PI) / 180.0;
            }
        }

        private double EndAngleInRadians
        {
            get
            {
                return (EndAngleInDegrees * Math.PI) / 180.0;
            }
        }

        // Hardcoded to be the same as the actual controlwidth in DIU
        private static double ControlWidth = 200;

        public RotaryControl()
        {
            InitializeComponent();
            PreviewMouseLeftButtonDown += RotaryControl_PreviewMouseLeftButtonDown;
            PreviewMouseDown += RotaryControl_PreviewMouseDown;
            MouseLeftButtonDown += RotaryControl_MouseLeftButtonDown;
            MouseLeftButtonUp += RotaryControl_MouseLeftButtonUp;
            MouseMove += RotaryControl_MouseMove;
            CreateControl();
        }

        private Point RotatePoint(Point point, double angleInDegrees, Point originPoint)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180.0);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point(
                (int)(cosTheta * (point.X - originPoint.X) - sinTheta * (point.Y - originPoint.Y) + originPoint.X),
                (int)(sinTheta * (point.X - originPoint.X) + cosTheta * (point.Y - originPoint.Y) + originPoint.Y));
        }

        private Size MeasureString(string candidate, Label label)
        {
            var formattedText = new FormattedText(
                candidate,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(label.FontFamily, label.FontStyle, label.FontWeight, label.FontStretch),
                label.FontSize,
                Brushes.Black,
                new NumberSubstitution(), TextFormattingMode.Display);

            return new Size(formattedText.Width, formattedText.Height);
        }

        System.Collections.Generic.List<Label> _labels;

        double constOuterDialWidth = 150.0;

        void CreateControl()
        {
            // Remove everything apart from the ellipses 

            for (int i = _grid.Children.Count - 1; i >= 0; --i)
            {
                if (!(_grid.Children[i] is Ellipse))
                {
                    _grid.Children.RemoveAt(i);
                }
            }

            _grid.Children.Add(_pointerStandard);
            _grid.Children.Add(_pointerAxle);
            _grid.Children.Add(_pointerArrow);
            _grid.Children.Add(_pointerRectangle);

            _ellipseOuterDial.Fill = OuterDialFill;
            _ellipseOuterDial.Stroke = OuterDialBorder;
            _ellipseOuterDial.StrokeThickness = OuterDialBorderThickness;

            _ellipseInnerDial.Width = InnerDialRadius * 2;
            _ellipseInnerDial.Height = InnerDialRadius * 2;
            _ellipseInnerDial.Fill = InnerDialFill;

            Point pointCentre = new Point(100.0, 100.0);
            if (Segments != null)
            {
                if (SegmentRadius == 0.0)
                {
                    SegmentRadius = 0.5 * constOuterDialWidth;
                }

                double segmentStartAngleInDegrees = StartAngleInDegrees;
                foreach (var item in Segments)
                {
                    double segmentAngleInDegrees = (double)(item as RotaryControlSegment).AngleInDegrees;
                    Brush brush = (item as RotaryControlSegment).Fill;

                    RotaryControlArc arc = new RotaryControlArc();
                    arc.Fill = (item as RotaryControlSegment).Fill;
                    arc.Radius = SegmentRadius;
                    arc.Centre = pointCentre;
                    arc.StartAngleInDegrees = segmentStartAngleInDegrees;
                    arc.AngleInDegrees = segmentAngleInDegrees;
                    arc.Thickness = SegmentThickness;
                    arc.Stroke = arc.Fill;
                    arc.StrokeThickness = 1;
                    segmentStartAngleInDegrees += segmentAngleInDegrees;

                    _grid.Children.Add(arc.CreateArcSegment());
                }
            }

            if (Arcs != null)
            {
                foreach (var item in Arcs)
                {
                    RotaryControlArc arc = item as RotaryControlArc;
                    arc.Centre = pointCentre;
                    _grid.Children.Add(arc.CreateArcSegment());
                }
            }

            _labels = new System.Collections.Generic.List<Label>();

            /*
             * Draw marker lines and labels: always 5 minor ticks
             */

            // Don't set MajorTickDialRadius and MajorTickLength as that will call this method! 

            double majorTicksDialWidth = MajorTickDialRadius * 2;
            if (majorTicksDialWidth == 0.0)
            {
                majorTicksDialWidth = constOuterDialWidth - 2 * OuterDialBorderThickness - 2;
            }

            double majorTickLength = MajorTickLength;
            if (majorTickLength == 0.0)
            {
                majorTickLength = (majorTicksDialWidth - InnerDialRadius * 2) / 2.0 - 2;
            }

            double majorTickStart = majorTicksDialWidth / 2.0;
            double majorTickEnd = majorTicksDialWidth / 2.0 - majorTickLength;

            double minorTickLength = (MinorTickLength > 0.0) ? MinorTickLength : majorTickLength / 8;
            double minorTickStart = (MinorTickDialRadius > 0.0) ? MinorTickDialRadius : majorTickEnd + minorTickLength;
            double minorTickEnd = minorTickStart - minorTickLength;

            // The angle in radians subtended by adjacent major ticks
            double angle = EndAngleInRadians - StartAngleInRadians;
            if (angle < 0)
            {
                angle += 2 * Math.PI;
            }
            double majorArc = angle / (NumberOfMajorTicks - 1);

            // The angle in radians subtended by adjacent minor ticks
            double minorArc = majorArc / (double)(NumberOfMinorTicks + 1);

            // Angles are measured relative to 3 o'clock. Thus 7 o'clock is 120 degrees etc. 
            double majorAngleInRadians = StartAngleInRadians;

            double labelsDialWidth = (LabelDialRadius > 0.0) ? 2.0 * LabelDialRadius : 1.2 * _ellipseOuterDial.Width;

            for (int iMajor = 0; iMajor < NumberOfMajorTicks; ++iMajor, majorAngleInRadians += majorArc)
            {
                // Major tick

                Polyline polyline = new Polyline();

                double cosineMajorAngle = System.Math.Cos(majorAngleInRadians);
                double sineMajorAngle = System.Math.Sin(majorAngleInRadians);

                double x = ControlWidth / 2 + majorTickStart * sineMajorAngle;
                double y = ControlWidth / 2 - majorTickStart * cosineMajorAngle;
                polyline.Points.Add(new Point(x, y));

                x = ControlWidth / 2 + majorTickEnd * sineMajorAngle;
                y = ControlWidth / 2 - majorTickEnd * cosineMajorAngle;
                polyline.Points.Add(new Point(x, y));

                polyline.Stroke = MajorTickBrush;
                polyline.StrokeThickness = MajorTickWidth;
                polyline.StrokeStartLineCap = PenLineCap.Square;
                polyline.StrokeEndLineCap = PenLineCap.Square;

                _grid.Children.Add(polyline);

                // Minor ticks

                if (iMajor != (NumberOfMajorTicks - 1))
                {
                    double minorAngleInRadians = majorAngleInRadians;
                    for (int iMinor = 1; iMinor <= NumberOfMinorTicks; ++iMinor)
                    {
                        minorAngleInRadians += minorArc;

                        polyline = new Polyline();

                        double cosineMinorAngle = System.Math.Cos(minorAngleInRadians);
                        double sineMinorAngle = System.Math.Sin(minorAngleInRadians);

                        x = ControlWidth / 2 + minorTickStart * sineMinorAngle;
                        y = ControlWidth / 2 - minorTickStart * cosineMinorAngle;
                        polyline.Points.Add(new Point(x, y));

                        x = ControlWidth / 2 + minorTickEnd * sineMinorAngle;
                        y = ControlWidth / 2 - minorTickEnd * cosineMinorAngle;
                        polyline.Points.Add(new Point(x, y));

                        polyline.Stroke = MinorTickBrush;
                        polyline.StrokeStartLineCap = PenLineCap.Round;
                        polyline.StrokeEndLineCap = PenLineCap.Round;
                        polyline.StrokeThickness = 1;

                        _grid.Children.Add(polyline);
                    }
                }

                // Major tick label

                Label label = new Label();
                string text = (MinimumValue + (iMajor * MajorTickIncrement)).ToString();
                label.Content = text;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Center;
                label.Foreground = FontBrush;
                label.FontSize = FontSize;
                _labels.Add(label);

                Size labelSize = MeasureString(text, label);

                x = labelsDialWidth / 2.0 * sineMajorAngle;
                y = -labelsDialWidth / 2.0 * cosineMajorAngle;

                label.RenderTransform = new TranslateTransform(x, y);
                _grid.Children.Add(label);

                UpdateMarkerPosition();
            }
        }

        public void PositionMarkerFromControlPosition(Point pointInControl)
        {
            double dX = pointInControl.X - _ellipseOuterDial.Width / 2;
            double dY = pointInControl.Y - _ellipseOuterDial.Height / 2;

            // The angle from the 12 o'clock position
            double angleInDegrees = -(360 * System.Math.Atan(dX / dY)) / (System.Math.PI + System.Math.PI);

            if (dY > 0)
            {
                angleInDegrees += 180;
            }

            double totalGaugeAngle = 360 - (StartAngleInDegrees - EndAngleInDegrees);
            if (totalGaugeAngle > 360)
            {
                totalGaugeAngle -= 360;
            }
            else if (totalGaugeAngle < 0)
            {
                totalGaugeAngle += 360;
            }

            double normalisedAngleInDegrees = (angleInDegrees > 360) ? (angleInDegrees - 360) : angleInDegrees;
            if ((normalisedAngleInDegrees < StartAngleInDegrees) && (normalisedAngleInDegrees > EndAngleInDegrees))
            {
                double arc = StartAngleInDegrees - normalisedAngleInDegrees;
                if (arc < (360 - totalGaugeAngle) / 2)
                {
                    Value = MinimumValue;
                }
                else
                {
                    Value = MinimumValue + (MajorTickIncrement * (NumberOfMajorTicks - 1));
                }
                return;
            }

            // Start and end angle are measured in a clockwise fashion from the 12 o'clock position

            if (angleInDegrees < StartAngleInDegrees)
            {
                angleInDegrees += 360;
            }

            double deltaAngle = angleInDegrees - StartAngleInDegrees;
            if (deltaAngle > 360)
            {
                deltaAngle -= 360;
            }

            Value = MinimumValue + ((MajorTickIncrement * (NumberOfMajorTicks - 1)) * deltaAngle) / totalGaugeAngle;
        }

        public bool PositionMarker(double xPositionOnScreen, double yPositionOnScreen)
        {
            Point pointOnScreen = new Point(xPositionOnScreen, yPositionOnScreen);
            if (_ellipseOuterDial.InputHitTest(_ellipseOuterDial.PointFromScreen(pointOnScreen)) == null)
            {
                return false;
            }

            PositionMarkerFromControlPosition(_ellipseOuterDial.PointFromScreen(pointOnScreen));

            return true;
        }

        private void UpdateMarkerPosition()
        {
            double offsetFromCentre = InnerDialRadius - 15;

            // The total angle in radians subtended by the gauge
            double arcAngleInRadians = EndAngleInRadians - StartAngleInRadians;
            if (arcAngleInRadians < 0)
            {
                arcAngleInRadians += 2 * Math.PI;
            }

            // The angle of the marker
            double majorAngleInRadians = StartAngleInRadians + (arcAngleInRadians * (Value - MinimumValue)) / (MajorTickIncrement * (NumberOfMajorTicks - 1));

            double x = offsetFromCentre * System.Math.Sin(majorAngleInRadians);
            double y = -offsetFromCentre * System.Math.Cos(majorAngleInRadians);

            _markerTranslation.X = x;
            _markerTranslation.Y = y;

            _pointerAxle.Visibility = Visibility.Hidden;
            _pointerCircle.Visibility = Visibility.Hidden;
            _pointerStandard.Visibility = Visibility.Hidden;
            _pointerArrow.Visibility = Visibility.Hidden;
            _pointerRectangle.Visibility = Visibility.Hidden;

            majorAngleInRadians -= 0.5 * Math.PI;

            if (PointerType == "standard")
            {
                _pointerAxle.Visibility = Visibility.Visible;
                _pointerStandard.Visibility = Visibility.Visible;

                double pointerLength = PointerLength;
                if (pointerLength == 0.0)
                {
                    pointerLength = _ellipseInnerDial.Width / 2.0;
                }

                _pointerTopRight.Point = new Point(100 + pointerLength - 10, _pointerTopRight.Point.Y);
                _pointerBottomRight.Point = new Point(100 + pointerLength - 10, _pointerBottomRight.Point.Y);
                _pointerTip.Point = new Point(100 + pointerLength, _pointerTip.Point.Y);
                _pointerStandard.RenderTransform = new RotateTransform((majorAngleInRadians * 180.0) / Math.PI, 100, 100);
            }
            else if (PointerType == "rectangle")
            {
                _pointerAxle.Visibility = Visibility.Visible;
                _pointerRectangle.Visibility = Visibility.Visible;

                double pointerLength = PointerLength;
                if (pointerLength == 0.0)
                {
                    pointerLength = _ellipseInnerDial.Width / 2.0;
                }

                _pointerRectangleTopRight.Point = new Point(100 + pointerLength, 100 - PointerWidth / 2);
                _pointerRectangleBottomRight.Point = new Point(100 + pointerLength, 100 + PointerWidth / 2);
                _pointerRectangleTopLeft.Point = new Point(100, 100 - PointerWidth / 2);
                _pointerRectangleBottomLeft.Point = new Point(100, 100 + PointerWidth / 2);
                _pointerRectangle.RenderTransform = new RotateTransform((majorAngleInRadians * 180.0) / Math.PI, 100, 100);
            }
            else if (PointerType == "arrow")
            {
                _pointerAxle.Visibility = Visibility.Visible;
                _pointerArrow.Visibility = Visibility.Visible;

                double pointerLength = PointerLength;
                if (pointerLength == 0.0)
                {
                    pointerLength = _ellipseInnerDial.Width / 2.0;
                }

                _pointerArrowTip.Point = new Point(100 + pointerLength, 100);
                _pointerArrowTopLeft.Point = new Point(100, 100 - PointerWidth / 2);
                _pointerArrowBottomLeft.Point = new Point(100, 100 + PointerWidth / 2);
                _pointerArrow.RenderTransform = new RotateTransform((majorAngleInRadians * 180.0) / Math.PI, 100, 100);
            }
            else
            {
                _pointerAxle.Visibility = Visibility.Hidden;
                _pointerCircle.Visibility = Visibility.Visible;
                _pointerStandard.Visibility = Visibility.Hidden;
            }
        }

        private static Brush DefaultInnerDialFill()
        {
            return new LinearGradientBrush(Color.FromRgb(0xBB, 0xBB, 0xBB), Color.FromRgb(0xDD, 0xDD, 0xDD), new Point(0.5, 1.0), new Point(0.5, 0.0));
        }

        private static Brush DefaultPointerFill()
        {
            return new LinearGradientBrush(Colors.Red, Colors.DarkRed, new Point(0.5, 1.0), new Point(0.5, 0.0));
        }

        #region dependency properties 

        #region Value dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnValueChanged)));

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                double maximumValue = MinimumValue + ((NumberOfMajorTicks - 1) * MajorTickIncrement);
                SetValue(ValueProperty, Math.Min(Math.Max(value, MinimumValue), maximumValue));
                UpdateMarkerPosition();
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnValueChanged(e);
        }

        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Value = (double)e.NewValue;
            }
        }

        #endregion

        #region MinimumValue dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty MinimumValueProperty = DependencyProperty.Register("MinimumValue", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnMinimumValueChanged)));

        public double MinimumValue
        {
            get
            {
                return (double)GetValue(MinimumValueProperty);
            }
            set
            {
                SetValue(MinimumValueProperty, value);
                CreateControl();
            }
        }

        private static void OnMinimumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnMinimumValueChanged(e);
        }

        protected virtual void OnMinimumValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Value = (double)e.NewValue;
            }
        }

        #endregion

        #region FontBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty FontBrushProperty = DependencyProperty.Register("FontBrush", typeof(Brush), typeof(RotaryControl), new FrameworkPropertyMetadata(System.Windows.Media.Brushes.Black, new PropertyChangedCallback(OnFontBrushChanged)));

        public Brush FontBrush
        {
            get
            {
                return (Brush)GetValue(FontBrushProperty);
            }
            set
            {
                SetValue(FontBrushProperty, value);
                foreach (var label in _labels)
                {
                    label.Foreground = value;
                }
            }
        }

        private static void OnFontBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnFontBrushChanged(e);
        }

        protected virtual void OnFontBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                FontBrush = (Brush)e.NewValue;
            }
        }

        #endregion

        #region StartAngleInDegrees dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty StartAngleInDegreesProperty = DependencyProperty.Register("StartAngleInDegrees", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(210.0, new PropertyChangedCallback(OnStartAngleInDegreesChanged)));

        public double StartAngleInDegrees
        {
            get
            {
                return (double)GetValue(StartAngleInDegreesProperty);
            }
            set
            {
                SetValue(StartAngleInDegreesProperty, value);
                CreateControl();
            }
        }

        private static void OnStartAngleInDegreesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnStartAngleInDegreesChanged(e);
        }

        protected virtual void OnStartAngleInDegreesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                StartAngleInDegrees = (double)e.NewValue;
            }
        }

        #endregion

        #region EndAngleInDegrees dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty EndAngleInDegreesProperty = DependencyProperty.Register("EndAngleInDegrees", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(150.0, new PropertyChangedCallback(OnEndAngleInDegreesChanged)));

        public double EndAngleInDegrees
        {
            get
            {
                return (double)GetValue(EndAngleInDegreesProperty);
            }
            set
            {
                SetValue(EndAngleInDegreesProperty, value);
                CreateControl();
            }
        }

        private static void OnEndAngleInDegreesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnEndAngleInDegreesChanged(e);
        }

        protected virtual void OnEndAngleInDegreesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                EndAngleInDegrees = (double)e.NewValue;
            }
        }

        #endregion

        #region MajorTickDialRadius dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty MajorTickDialRadiusProperty = DependencyProperty.Register("MajorTickDialRadius", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnMajorTickDialRadiusChanged)));

        public double MajorTickDialRadius
        {
            get
            {
                return (double)GetValue(MajorTickDialRadiusProperty);
            }
            set
            {
                SetValue(MajorTickDialRadiusProperty, value);
                CreateControl();
            }
        }

        private static void OnMajorTickDialRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnMajorTickDialRadiusChanged(e);
        }

        protected virtual void OnMajorTickDialRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                MajorTickDialRadius = (double)e.NewValue;
            }
        }

        #endregion

        #region MajorTickLength dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty MajorTickLengthProperty = DependencyProperty.Register("MajorTickLength", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnMajorTickLengthChanged)));

        public double MajorTickLength
        {
            get
            {
                return (double)GetValue(MajorTickLengthProperty);
            }
            set
            {
                SetValue(MajorTickLengthProperty, value);
                CreateControl();
            }
        }

        private static void OnMajorTickLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnMajorTickLengthChanged(e);
        }

        protected virtual void OnMajorTickLengthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                MajorTickLength = (double)e.NewValue;
            }
        }

        #endregion

        #region MajorTickWidth dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty MajorTickWidthProperty = DependencyProperty.Register("MajorTickWidth", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(OnMajorTickWidthChanged)));

        public double MajorTickWidth
        {
            get
            {
                return (double)GetValue(MajorTickWidthProperty);
            }
            set
            {
                SetValue(MajorTickWidthProperty, value);
                CreateControl();
            }
        }

        private static void OnMajorTickWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnMajorTickWidthChanged(e);
        }

        protected virtual void OnMajorTickWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                MajorTickWidth = (double)e.NewValue;
            }
        }

        #endregion

        #region NumberOfMajorTicks dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty NumberOfMajorTicksProperty = DependencyProperty.Register("NumberOfMajorTicks", typeof(int), typeof(RotaryControl), new FrameworkPropertyMetadata(10, new PropertyChangedCallback(OnNumberOfMajorTicksChanged)));

        private const int constMinimumNumberOfMajorTicks = 3;
        private const int constMaximumNumberOfMajorTicks = 20;

        public int NumberOfMajorTicks
        {
            get
            {
                return (int)GetValue(NumberOfMajorTicksProperty);
            }
            set
            {
                SetValue(NumberOfMajorTicksProperty, Math.Min(Math.Max(value, constMinimumNumberOfMajorTicks), constMaximumNumberOfMajorTicks));
            }
        }

        private static void OnNumberOfMajorTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnNumberOfMajorTicksChanged(e);
        }

        protected virtual void OnNumberOfMajorTicksChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                NumberOfMajorTicks = (int)e.NewValue;
                CreateControl();
            }
        }

        #endregion

        #region MajorTickIncrement dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty MajorTickIncrementProperty = DependencyProperty.Register("MajorTickIncrement", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(10.0, new PropertyChangedCallback(OnMajorTickIncrementChanged)));

        private const double constMinimumMajorTickIncrement = 0.1;
        private const double constMaximumMajorTickIncrement = 1000;

        public double MajorTickIncrement
        {
            get
            {
                return (double)GetValue(MajorTickIncrementProperty);
            }
            set
            {
                SetValue(MajorTickIncrementProperty, Math.Min(Math.Max(value, constMinimumMajorTickIncrement), constMaximumMajorTickIncrement));
            }
        }

        private static void OnMajorTickIncrementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnMajorTickIncrementChanged(e);
        }

        protected virtual void OnMajorTickIncrementChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                MajorTickIncrement = (double)e.NewValue;
                CreateControl();
            }
        }

        #endregion

        #region MajorTickBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty MajorTickBrushProperty = DependencyProperty.Register("MajorTickBrush", typeof(Brush), typeof(RotaryControl), new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(OnMajorTickBrushChanged)));

        public Brush MajorTickBrush
        {
            get
            {
                return (Brush)GetValue(MajorTickBrushProperty);
            }
            set
            {
                SetValue(MajorTickBrushProperty, value);
                CreateControl();
            }
        }

        private static void OnMajorTickBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnMajorTickBrushChanged(e);
        }

        protected virtual void OnMajorTickBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                MajorTickBrush = (Brush)e.NewValue;
            }
        }

        #endregion MajorTickBrush dependency property

        #region MinorTickDialRadius dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty MinorTickDialRadiusProperty = DependencyProperty.Register("MinorTickDialRadius", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnMinorTickDialRadiusChanged)));

        public double MinorTickDialRadius
        {
            get
            {
                return (double)GetValue(MinorTickDialRadiusProperty);
            }
            set
            {
                SetValue(MinorTickDialRadiusProperty, value);
                CreateControl();
            }
        }

        private static void OnMinorTickDialRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnMinorTickDialRadiusChanged(e);
        }

        protected virtual void OnMinorTickDialRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                MinorTickDialRadius = (double)e.NewValue;
            }
        }

        #endregion

        #region MinorTickLength dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty MinorTickLengthProperty = DependencyProperty.Register("MinorTickLength", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnMinorTickLengthChanged)));

        public double MinorTickLength
        {
            get
            {
                return (double)GetValue(MinorTickLengthProperty);
            }
            set
            {
                SetValue(MinorTickLengthProperty, value);
                CreateControl();
            }
        }

        private static void OnMinorTickLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnMinorTickLengthChanged(e);
        }

        protected virtual void OnMinorTickLengthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                MinorTickLength = (double)e.NewValue;
            }
        }

        #endregion

        #region NumberOfMinorTicks dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty NumberOfMinorTicksProperty = DependencyProperty.Register("NumberOfMinorTicks", typeof(int), typeof(RotaryControl), new FrameworkPropertyMetadata(4, new PropertyChangedCallback(OnNumberOfMinorTicksChanged)));

        private const int constMinimumNumberOfMinorTicks = 0;
        private const int constMaximumNumberOfMinorTicks = 20;

        public int NumberOfMinorTicks
        {
            get
            {
                return (int)GetValue(NumberOfMinorTicksProperty);
            }
            set
            {
                SetValue(NumberOfMinorTicksProperty, Math.Min(Math.Max(value, constMinimumNumberOfMinorTicks), constMaximumNumberOfMinorTicks));
            }
        }

        private static void OnNumberOfMinorTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnNumberOfMinorTicksChanged(e);
        }

        protected virtual void OnNumberOfMinorTicksChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                NumberOfMinorTicks = (int)e.NewValue;
                CreateControl();
            }
        }

        #endregion NumberOfMinorTicks dependency property

        #region MinorTickBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty MinorTickBrushProperty = DependencyProperty.Register("MinorTickBrush", typeof(Brush), typeof(RotaryControl), new FrameworkPropertyMetadata(Brushes.Blue, new PropertyChangedCallback(OnMinorTickBrushChanged)));

        public Brush MinorTickBrush
        {
            get
            {
                return (Brush)GetValue(MinorTickBrushProperty);
            }
            set
            {
                SetValue(MinorTickBrushProperty, value);
                CreateControl();
            }
        }

        private static void OnMinorTickBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnMinorTickBrushChanged(e);
        }

        protected virtual void OnMinorTickBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                MinorTickBrush = (Brush)e.NewValue;
            }
        }

        #endregion MinorTickBrush dependency property

        #region InnerDialRadius dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty InnerDialRadiusProperty = DependencyProperty.Register("InnerDialRadius", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(100.0, new PropertyChangedCallback(OnInnerDialRadiusChanged)));

        private const double constMinimumInnerDialRadius = 0;
        private const double constMaximumInnerDialRadius = 75;

        public double InnerDialRadius
        {
            get
            {
                return (double)GetValue(InnerDialRadiusProperty);
            }
            set
            {
                SetValue(InnerDialRadiusProperty, Math.Min(Math.Max(value, constMinimumInnerDialRadius), constMaximumInnerDialRadius));
            }
        }

        private static void OnInnerDialRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnInnerDialRadiusChanged(e);
        }

        protected virtual void OnInnerDialRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                InnerDialRadius = (double)e.NewValue;
                CreateControl();
            }
        }

        #endregion

        #region InnerDialFill dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty InnerDialFillProperty = DependencyProperty.Register("InnerDialFill", typeof(Brush), typeof(RotaryControl), new FrameworkPropertyMetadata(DefaultInnerDialFill(), new PropertyChangedCallback(OnInnerDialFillChanged)));

        public Brush InnerDialFill
        {
            get
            {
                return (Brush)GetValue(InnerDialFillProperty);
            }
            set
            {
                SetValue(InnerDialFillProperty, value);
                _ellipseInnerDial.Fill = value;
            }
        }

        private static void OnInnerDialFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnInnerDialFillChanged(e);
        }

        protected virtual void OnInnerDialFillChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                InnerDialFill = (Brush)e.NewValue;
            }
        }

        #endregion

        #region OuterDialFill dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty OuterDialFillProperty = DependencyProperty.Register("OuterDialFill", typeof(Brush), typeof(RotaryControl), new FrameworkPropertyMetadata(Brushes.SteelBlue, new PropertyChangedCallback(OnOuterDialFillChanged)));

        public Brush OuterDialFill
        {
            get
            {
                return (Brush)GetValue(OuterDialFillProperty);
            }
            set
            {
                SetValue(OuterDialFillProperty, value);
                _ellipseOuterDial.Fill = value;
            }
        }

        private static void OnOuterDialFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnOuterDialFillChanged(e);
        }

        protected virtual void OnOuterDialFillChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                OuterDialFill = (Brush)e.NewValue;
            }
        }

        #endregion

        #region SegmentThickness dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SegmentThicknessProperty = DependencyProperty.Register("SegmentThickness", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnSegmentThicknessChanged)));

        public double SegmentThickness
        {
            get
            {
                return (double)GetValue(SegmentThicknessProperty);
            }
            set
            {
                if ((value >= InnerDialRadius) && (value <= _ellipseOuterDial.Width))
                {
                    SetValue(SegmentThicknessProperty, value);
                    CreateControl();
                }
            }
        }

        private static void OnSegmentThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnSegmentThicknessChanged(e);
        }

        protected virtual void OnSegmentThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                SegmentThickness = (double)e.NewValue;
            }
        }

        #endregion

        #region SegmentRadius dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SegmentRadiusProperty = DependencyProperty.Register("SegmentRadius", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnSegmentRadiusChanged)));

        public double SegmentRadius
        {
            get
            {
                return (double)GetValue(SegmentRadiusProperty);
            }
            set
            {
                SetValue(SegmentRadiusProperty, value);
                CreateControl();
            }
        }

        private static void OnSegmentRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnSegmentRadiusChanged(e);
        }

        protected virtual void OnSegmentRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                SegmentRadius = (double)e.NewValue;
            }
        }

        #endregion

        #region Segments dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SegmentsProperty = DependencyProperty.Register("Segments", typeof(System.Collections.IEnumerable), typeof(RotaryControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSegmentsChanged)));

        public System.Collections.IEnumerable Segments
        {
            get
            {
                return (System.Collections.IEnumerable)GetValue(SegmentsProperty);
            }
            set
            {
                SetValue(SegmentsProperty, value);
                CreateControl();
            }
        }

        private static void OnSegmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnSegmentsChanged(e);
        }

        protected virtual void OnSegmentsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Segments = (System.Collections.IEnumerable)e.NewValue;
            }
        }

        #endregion

        #region Arcs dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ArcsProperty = DependencyProperty.Register("Arcs", typeof(System.Collections.IEnumerable), typeof(RotaryControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnArcsChanged)));

        public System.Collections.IEnumerable Arcs
        {
            get
            {
                return (System.Collections.IEnumerable)GetValue(ArcsProperty);
            }
            set
            {
                SetValue(ArcsProperty, value);
                CreateControl();
            }
        }

        private static void OnArcsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnArcsChanged(e);
        }

        protected virtual void OnArcsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Arcs = (System.Collections.IEnumerable)e.NewValue;
            }
        }

        #endregion

        #region OuterDialBorder dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty OuterDialBorderProperty = DependencyProperty.Register("OuterDialBorder", typeof(Brush), typeof(RotaryControl), new FrameworkPropertyMetadata(Brushes.Gainsboro, new PropertyChangedCallback(OnOuterDialBorderChanged)));

        public Brush OuterDialBorder
        {
            get
            {
                return (Brush)GetValue(OuterDialBorderProperty);
            }
            set
            {
                SetValue(OuterDialBorderProperty, value);
                _ellipseOuterDial.Stroke = value;
            }
        }

        private static void OnOuterDialBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnOuterDialBorderChanged(e);
        }

        protected virtual void OnOuterDialBorderChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                OuterDialBorder = (Brush)e.NewValue;
            }
        }

        #endregion

        #region OuterDialBorderThickness dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty OuterDialBorderThicknessProperty = DependencyProperty.Register("OuterDialBorderThickness", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(4.0, new PropertyChangedCallback(OnOuterDialBorderThicknessChanged)));

        private const double constMinimumOuterDialBorderThickness = 0.0;
        private const double constMaximumOuterDialBorderThickness = 30.0;

        public double OuterDialBorderThickness
        {
            get
            {
                return (double)GetValue(OuterDialBorderThicknessProperty);
            }
            set
            {
                SetValue(OuterDialBorderThicknessProperty, Math.Min(Math.Max(value, constMinimumOuterDialBorderThickness), constMaximumOuterDialBorderThickness));
            }
        }

        private static void OnOuterDialBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnOuterDialBorderThicknessChanged(e);
        }

        protected virtual void OnOuterDialBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                OuterDialBorderThickness = (double)e.NewValue;
                CreateControl();
            }
        }

        #endregion

        #region PointerAxleFill dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty PointerAxleFillProperty = DependencyProperty.Register("PointerAxleFill", typeof(Brush), typeof(RotaryControl), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnPointerAxleFillChanged)));

        public Brush PointerAxleFill
        {
            get
            {
                return (Brush)GetValue(PointerAxleFillProperty);
            }
            set
            {
                SetValue(PointerAxleFillProperty, value);
                _pointerAxle.Fill = PointerAxleFill;
            }
        }

        private static void OnPointerAxleFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnPointerAxleFillChanged(e);
        }

        protected virtual void OnPointerAxleFillChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                PointerAxleFill = (Brush)e.NewValue;
            }
        }

        #endregion

        #region LabelDialRadius dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty LabelDialRadiusProperty = DependencyProperty.Register("LabelDialRadius", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnLabelDialRadiusChanged)));

        public double LabelDialRadius
        {
            get
            {
                return (double)GetValue(LabelDialRadiusProperty);
            }
            set
            {
                SetValue(LabelDialRadiusProperty, value);
            }
        }

        private static void OnLabelDialRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnLabelDialRadiusChanged(e);
        }

        protected virtual void OnLabelDialRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                LabelDialRadius = (double)e.NewValue;
                CreateControl();
            }
        }

        #endregion

        #region PointerLength dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty PointerLengthProperty = DependencyProperty.Register("PointerLength", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnPointerLengthChanged)));

        public double PointerLength
        {
            get
            {
                return (double)GetValue(PointerLengthProperty);
            }
            set
            {
                SetValue(PointerLengthProperty, value);
                CreateControl();
            }
        }

        private static void OnPointerLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnPointerLengthChanged(e);
        }

        protected virtual void OnPointerLengthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                PointerLength = (double)e.NewValue;
            }
        }

        #endregion

        #region PointerAxleRadius dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty PointerAxleRadiusProperty = DependencyProperty.Register("PointerAxleRadius", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(3.0, new PropertyChangedCallback(OnPointerAxleRadiusChanged)));

        public double PointerAxleRadius
        {
            get
            {
                return (double)GetValue(PointerAxleRadiusProperty);
            }
            set
            {
                SetValue(PointerAxleRadiusProperty, value);

                _pointerPathFigure.StartPoint = new Point(100.0, 100.0 - value);
                _pointerAxleArc1.Size = new Size(value, value);
                _pointerAxleArc1.Point = new Point(100, 100 + value);
                _pointerAxleArc2.Size = new Size(value, value);
                _pointerAxleArc2.Point = new Point(100, 100 - value);
            }
        }

        private static void OnPointerAxleRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnPointerAxleRadiusChanged(e);
        }

        protected virtual void OnPointerAxleRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                PointerAxleRadius = (double)e.NewValue;
            }
        }

        #endregion

        #region PointerWidth dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty PointerWidthProperty = DependencyProperty.Register("PointerWidth", typeof(double), typeof(RotaryControl), new FrameworkPropertyMetadata(4.0, new PropertyChangedCallback(OnPointerWidthChanged)));

        public double PointerWidth
        {
            get
            {
                return (double)GetValue(PointerWidthProperty);
            }
            set
            {
                SetValue(PointerWidthProperty, value);
                CreateControl();
            }
        }

        private static void OnPointerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnPointerWidthChanged(e);
        }

        protected virtual void OnPointerWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                PointerWidth = (double)e.NewValue;
                _pointerTopLeft.Point = new Point(_pointerTopLeft.Point.X, _pointerTip.Point.Y - PointerWidth / 2);
                _pointerTopRight.Point = new Point(_pointerTopRight.Point.X, _pointerTip.Point.Y - PointerWidth / 2);
                _pointerBottomLeft.Point = new Point(_pointerBottomLeft.Point.X, _pointerTip.Point.Y + PointerWidth / 2);
                _pointerBottomRight.Point = new Point(_pointerBottomRight.Point.X, _pointerTip.Point.Y + PointerWidth / 2);
            }
        }

        #endregion

        #region PointerFill dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty PointerFillProperty = DependencyProperty.Register("PointerFill", typeof(Brush), typeof(RotaryControl), new FrameworkPropertyMetadata(DefaultPointerFill(), new PropertyChangedCallback(OnPointerFillChanged)));

        public Brush PointerFill
        {
            get
            {
                return (Brush)GetValue(PointerFillProperty);
            }
            set
            {
                SetValue(PointerFillProperty, value);
                _pointerCircle.Fill = PointerFill;
                _pointerStandard.Fill = PointerFill;
                _pointerRectangle.Fill = PointerFill;
                _pointerArrow.Fill = PointerFill;
            }
        }

        private static void OnPointerFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnPointerFillChanged(e);
        }

        protected virtual void OnPointerFillChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                PointerFill = (Brush)e.NewValue;
            }
        }

        #endregion

        #region PointerType dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty PointerTypeProperty = DependencyProperty.Register("PointerType", typeof(string), typeof(RotaryControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPointerTypeChanged)));

        public string PointerType
        {
            get
            {
                return (string)GetValue(PointerTypeProperty);
            }
            set
            {
                SetValue(PointerTypeProperty, value);
                CreateControl();
            }
        }

        private static void OnPointerTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotaryControl)d).OnPointerTypeChanged(e);
        }

        protected virtual void OnPointerTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                PointerType = ((string)e.NewValue).ToLower();
            }
        }

        #endregion

        #endregion dependency properties 

        #region event handlers

        private void RotaryControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_ellipseOuterDial.InputHitTest(e.GetPosition(_ellipseOuterDial)) == null)
            {
                e.Handled = false;
                return;
            }
        }

        private void RotaryControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_ellipseOuterDial.InputHitTest(e.GetPosition(_ellipseOuterDial)) == null)
            {
                e.Handled = false;
                return;
            }
        }

        private void RotaryControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_ellipseOuterDial.InputHitTest(e.GetPosition(_ellipseOuterDial)) == null)
            {
                e.Handled = false;  
                return;
            }

            PositionMarkerFromControlPosition(e.GetPosition(_ellipseOuterDial));

            _ellipseOuterDial.CaptureMouse();
        }

        private void RotaryControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_ellipseOuterDial.IsMouseCaptured)
            {
                PositionMarkerFromControlPosition(e.GetPosition(_ellipseOuterDial));

                _ellipseOuterDial.CaptureMouse();
            }
        }

        private void RotaryControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_ellipseOuterDial.IsMouseCaptured)
            {
                _ellipseOuterDial.ReleaseMouseCapture();
            }
        }

        #endregion event handlers
    }
}
