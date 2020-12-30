namespace OpenControls.Wpf.DatabaseDialogs.Model
{
    public interface IDatabaseConfiguration
    {
        Model.DatabaseProvider SelectedDatabaseProvider { get; set; }

        bool SavePassword { get; set; }

        string SQLite_Folder { get; set; }
        string SQLite_DatabaseName { get; set; }
        string SQLite_Filename { get; set; }

        int SelectedSqlServerInstance { get; set; }
        bool SQLServer_UseLocalServer { get; set; }
        string SQLServer_LocalServerName { get; set; }
        string SQLServer_IPAddress { get; set; }
        bool SQLServer_UseIPv6 { get; set; }
        ushort SQLServer_Port { get; set; }
        bool SQLServer_UseWindowsAuthentication { get; set; }
        string SQLServer_UserName { get; set; }
        string SQLServer_Password { get; set; }
        string SQLServer_Folder { get; set; }
        string SQLServer_Filename { get; set; }
        string SQLServer_DatabaseName { get; set; }

        string PostgreSQL_IPAddress { get; set; }
        bool PostgreSQL_UseIPv6 { get; set; }
        ushort PostgreSQL_Port { get; set; }
        bool PostgreSQL_UseWindowsAuthentication { get; set; }
        string PostgreSQL_UserName { get; set; }
        string PostgreSQL_Password { get; set; }
        string PostgreSQL_DatabaseName { get; set; }

        string MySQL_IPAddress { get; set; }
        bool MySQL_UseIPv6 { get; set; }
        ushort MySQL_Port { get; set; }
        bool MySQL_UseWindowsAuthentication { get; set; }
        string MySQL_UserName { get; set; }
        string MySQL_Password { get; set; }
        string MySQL_DatabaseName { get; set; }
    }
}
