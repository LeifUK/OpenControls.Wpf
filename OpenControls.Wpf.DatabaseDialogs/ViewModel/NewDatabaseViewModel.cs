using System;
using System.Collections.ObjectModel;

namespace OpenControls.Wpf.DatabaseDialogs.ViewModel
{
    public class NewDatabaseViewModel : BaseViewModel
    {
        public NewDatabaseViewModel(Model.IDatabaseConfiguration iDatabaseConfiguration)
        {
            IDatabaseConfiguration = iDatabaseConfiguration;
            Title = "Create Database";
            SqlServerInstances = new ObservableCollection<string>();
            if (!string.IsNullOrEmpty(IDatabaseConfiguration.SQLServer_LocalServerName))
            {
                SqlServerInstances.Add(IDatabaseConfiguration.SQLServer_LocalServerName);
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        public readonly Model.IDatabaseConfiguration IDatabaseConfiguration;

        public void Refresh()
        {
            SqlServerInstances.Clear();
            System.Data.Sql.SqlDataSourceEnumerator instance = System.Data.Sql.SqlDataSourceEnumerator.Instance;
            System.Data.DataTable dataTable = instance.GetDataSources();

            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                foreach (System.Data.DataColumn col in dataTable.Columns)
                {
                    Console.WriteLine("{0} = {1}", col.ColumnName, row[col]);
                }

                SqlServerInstances.Add(row[0] + "\\" + row[1]);
            }

            if (SqlServerInstances.Count > 0)
            {
                SelectedSqlServerInstance = SqlServerInstances[0];
            }
        }

        public bool SavePassword
        {
            get
            {
                return IDatabaseConfiguration.SavePassword;
            }
            set
            {
                IDatabaseConfiguration.SavePassword = value;
                NotifyPropertyChanged("SavePassword");
            }
        }

        public Model.DatabaseProvider SelectedDatabaseProvider
        {
            get
            {
                return IDatabaseConfiguration.SelectedDatabaseProvider;
            }
            set
            {
                IDatabaseConfiguration.SelectedDatabaseProvider = value;
                NotifyPropertyChanged("SelectedDatabaseProvider");
            }
        }

        private ObservableCollection<string> _sqlServerInstances;
        public ObservableCollection<string> SqlServerInstances
        {
            get
            {
                return _sqlServerInstances;
            }
            set
            {
                _sqlServerInstances = value;
                NotifyPropertyChanged("SqlServerInstances");
            }
        }

        public string SQLite_Folder
        {
            get
            {
                return IDatabaseConfiguration.SQLite_Folder;
            }
            set
            {
                IDatabaseConfiguration.SQLite_Folder = value;
                NotifyPropertyChanged("SQLite_Folder");
            }
        }

        public string SQLite_DatabaseName
        {
            get
            {
                return IDatabaseConfiguration.SQLite_DatabaseName;
            }
            set
            {
                IDatabaseConfiguration.SQLite_DatabaseName = value;
                NotifyPropertyChanged("SQLite_DatabaseName");
            }
        }

        public string SelectedSqlServerInstance
        {
            get
            {
                return IDatabaseConfiguration.SQLServer_LocalServerName;
            }
            set
            {
                IDatabaseConfiguration.SQLServer_LocalServerName = value;
                NotifyPropertyChanged("SelectedSqlServerInstance");
            }
        }

        public bool SQLServer_UseWindowsAuthentication
        {
            get
            {
                return IDatabaseConfiguration.SQLServer_UseWindowsAuthentication;
            }
            set
            {
                IDatabaseConfiguration.SQLServer_UseWindowsAuthentication = value;
                NotifyPropertyChanged("SQLServer_UseWindowsAuthentication");
            }
        }

        public string SQLServer_UserName
        {
            get
            {
                return IDatabaseConfiguration.SQLServer_UserName;
            }
            set
            {
                IDatabaseConfiguration.SQLServer_UserName = value;
                NotifyPropertyChanged("SQLServer_UserName");
            }
        }

        public string SQLServer_Password
        {
            get
            {
                return IDatabaseConfiguration.SQLServer_Password;
            }
            set
            {
                IDatabaseConfiguration.SQLServer_Password = value;
                NotifyPropertyChanged("SQLServer_Password");
            }
        }

        public string SQLServer_Folder
        {
            get
            {
                return IDatabaseConfiguration.SQLServer_Folder;
            }
            set
            {
                IDatabaseConfiguration.SQLServer_Folder = value;
                NotifyPropertyChanged("SQLServer_Folder");
            }
        }

        public string SQLServer_Filename
        {
            get
            {
                return IDatabaseConfiguration.SQLServer_Filename;
            }
            set
            {
                IDatabaseConfiguration.SQLServer_Filename = value;
                NotifyPropertyChanged("SQLServer_Filename");
            }
        }

        public string PostgreSQL_IPAddress
        {
            get
            {
                return IDatabaseConfiguration.PostgreSQL_IPAddress;
            }
            set
            {
                try
                {
                    System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(value);
                    if (PostgreSQL_UseIPv6)
                    {
                        byte[] bytes = ipAddress.GetAddressBytes();
                        bool first = true;
                        string text = "";
                        for (int index = 0; index < 16; index += 2)
                        {
                            if (!first)
                            {
                                text += ":";
                            }
                            short shortVal = (short)(bytes[index + 1] + (bytes[index] << 8));
                            first = false;
                            text += shortVal.ToString("X");
                        }
                        IDatabaseConfiguration.PostgreSQL_IPAddress = text;
                    }
                    else
                    {
                        IDatabaseConfiguration.PostgreSQL_IPAddress = ipAddress.ToString();
                    }
                }
                catch
                {

                }
                NotifyPropertyChanged("PostgreSQL_IPAddress");
            }
        }

        public bool PostgreSQL_UseIPv6
        {
            get
            {
                return IDatabaseConfiguration.PostgreSQL_UseIPv6;
            }
            set
            {
                if (IDatabaseConfiguration.PostgreSQL_UseIPv6 != value)
                {
                    IDatabaseConfiguration.PostgreSQL_UseIPv6 = value;
                    if (value)
                    {
                        PostgreSQL_IPAddress = "0:0:0:0:0:0:0:1";
                    }
                    else
                    {
                        PostgreSQL_IPAddress = "127.0.0.1";
                    }
                }
                NotifyPropertyChanged("PostgreSQL_UseIPv6");
            }
        }

        public ushort PostgreSQL_Port
        {
            get
            {
                return IDatabaseConfiguration.PostgreSQL_Port;
            }
            set
            {
                IDatabaseConfiguration.PostgreSQL_Port = value;
                NotifyPropertyChanged("PostgreSQL_Port");
            }
        }

        public bool PostgreSQL_UseWindowsAuthentication
        {
            get
            {
                return IDatabaseConfiguration.PostgreSQL_UseWindowsAuthentication;
            }
            set
            {
                IDatabaseConfiguration.PostgreSQL_UseWindowsAuthentication = value;
                NotifyPropertyChanged("PostgreSQL_UseWindowsAuthentication");
            }
        }

        public string PostgreSQL_UserName
        {
            get
            {
                return IDatabaseConfiguration.PostgreSQL_UserName;
            }
            set
            {
                IDatabaseConfiguration.PostgreSQL_UserName = value;
                NotifyPropertyChanged("PostgreSQL_UserName");
            }
        }

        public string PostgreSQL_Password
        {
            get
            {
                return IDatabaseConfiguration.PostgreSQL_Password;
            }
            set
            {
                IDatabaseConfiguration.PostgreSQL_Password = value;
                NotifyPropertyChanged("PostgreSQL_Password");
            }
        }

        public string PostgreSQL_DatabaseName
        {
            get
            {
                return IDatabaseConfiguration.PostgreSQL_DatabaseName;
            }
            set
            {
                IDatabaseConfiguration.PostgreSQL_DatabaseName = value;
                NotifyPropertyChanged("PostgreSQL_DatabaseName");
            }
        }

        public string MySQL_IPAddress
        {
            get
            {
                return IDatabaseConfiguration.MySQL_IPAddress;
            }
            set
            {
                try
                {
                    System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(value);
                    if (MySQL_UseIPv6)
                    {
                        byte[] bytes = ipAddress.GetAddressBytes();
                        bool first = true;
                        string text = "";
                        for (int index = 0; index < 16; index += 2)
                        {
                            if (!first)
                            {
                                text += ":";
                            }
                            short shortVal = (short)(bytes[index + 1] + (bytes[index] << 8));
                            first = false;
                            text += shortVal.ToString("X");
                        }
                        IDatabaseConfiguration.MySQL_IPAddress = text;
                    }
                    else
                    {
                        IDatabaseConfiguration.MySQL_IPAddress = ipAddress.ToString();
                    }
                }
                catch
                {

                }
                NotifyPropertyChanged("MySQL_IPAddress");
            }
        }

        public bool MySQL_UseIPv6
        {
            get
            {
                return IDatabaseConfiguration.MySQL_UseIPv6;
            }
            set
            {
                if (IDatabaseConfiguration.MySQL_UseIPv6 != value)
                {
                    IDatabaseConfiguration.MySQL_UseIPv6 = value;
                    if (value)
                    {
                        MySQL_IPAddress = "0:0:0:0:0:0:0:1";
                    }
                    else
                    {
                        MySQL_IPAddress = "127.0.0.1";
                    }
                }
                NotifyPropertyChanged("MySQL_UseIPv6");
            }
        }

        public ushort MySQL_Port
        {
            get
            {
                return IDatabaseConfiguration.MySQL_Port;
            }
            set
            {
                IDatabaseConfiguration.MySQL_Port = value;
                NotifyPropertyChanged("MySQL_Port");
            }
        }

        public bool MySQL_UseWindowsAuthentication
        {
            get
            {
                return IDatabaseConfiguration.MySQL_UseWindowsAuthentication;
            }
            set
            {
                IDatabaseConfiguration.MySQL_UseWindowsAuthentication = value;
                NotifyPropertyChanged("MySQL_UseWindowsAuthentication");
            }
        }

        public string MySQL_UserName
        {
            get
            {
                return IDatabaseConfiguration.MySQL_UserName;
            }
            set
            {
                IDatabaseConfiguration.MySQL_UserName = value;
                NotifyPropertyChanged("MySQL_UserName");
            }
        }

        public string MySQL_Password
        {
            get
            {
                return IDatabaseConfiguration.MySQL_Password;
            }
            set
            {
                IDatabaseConfiguration.MySQL_Password = value;
                NotifyPropertyChanged("MySQL_Password");
            }
        }

        public string MySQL_DatabaseName
        {
            get
            {
                return IDatabaseConfiguration.MySQL_DatabaseName;
            }
            set
            {
                IDatabaseConfiguration.MySQL_DatabaseName = value;
                NotifyPropertyChanged("MySQL_DatabaseName");
            }
        }
    }
}
