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
        private QuoteViewer quoteViewer;
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

        public ICommand BrowseCommand
        {
            get
            {
                return new CommandHelper(Browse);
            }
        }

        public void Browse()
        {
            var dlg = DialogsUtility.CreateSaveFileDialog("Select Database File");
            DialogsUtility.AddExtension(dlg, "SQLite Database", "*.db");
            dlg.ShowDialog();

            if (!string.IsNullOrEmpty(dlg.FileName))
                DatabasePath = dlg.FileName;
        }

        public ICommand RefreshQuoteCommand
        {
            get { return new CommandHelper(RefreshQuote); }
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

        public ICommand OpenPopupSettingsCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    PopupSettings window = new PopupSettings();
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
                    manager.ShowDialog();
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
                QuoteService.Instance.SetDataStore(dataStore);

                //Store
                Properties.Settings.Default.LastPath = databasePath;
                Properties.Settings.Default.Save();

                RaisePropertyChanged("Loaded");
                RaisePropertyChanged("NotLoaded");


                quoteViewer.SetDataStore(dataStore);

                RefreshQuote();
                
                QuoteService.Instance.StartTimer();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public MainWindowViewModel(MainWindow window, QuoteViewer quoteViewer)
        {
            this.window = window;
            this.quoteViewer = quoteViewer;

        }

        private void RefreshQuote()
        {
            if (dataStore != null)
            {
                var quote = dataStore.GetRandomQuote();
                quoteViewer.SetQuote(quote);
            }
        }
    }
}