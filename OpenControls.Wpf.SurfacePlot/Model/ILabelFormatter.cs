namespace OpenControls.Wpf.SurfacePlot.Model
{
    public interface ILabelFormatter
    {
        string XLabel(float x);
        string YLabel(float y);
        string ZLabel(float z);
    }
}
