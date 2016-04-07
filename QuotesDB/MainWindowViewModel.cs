using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using QuotesDB.DAO;
using System;
using System.IO;
using System.Windows.Input;

namespace QuotesDB
{
    internal class MainWindowViewModel : ViewModel
    {
        private MainWindow window;

        private DataStore dataStore;

        private string databasePath;
        public string DatabasePath
        {
            get { return databasePath; }
            set
            {
                databasePath = value;
                RaisePropertyChanged();
            }
        }

        public ICommand LoadCommand
        {
            get { return new CommandHelper(LoadDatabase); }
        }

        private void LoadDatabase()
        {
            try
            {
                if (string.IsNullOrEmpty(DatabasePath))
                    throw new Exception("Path cannot be empty.");

                var exists = File.Exists(databasePath);

                dataStore = new DataStore(databasePath, !exists);
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public MainWindowViewModel(MainWindow window)
        {
            this.window = window;
        }
    }
}