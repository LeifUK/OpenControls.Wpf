namespace OpenControls.Wpf.DatabaseDialogs.Model
{
    public interface IConfigurationSerialiser
    {
        void WriteEntry<T>(string name, T value);
        T ReadEntry<T>(string name, T defaultValue);
    }
}
