using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using QuotesDB.DAO;
using QuotesDB.Exporter;
using System;
using System.Windows;
using System.Windows.Input;

namespace QuotesDB
{
    internal class ExportImportViewModel : ViewModel
    {
        private IQuoteStore ds;
        private Window window;

        public string ExportPath { get; set; }

        public ICommand ExportCommand
        {
            get
            {
                return new CommandHelper(Export);
            }
        }

        public void Export()
        {
            try
            {
                if (string.IsNullOrEmpty(ExportPath))
                    throw new Exception("Path cannot be empty.");

                Bundle bundle = ds.Export();
                Bundle.Serialize(ExportPath, bundle);

                MessageBoxFactory.ShowInfo("Database exported to: " + ExportPath, "Exported");

            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ExportImportViewModel(Window window, IQuoteStore ds)
        {
            this.window = window;
            this.ds = ds;
        }
    }
}