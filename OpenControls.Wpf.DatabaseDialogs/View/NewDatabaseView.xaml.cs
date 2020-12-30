using System.Windows;

namespace OpenControls.Wpf.DatabaseDialogs.View
{
    /// <summary>
    /// Interaction logic for SqlConnectionDialog.xaml
    /// </summary>
    public partial class NewDatabaseView : Window
    {
        public NewDatabaseView(Model.IEncryption iEncryption)
        {
            IEncryption = iEncryption;
            InitializeComponent();
        }

        private readonly Model.IEncryption IEncryption;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.NewDatabaseViewModel newDatabaseViewModel = DataContext as ViewModel.NewDatabaseViewModel;
            if (newDatabaseViewModel.IDatabaseConfiguration.SavePassword)
            {
                if (IEncryption != null)
                {
                    _passwordBoxSQLServer.Password = IEncryption.Decrypt(newDatabaseViewModel.SQLServer_Password);
                    _passwordBoxPostgreSQL.Password = IEncryption.Decrypt(newDatabaseViewModel.PostgreSQL_Password);
                    _passwordBoxMySQL.Password = IEncryption.Decrypt(newDatabaseViewModel.MySQL_Password);
                }
                else
                {
                    _passwordBoxSQLServer.Password = newDatabaseViewModel.SQLServer_Password;
                    _passwordBoxPostgreSQL.Password = newDatabaseViewModel.PostgreSQL_Password;
                    _passwordBoxMySQL.Password = newDatabaseViewModel.MySQL_Password;
                }
            }
        }

        private void _buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            ViewModel.NewDatabaseViewModel newDatabaseViewModel = DataContext as ViewModel.NewDatabaseViewModel;
            newDatabaseViewModel.Refresh();
            System.Windows.Input.Mouse.OverrideCursor = null;
        }

        private void _buttonBrowseMicrosoftSQLServerFolders_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog == null)
            {
                return;
            }

            ViewModel.NewDatabaseViewModel newDatabaseViewModel = DataContext as ViewModel.NewDatabaseViewModel;
            dialog.SelectedPath = newDatabaseViewModel.SQLServer_Folder;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                newDatabaseViewModel.SQLServer_Folder = dialog.SelectedPath;
            }
        }

        private void _buttonBrowseSQLiteFolders_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog == null)
            {
                return;
            }

            ViewModel.NewDatabaseViewModel newDatabaseViewModel = DataContext as ViewModel.NewDatabaseViewModel;
            dialog.SelectedPath = newDatabaseViewModel.SQLite_Folder;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                newDatabaseViewModel.SQLite_Folder = dialog.SelectedPath;
            }
        }

        private void _buttonOK_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NewDatabaseViewModel newDatabaseViewModel = DataContext as ViewModel.NewDatabaseViewModel;
            if (IEncryption != null)
            {
                newDatabaseViewModel.SQLServer_Password = IEncryption.Encrypt(_passwordBoxSQLServer.Password);
                newDatabaseViewModel.PostgreSQL_Password = IEncryption.Encrypt(_passwordBoxPostgreSQL.Password);
                newDatabaseViewModel.MySQL_Password = IEncryption.Encrypt(_passwordBoxMySQL.Password);
            }
            else
            {
                newDatabaseViewModel.SQLServer_Password = _passwordBoxSQLServer.Password;
                newDatabaseViewModel.PostgreSQL_Password = _passwordBoxPostgreSQL.Password;
                newDatabaseViewModel.MySQL_Password = _passwordBoxMySQL.Password;
            }
            DialogResult = true;
        }

        private void _buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
