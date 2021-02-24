namespace OpenControls.Wpf.SurfacePlot.Model
{
    public interface IConfiguration
    {
        // Signalled whenever a configuration setting is changed
        event Model.ConfigurationChangedEventHandler ConfigurationChanged;

        void Load(IConfigurationSerialiser configurationSerialiser);
        void Save(IConfigurationSerialiser configurationSerialiser);

        int Zoom { get; set; }
        int MaximumZoom { get; }
        int MinimumZoom { get; }
        double ZScale { get; set; }
        string BackgroundColour { get; set; }
        bool ShowAxes { get; set; }
        bool ShowAxesTitles { get; set; }
        bool ShowZBar { get; set; }
        bool ShowFrame { get; set; }
        string FrameColour { get; set; }
        bool ShowLabels { get; set; }
        string LabelColour { get; set; }
        int LabelFontSize { get; set; }
        int LabelAngleInDegrees { get; set; }
        bool TransparentLabelBackground { get; set; }
        XYLabelPosition XYLabelPosition { get; set; }
        float Perspective { get; set; }
        ViewProjection ViewProjection { get; set; }
        ShadingMethod ShadingMethod { get; set; }
        bool ShowGrid { get; set; }
        string GridColour { get; set; }
        bool ShowScatterPlot { get; set; }
        bool ShowShading { get; set; }
        ShadingAlgorithm ShadingAlgorithm { get; set; }
        short BlueLevel { get; set; }
        short RedLevel { get; set; }
        bool Hold { get; set; }
        bool HoldMaximum { get; set; }
    }
}
