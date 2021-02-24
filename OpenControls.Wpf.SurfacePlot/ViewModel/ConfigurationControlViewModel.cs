using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenControls.Wpf.SurfacePlot.ViewModel
{
    public class ConfigurationControlViewModel : OpenControls.Wpf.Utilities.ViewModel.BaseViewModel
    {
        public ConfigurationControlViewModel(Model.IConfiguration iConfiguration)
        {
            IConfiguration = iConfiguration;
            IConfiguration.ConfigurationChanged += ConfigurationChangedEventHandler;

            FontSizes = new System.Collections.ObjectModel.ObservableCollection<int>(new List<int> { 8, 10, 12, 14, 16, 18, 20, 24, 28, 32, 36, 40, 50, 60 });
            LabelAngles = new System.Collections.ObjectModel.ObservableCollection<int> { 0, 30, 60, 90 };
            ShadingMethods = new System.Collections.ObjectModel.ObservableCollection<Model.ShadingMethod>((Model.ShadingMethod[])Enum.GetValues(typeof(Model.ShadingMethod)));
            ViewProjectionTypes = new System.Collections.ObjectModel.ObservableCollection<ViewProjectionType>
            {
                new ViewProjectionType() { ViewProjection = Model.ViewProjection.ThreeDimensional, Name = "3D" },
                new ViewProjectionType() { ViewProjection = Model.ViewProjection.BirdsEye_0, Name = "Birds Eye - 0 Degrees" },
                new ViewProjectionType() { ViewProjection = Model.ViewProjection.BirdsEye_90, Name = "Birds Eye - 90 Degrees" },
                new ViewProjectionType() { ViewProjection = Model.ViewProjection.BirdsEye_180, Name = "Birds Eye - 180 Degrees" },
                new ViewProjectionType() { ViewProjection = Model.ViewProjection.BirdsEye_270, Name = "Birds Eye - 270 Degrees" },
                new ViewProjectionType() { ViewProjection = Model.ViewProjection.Orthographic_Front, Name = "Orthographic - Front" },
                new ViewProjectionType() { ViewProjection = Model.ViewProjection.Orthographic_Side, Name = "Orthographic - Side" },
            };
            ViewProjection = IConfiguration.ViewProjection;
            XYLabelPositions = new System.Collections.ObjectModel.ObservableCollection<Model.XYLabelPosition>() { Model.XYLabelPosition.Middle, Model.XYLabelPosition.Bottom };
        }

        private readonly Model.IConfiguration IConfiguration;

        public string BackgroundColour
        {
            get
            {
                return IConfiguration.BackgroundColour;
            }
            set
            {
                IConfiguration.BackgroundColour = value;
                NotifyPropertyChanged("BackgroundColour");
            }
        }

        public bool ShowAxes
        {
            get
            {
                return IConfiguration.ShowAxes;
            }
            set
            {
                IConfiguration.ShowAxes = value;
                NotifyPropertyChanged("ShowAxes");
            }
        }

        public bool ShowAxesTitles
        {
            get
            {
                return IConfiguration.ShowAxesTitles;
            }
            set
            {
                IConfiguration.ShowAxesTitles = value;
                NotifyPropertyChanged("ShowAxesTitles");
            }
        }

        public bool ShowZBar
        {
            get
            {
                return IConfiguration.ShowZBar;
            }
            set
            {
                IConfiguration.ShowZBar = value;
                NotifyPropertyChanged("ShowZBar");
            }
        }

        public bool ShowFrame
        {
            get
            {
                return IConfiguration.ShowFrame;
            }
            set
            {
                IConfiguration.ShowFrame = value;
                NotifyPropertyChanged("ShowFrame");
            }
        }

        public string FrameColour
        {
            get
            {
                return IConfiguration.FrameColour;
            }
            set
            {
                IConfiguration.FrameColour = value;
                NotifyPropertyChanged("FrameColour");
            }
        }

        public bool ShowLabels
        {
            get
            {
                return IConfiguration.ShowLabels;
            }
            set
            {
                IConfiguration.ShowLabels = value;
                NotifyPropertyChanged("ShowLabels");
            }
        }

        public string LabelColour
        {
            get
            {
                return IConfiguration.LabelColour;
            }
            set
            {
                IConfiguration.LabelColour = value;
                NotifyPropertyChanged("LabelColour");
            }
        }

        public int LabelFontSize
        {
            get
            {
                return IConfiguration.LabelFontSize;
            }
            set
            {
                IConfiguration.LabelFontSize = value;
                NotifyPropertyChanged("LabelFontSize");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<int> _labelAngles;
        public System.Collections.ObjectModel.ObservableCollection<int> LabelAngles
        {
            get
            {
                return _labelAngles;
            }
            set
            {
                _labelAngles = value;
                NotifyPropertyChanged("LabelAngles");
            }
        }

        public int LabelAngleInDegrees
        {
            get
            {
                return IConfiguration.LabelAngleInDegrees;
            }
            set
            {
                IConfiguration.LabelAngleInDegrees = value;
                NotifyPropertyChanged("LabelAngle");
            }
        }

        public bool TransparentLabelBackground
        {
            get
            {
                return IConfiguration.TransparentLabelBackground;
            }
            set
            {
                IConfiguration.TransparentLabelBackground = value;
                NotifyPropertyChanged("TransparentLabelBackground");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<Model.XYLabelPosition> _xyYLabelPositions;
        public System.Collections.ObjectModel.ObservableCollection<Model.XYLabelPosition> XYLabelPositions
        {
            get
            {
                return _xyYLabelPositions;
            }
            set
            {
                _xyYLabelPositions = value;
                NotifyPropertyChanged("XYLabelPositions");
            }
        }

        public Model.XYLabelPosition XYLabelPosition
        {
            get
            {
                return IConfiguration.XYLabelPosition;
            }
            set
            {
                IConfiguration.XYLabelPosition = value;
                NotifyPropertyChanged("XYLabelPosition");
            }
        }

        // Scales the Z values according to user preferences
        public double ZScale
        {
            get
            {
                return Math.Log(10000 * IConfiguration.ZScale, 2);
            }
            set
            {
                IConfiguration.ZScale = (float)(0.0001 * Math.Pow(2, value));
                NotifyPropertyChanged("ZScale");
            }
        }

        // A simple number to zoom in and out: initial value is sensible with TouchHub2
        public int Zoom
        {
            get
            {
                return IConfiguration.Zoom;
            }
            set
            {
                IConfiguration.Zoom = value;
                NotifyPropertyChanged("Zoom");
            }
        }

        public int MinimumZoom
        {
            get
            {
                return IConfiguration.MinimumZoom;
            }
        }

        public int MaximumZoom
        {
            get
            {
                return IConfiguration.MaximumZoom;
            }
        }

        // A simple scaling of the field of view: minimum 1
        public float Perspective
        {
            get
            {
                return IConfiguration.Perspective;
            }
            set
            {
                IConfiguration.Perspective = value;
                NotifyPropertyChanged("Perspective");
            }
        }

        public float PerspectiveSlider
        {
            get
            {
                return Perspective;
            }
            set
            {
                Perspective = value;
                NotifyPropertyChanged("PerspectiveSlider");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<int> _fontSizes;
        public System.Collections.ObjectModel.ObservableCollection<int> FontSizes
        {
            get
            {
                return _fontSizes;
            }
            set
            {
                _fontSizes = value;
                NotifyPropertyChanged("FontSizes");
            }
        }

        public int FontSize
        {
            get
            {
                return IConfiguration.LabelFontSize;
            }
            set
            {
                IConfiguration.LabelFontSize = value;
                NotifyPropertyChanged("LabelFontSize");
            }
        }

        public bool ShowGrid
        {
            get
            {
                return IConfiguration.ShowGrid;
            }
            set
            {
                IConfiguration.ShowGrid = value;
                NotifyPropertyChanged("ShowGrid");
            }
        }

        public string GridColour
        {
            get
            {
                return IConfiguration.GridColour;
            }
            set
            {
                IConfiguration.GridColour = value;
                NotifyPropertyChanged("GridColour");
            }
        }

        public bool ShowScatterPlot
        {
            get
            {
                return IConfiguration.ShowScatterPlot;
            }
            set
            {
                IConfiguration.ShowScatterPlot = value;
                NotifyPropertyChanged("ShowScatterPlot");
                if (value)
                {
                    ShowShading = false;
                }
            }
        }

        public bool ShowShading
        {
            get
            {
                return IConfiguration.ShowShading;
            }
            set
            {
                IConfiguration.ShowShading = value;
                NotifyPropertyChanged("ShowShading");
                if (value)
                {
                    ShowScatterPlot = false;
                }
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<Model.ShadingMethod> _shadingMethods;
        public System.Collections.ObjectModel.ObservableCollection<Model.ShadingMethod> ShadingMethods
        {
            get
            {
                return _shadingMethods;
            }
            set
            {
                _shadingMethods = value;
                NotifyPropertyChanged("ShadingMethods");
            }
        }

        public Model.ShadingMethod ShadingMethod
        {
            get
            {
                return IConfiguration.ShadingMethod;
            }
            set
            {
                IConfiguration.ShadingMethod = value;
                NotifyPropertyChanged("ShadingMethod");
            }
        }

        public Model.ShadingAlgorithm ShadingAlgorithm
        {
            get
            {
                return IConfiguration.ShadingAlgorithm;
            }
            set
            {
                IConfiguration.ShadingAlgorithm = value;
                NotifyPropertyChanged("ShadingAlgorithm");
            }
        }

        // The z value below which all vertices are painted blue
        public short BlueLevel
        {
            get
            {
                return IConfiguration.BlueLevel;
            }
            set
            {
                if ((value <= 0) && (value > -32768))
                {
                    IConfiguration.BlueLevel = value;
                    NotifyPropertyChanged("BlueLevel");
                }
            }
        }

        // The z value above which all vertices are painted red
        public short RedLevel
        {
            get
            {
                return IConfiguration.RedLevel;
            }
            set
            {
                if ((value >= 0) && (value < 32767))
                {
                    IConfiguration.RedLevel = value;
                    NotifyPropertyChanged("RedLevel");
                }
            }
        }

        public short BlueLevelSlider
        {
            get
            {
                return (short)-Math.Log(-BlueLevel, 2);
            }
            set
            {
                if (value == 0)
                {
                    BlueLevel = 0;
                }
                else
                {
                    BlueLevel = (short)-(1 << -value);
                }
                NotifyPropertyChanged("BlueLevelSlider");
            }
        }

        public short RedLevelSlider
        {
            get
            {
                return (short)Math.Log(RedLevel, 2);
            }
            set
            {
                if (value == 0)
                {
                    RedLevel = 0;
                }
                else
                {
                    RedLevel = (short)(1 << value);
                }
                NotifyPropertyChanged("RedLevelSlider");
            }
        }

        public bool Hold
        {
            get
            {
                return IConfiguration.Hold;
            }
            set
            {
                IConfiguration.Hold = value;
                NotifyPropertyChanged("Hold");
            }
        }

        public bool HoldMaximum
        {
            get
            {
                return IConfiguration.HoldMaximum;
            }
            set
            {
                IConfiguration.HoldMaximum = value;
                NotifyPropertyChanged("HoldMaximum");
            }
        }

        public class ViewProjectionType
        {
            public string Name { get; set; }
            public Model.ViewProjection ViewProjection { get; set; }
        }

        public ViewProjectionType _selectedViewProjectionType;
        public ViewProjectionType SelectedViewProjectionType
        {
            get
            {
                return _selectedViewProjectionType;
            }
            set
            {
                _selectedViewProjectionType = value;
                IConfiguration.ViewProjection = value.ViewProjection;
                NotifyPropertyChanged("SelectedViewProjectionType");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<ViewProjectionType> _viewProjectionTypes;
        public System.Collections.ObjectModel.ObservableCollection<ViewProjectionType> ViewProjectionTypes
        {
            get
            {
                return _viewProjectionTypes;
            }
            set
            {
                _viewProjectionTypes = value;
                NotifyPropertyChanged("ViewProjectionTypes");
            }
        }

        public Model.ViewProjection ViewProjection
        {
            get
            {
                return SelectedViewProjectionType != null ? SelectedViewProjectionType.ViewProjection : Model.ViewProjection.ThreeDimensional;
            }
            set
            {
                foreach (var item in ViewProjectionTypes)
                {
                    if (item.ViewProjection == value)
                    {
                        SelectedViewProjectionType = item;
                        IConfiguration.ViewProjection = value;
                        break;
                    }
                }
            }
        }

        #region Model.ConfigurationChangedEventHandler

        public void ConfigurationChangedEventHandler(Model.ConfigurationItem item)
        {
            switch (item)
            {
                case Model.ConfigurationItem.Zoom:
                    NotifyPropertyChanged("Zoom");
                    break;
                case Model.ConfigurationItem.ZScale:
                    NotifyPropertyChanged("ZScale");
                    break;
                case Model.ConfigurationItem.ShowFrame:
                    NotifyPropertyChanged("ShowFrame");
                    break;
                case Model.ConfigurationItem.ShowGrid:
                    NotifyPropertyChanged("ShowGrid");
                    break;
                case Model.ConfigurationItem.ShowShading:
                    NotifyPropertyChanged("ShowShading");
                    break;
                case Model.ConfigurationItem.ShadingMethod:
                    NotifyPropertyChanged("ShadingMethod");
                    break;
                case Model.ConfigurationItem.ShowLabels:
                    NotifyPropertyChanged("ShowLabels");
                    break;
            }
        }

        #endregion Model.ConfigurationChangedEventHandler
    }
}
