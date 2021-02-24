namespace OpenControls.Wpf.SurfacePlot.Model
{
    public interface IConfigurationSerialiser
    {
        void WriteEntry<T>(string key, T value);
        T ReadEntry<T>(string key, T value);
    }
}
