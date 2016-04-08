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

        private IQuoteStore dataStore;

        private string databasePath = Properties.Settings.Default.LastPath;
        public string DatabasePath
        {
            get { return databasePath; }
            set
            {
                databasePath = value;
                RaisePropertyChanged();
            }
        }

        public bool Loaded
        {
            get
            {
                return dataStore != null;
            }
        }

        public bool NotLoaded
        {
            get
            {
                return !Loaded;
            }
        }

        public ICommand LoadCommand
        {
            get { return new CommandHelper(LoadDatabase); }
        }

        public ICommand ExportImportCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    var window = new ExportImportWindow(dataStore);
                    window.ShowDialog();
                });
            }
        }

        public ICommand OpenQuotesManagerCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    Manager manager = new Manager(dataStore);
                    manager.Show();
                });
            }
        }

        private void LoadDatabase()
        {
            try
            {
                if (string.IsNullOrEmpty(DatabasePath))
                    throw new Exception("Path cannot be empty.");

                var exists = File.Exists(databasePath);

                dataStore = new DataStore(databasePath, !exists);

                //Store
                Properties.Settings.Default.LastPath = databasePath;
                Properties.Settings.Default.Save();

                RaisePropertyChanged("Loaded");
                RaisePropertyChanged("NotLoaded");
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