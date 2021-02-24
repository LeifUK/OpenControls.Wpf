using System;

namespace OpenControls.Wpf.SurfacePlot.Model
{
    public class ConfigurationSerialiser : IConfigurationSerialiser
    {
        public static T LoadRegistryEntry<T>(Microsoft.Win32.RegistryKey key, T value, string valueName)
        {
            if (key != null)
            {
                object obj = key.GetValue(valueName);
                if (obj != null)
                {
                    value = (T)Convert.ChangeType(obj, typeof(T));
                }
            }

            return value;
        }

        #region Model.IConfigurationSerialiser

        public Microsoft.Win32.RegistryKey CurrentRegistryKey;

        public void WriteEntry<T>(string key, T value)
        {
            if (value != null)
            {
                CurrentRegistryKey.SetValue(key, value.ToString());
            }
        }

        public T ReadEntry<T>(string key, T value)
        {
            try
            {
                if (typeof(T).IsEnum)
                {
                    string text = ReadEntry(key, value.ToString());
                    if (text != null)
                    {
                        T temp = value;
                        value = (T)Enum.Parse(typeof(T), text, true);
                    }
                }
                else
                {
                    value = LoadRegistryEntry<T>(CurrentRegistryKey, value, key);
                }
            }
            catch
            {

            }
            return value;
        }

        #endregion Model.IConfigurationSerialiser
    }
}
