using System;

namespace OpenControls.Wpf.Serialisation
{
    public class RegistryItemSerialiser : IConfigurationSerialiser
    {
        public RegistryItemSerialiser(string keyPath)
        {
            _keyPath = keyPath;
        }

        private readonly string _keyPath;

        public bool OpenKey()
        {
            CurrentRegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(_keyPath, true);
            return CurrentRegistryKey != null;
        }

        public bool IsOpen
        {
            get
            {
                return CurrentRegistryKey != null;
            }
        }

        public void Close()
        {
            if (CurrentRegistryKey != null)
            {
                CurrentRegistryKey.Close();
            }
            CurrentRegistryKey = null;
        }

        public bool CreateKey()
        {
            CurrentRegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(_keyPath, true);
            return CurrentRegistryKey != null;
        }

        private Microsoft.Win32.RegistryKey CurrentRegistryKey { get; set; }

        #region Model.IConfigurationSerialiser

        public void WriteEntry<T>(string name, T value)
        {
            if (value != null)
            {
                CurrentRegistryKey.SetValue(name, value);
            }
        }

        public T ReadEntry<T>(string name, T value)
        {
            if (CurrentRegistryKey != null)
            {
                object obj = CurrentRegistryKey.GetValue(name);
                if (obj != null)
                {
                    try
                    {
                        if (typeof(T) == typeof(byte[]))
                        {
                            value = (T)Convert.ChangeType(obj, typeof(T));
                        }
                        else
                        {
                            if (typeof(T).IsEnum)
                            {
                                obj = Enum.Parse(typeof(T), obj as string);
                            }

                            value = (T)Convert.ChangeType(obj, typeof(T));
                        }
                    }
                    catch
                    {
                        // Fall through
                    }
                }
            }

            return value;
        }

        #endregion Model.IConfigurationSerialiser
    }
}
