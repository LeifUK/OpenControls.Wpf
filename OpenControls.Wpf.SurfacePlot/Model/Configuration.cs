namespace OpenControls.Wpf.SurfacePlot.Model
{
    public class Configuration : IConfiguration
    {
        public Configuration()
        {
            Zoom = 100;
            ZScale = 0.002f;
            ShadingAlgorithm = Model.ShadingAlgorithm.FixedLevels;
            BlueLevel = -64;
            RedLevel = 2048;
            Perspective = 1.0f;
            BackgroundColour = "Black";
            ShowScatterPlot = false;
            ShowShading = true;
            ShadingMethod = ShadingMethod.Interpolated;
            LabelFontSize = 10;
            LabelColour = "White";
            ShowLabels = true;
            XYLabelPosition = XYLabelPosition.Bottom;
            GridColour = "White";
            ShowGrid = true;
            FrameColour = "White";
            ShowAxes = true;
            ShowAxesTitles = true;
            ShowZBar = true;
            ShowFrame = true;
            Hold = false;
            HoldMaximum = true;
            ViewProjection = ViewProjection.ThreeDimensional;
        }

        #region IRawDataConfiguration

        public void Load(OpenControls.Wpf.Serialisation.IConfigurationSerialiser configurationSerialiser)
        {
            Zoom = configurationSerialiser.ReadEntry("Zoom", Zoom);
            ZScale = configurationSerialiser.ReadEntry("ZScale", ZScale);
            BackgroundColour = configurationSerialiser.ReadEntry("BackgroundColour", string.IsNullOrEmpty(BackgroundColour) ? "Black" : BackgroundColour);
            ShowAxes = configurationSerialiser.ReadEntry("ShowAxes", ShowAxes);
            ShowAxesTitles = configurationSerialiser.ReadEntry("ShowAxesTitles", ShowAxesTitles);
            ShowZBar = configurationSerialiser.ReadEntry("ShowZBar", ShowZBar);
            ShowFrame = configurationSerialiser.ReadEntry("ShowFrame", ShowFrame);
            FrameColour = configurationSerialiser.ReadEntry("FrameColour", string.IsNullOrEmpty(FrameColour) ? "White" : FrameColour);
            ShowLabels = configurationSerialiser.ReadEntry("ShowLabels", ShowLabels);
            LabelColour = configurationSerialiser.ReadEntry("LabelColour", string.IsNullOrEmpty(LabelColour) ? "White" : LabelColour);
            LabelFontSize = configurationSerialiser.ReadEntry("LabelFontSize", LabelFontSize);
            LabelAngleInDegrees = configurationSerialiser.ReadEntry("LabelAngleInDegrees", LabelAngleInDegrees);
            XYLabelPosition = configurationSerialiser.ReadEntry<XYLabelPosition>("XYLabelPosition", XYLabelPosition);
            Perspective = configurationSerialiser.ReadEntry("Perspective", Perspective);
            ViewProjection = configurationSerialiser.ReadEntry<ViewProjection>("ViewProjection", ViewProjection);
            ShowGrid = configurationSerialiser.ReadEntry("ShowGrid", ShowGrid);
            GridColour = configurationSerialiser.ReadEntry("GridColour", string.IsNullOrEmpty(GridColour) ? "White" : GridColour);
            ShowScatterPlot = configurationSerialiser.ReadEntry("ShowScatterPlot", ShowScatterPlot);
            ShowShading = configurationSerialiser.ReadEntry("ShowShading", ShowShading);
            ShadingMethod = configurationSerialiser.ReadEntry<ShadingMethod>("ShadingMethod", ShadingMethod);
            ShadingAlgorithm = configurationSerialiser.ReadEntry<ShadingAlgorithm>("ShadingAlgorithm", ShadingAlgorithm);
            BlueLevel = configurationSerialiser.ReadEntry("BlueLevel", BlueLevel);
            RedLevel = configurationSerialiser.ReadEntry("RedLevel", RedLevel);
        }

        public void Save(OpenControls.Wpf.Serialisation.IConfigurationSerialiser configurationSerialiser)
        {
            configurationSerialiser.WriteEntry("Zoom", Zoom);
            configurationSerialiser.WriteEntry("ZScale", ZScale);
            configurationSerialiser.WriteEntry("BackgroundColour", string.IsNullOrEmpty(BackgroundColour) ? "Black" : BackgroundColour);
            configurationSerialiser.WriteEntry("ShowAxes", ShowAxes);
            configurationSerialiser.WriteEntry("ShowAxesTitles", ShowAxesTitles);
            configurationSerialiser.WriteEntry("ShowZBar", ShowZBar);
            configurationSerialiser.WriteEntry("ShowFrame", ShowFrame);
            configurationSerialiser.WriteEntry("FrameColour", string.IsNullOrEmpty(FrameColour) ? "White" : FrameColour);
            configurationSerialiser.WriteEntry("ShowLabels", ShowLabels);
            configurationSerialiser.WriteEntry("LabelColour", string.IsNullOrEmpty(LabelColour) ? "White" : LabelColour);
            configurationSerialiser.WriteEntry("LabelFontSize", LabelFontSize);
            configurationSerialiser.WriteEntry("LabelAngleInDegrees", LabelAngleInDegrees);
            configurationSerialiser.WriteEntry("XYLabelPosition", XYLabelPosition);
            configurationSerialiser.WriteEntry("Perspective", Perspective);
            configurationSerialiser.WriteEntry("ViewProjection", ViewProjection);
            configurationSerialiser.WriteEntry("ShowGrid", ShowGrid);
            configurationSerialiser.WriteEntry("GridColour", string.IsNullOrEmpty(GridColour) ? "White" : GridColour);
            configurationSerialiser.WriteEntry("ShowScatterPlot", ShowScatterPlot);
            configurationSerialiser.WriteEntry("ShowShading", ShowShading);
            configurationSerialiser.WriteEntry("ShadingMethod", ShadingMethod);
            configurationSerialiser.WriteEntry("ShadingAlgorithm", ShadingAlgorithm);
            configurationSerialiser.WriteEntry("BlueLevel", BlueLevel);
            configurationSerialiser.WriteEntry("RedLevel", RedLevel);
        }

        public event ConfigurationChangedEventHandler ConfigurationChanged;

        private int _zoom;
        public int Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                if (value > MaximumZoom)
                {
                    value = MaximumZoom;
                }
                else if (value < MinimumZoom)
                {
                    value = MinimumZoom;
                }
                _zoom = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.Zoom);
            }
        }

        public int MaximumZoom
        {
            get
            {
                return 400;
            }
        }

        public int MinimumZoom
        {
            get
            {
                return 1;
            }
        }

        private double _zScale;
        public double ZScale
        {
            get
            {
                return _zScale;
            }
            set
            {
                _zScale = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ZScale);
            }
        }

        private string _backgroundColour;
        public string BackgroundColour
        {
            get
            {
                return _backgroundColour;
            }
            set
            {
                _backgroundColour = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.BackgroundColour);
            }
        }

        private bool _showAxes;
        public bool ShowAxes
        {
            get
            {
                return _showAxes;
            }
            set
            {
                _showAxes = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ShowAxes);
            }
        }

        private bool _showAxesTitles;
        public bool ShowAxesTitles
        {
            get
            {
                return _showAxesTitles;
            }
            set
            {
                _showAxesTitles = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ShowAxesTitles);
            }
        }

        private bool _showZBar;
        public bool ShowZBar
        {
            get
            {
                return _showZBar;
            }
            set
            {
                _showZBar = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ShowZBar);
            }
        }

        private bool _showFrame;
        public bool ShowFrame
        {
            get
            {
                return _showFrame;
            }
            set
            {
                _showFrame = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ShowFrame);
            }
        }

        private string _frameColour;
        public string FrameColour
        {
            get
            {
                return _frameColour;
            }
            set
            {
                _frameColour = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.FrameColour);
            }
        }

        private bool _showLabels;
        public bool ShowLabels
        {
            get
            {
                return _showLabels;
            }
            set
            {
                _showLabels = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ShowLabels);
            }
        }

        private string _labelColour;
        public string LabelColour
        {
            get
            {
                return _labelColour;
            }
            set
            {
                _labelColour = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.LabelColour);
            }
        }

        private int _labelFontSize;
        public int LabelFontSize
        {
            get
            {
                return _labelFontSize;
            }
            set
            {
                _labelFontSize = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.LabelFontSize);
            }
        }

        private int _labelAngleInDegrees;
        public int LabelAngleInDegrees
        {
            get
            {
                return _labelAngleInDegrees;
            }
            set
            {
                _labelAngleInDegrees = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.LabelAngleInDegrees);
            }
        }

        private Model.XYLabelPosition _xyLabelPosition;
        public Model.XYLabelPosition XYLabelPosition
        {
            get
            {
                return _xyLabelPosition;
            }
            set
            {
                _xyLabelPosition = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.XYLabelPosition);
            }
        }

        private bool _transparentLabelBackground;
        public bool TransparentLabelBackground
        {
            get
            {
                return _transparentLabelBackground;
            }
            set
            {
                _transparentLabelBackground = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.TransparentLabelBackground);
            }
        }

        private float _perspective;
        public float Perspective
        {
            get
            {
                return _perspective;
            }
            set
            {
                _perspective = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.Perspective);
            }
        }

        private ViewProjection _viewProjection;
        public ViewProjection ViewProjection
        {
            get
            {
                return _viewProjection;
            }
            set
            {
                _viewProjection = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ViewProjection);
            }
        }

        private bool _showGrid;
        public bool ShowGrid
        {
            get
            {
                return _showGrid;
            }
            set
            {
                _showGrid = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ShowGrid);
            }
        }

        private string _gridColour;
        public string GridColour
        {
            get
            {
                return _gridColour;
            }
            set
            {
                _gridColour = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.GridColour);
            }
        }

        private bool _showScatterPlot;
        public bool ShowScatterPlot
        {
            get
            {
                return _showScatterPlot;
            }
            set
            {
                _showScatterPlot = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ShowScatterPlot);
            }
        }

        private bool _showShading;
        public bool ShowShading
        {
            get
            {
                return _showShading;
            }
            set
            {
                _showShading = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ShowShading);
            }
        }

        private ShadingMethod _shadingMethod;
        public ShadingMethod ShadingMethod
        {
            get
            {
                return _shadingMethod;
            }
            set
            {
                _shadingMethod = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ShadingMethod);
            }
        }

        private ShadingAlgorithm _shadingAlgorithm;
        public ShadingAlgorithm ShadingAlgorithm
        {
            get
            {
                return _shadingAlgorithm;
            }
            set
            {
                _shadingAlgorithm = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.ShadingAlgorithm);
            }
        }

        private short _blueLevel;
        public short BlueLevel
        {
            get
            {
                return _blueLevel;
            }
            set
            {
                _blueLevel = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.BlueLevel);
            }
        }

        private short _redLevel;
        public short RedLevel
        {
            get
            {
                return _redLevel;
            }
            set
            {
                _redLevel = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.RedLevel);
            }
        }

        // If Hold is true, display the maximum value if HoldMaximum is true and the minimum otherwise

        private bool _hold;
        public bool Hold
        {
            get
            {
                return _hold;
            }
            set
            {
                _hold = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.Hold);
            }
        }

        private bool _holdMaximum;
        public bool HoldMaximum
        {
            get
            {
                return _holdMaximum;
            }
            set
            {
                _holdMaximum = value;
                ConfigurationChanged?.Invoke(ConfigurationItem.HoldMaximum);
            }
        }

        #endregion IRawDataConfiguration
    }

}
