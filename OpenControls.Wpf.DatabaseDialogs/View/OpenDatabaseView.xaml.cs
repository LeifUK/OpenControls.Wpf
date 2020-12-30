using System.Windows;

namespace OpenControls.Wpf.DatabaseDialogs.View
{
    /// <summary>
    /// Interaction logic for OpenDatabaseView.xaml
    /// </summary>
    public partial class OpenDatabaseView : Window
    {
        public OpenDatabaseView(Model.IEncryption iEncryption)
        {
            IEncryption = iEncryption;
            InitializeComponent();
        }

        private readonly Model.IEncryption IEncryption;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenDatabaseViewModel openDatabaseViewModel = DataContext as ViewModel.OpenDatabaseViewModel;
            if (openDatabaseViewModel.IDatabaseConfiguration.SavePassword)
            {
                if (IEncryption != null)
                {
                    _passwordBoxSQLServer.Password = IEncryption.Decrypt(openDatabaseViewModel.SQLServer_Password);
                    _passwordBoxPostgreSQL.Password = IEncryption.Decrypt(openDatabaseViewModel.PostgreSQL_Password);
                    _passwordBoxMySQL.Password = IEncryption.Decrypt(openDatabaseViewModel.MySQL_Password);
                }
                else
                {
                    _passwordBoxSQLServer.Password = openDatabaseViewModel.SQLServer_Password;
                    _passwordBoxPostgreSQL.Password = openDatabaseViewModel.PostgreSQL_Password;
                    _passwordBoxMySQL.Password = openDatabaseViewModel.MySQL_Password;
                }
            }
        }

        private void _buttonBrowseSQLiteDatabases_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog == null)
            {
                return;
            }

            ViewModel.OpenDatabaseViewModel openDatabaseViewModel = DataContext as ViewModel.OpenDatabaseViewModel;
            openFileDialog.FileName = openDatabaseViewModel.SQLite_Filename;
            openFileDialog.Filter = "SQLite Database (*.sqlite)|*.sqlite|Microsoft SQL Server Database(*.mdf) | *.mdf";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openDatabaseViewModel.SQLite_Filename = openFileDialog.FileName;
            }
        }

        private void _buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            ViewModel.OpenDatabaseViewModel openDatabaseViewModel = DataContext as ViewModel.OpenDatabaseViewModel;
            openDatabaseViewModel.Refresh();
            System.Windows.Input.Mouse.OverrideCursor = null;
        }

        private void _buttonOK_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenDatabaseViewModel openDatabaseViewModel = DataContext as ViewModel.OpenDatabaseViewModel;
            if (IEncryption != null)
            {
                openDatabaseViewModel.SQLServer_Password = IEncryption.Encrypt(_passwordBoxSQLServer.Password);
                openDatabaseViewModel.PostgreSQL_Password = IEncryption.Encrypt(_passwordBoxPostgreSQL.Password);
                openDatabaseViewModel.MySQL_Password = IEncryption.Encrypt(_passwordBoxMySQL.Password);
            }
            else
            {
                openDatabaseViewModel.SQLServer_Password = _passwordBoxSQLServer.Password;
                openDatabaseViewModel.PostgreSQL_Password = _passwordBoxPostgreSQL.Password;
                openDatabaseViewModel.MySQL_Password = _passwordBoxMySQL.Password;
            }
            DialogResult = true;
        }

        private void _buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
