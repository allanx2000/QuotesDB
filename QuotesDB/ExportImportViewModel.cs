﻿using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using QuotesDB.DAO;
using QuotesDB.Exporter;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace QuotesDB
{
    internal class ExportImportViewModel : ViewModel
    {
        private IQuoteStore ds;
        private Window window;
        
        public ExportImportViewModel(Window window, IQuoteStore ds)
        {
            this.window = window;
            this.ds = ds;
        }

        #region Export
        private string exportPath;
        public string ExportPath {
            get
            {
                return exportPath;
            }
            set
            {
                exportPath = value;
                RaisePropertyChanged();
            }
        }

        public ICommand BrowseForExportCommand
        {
            get
            {
                return new CommandHelper(BrowseForExport);
            }
        }

        private void BrowseForExport()
        {
            var sfd = DialogsUtility.CreateSaveFileDialog("Export");
            DialogsUtility.AddExtension(sfd, "XML File", "*.xml");

            sfd.ShowDialog();

            ExportPath = sfd.FileName;
        }

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

        #endregion

        #region Import
        private string importPath;
        public string ImportPath
        {
            get
            {
                return importPath;
            }
            set
            {
                importPath = value;
                RaisePropertyChanged();
            }
        }

        public ICommand BrowseForImportCommand
        {
            get
            {
                return new CommandHelper(BrowseForImport);
            }
        }

        private void BrowseForImport()
        {
            var ofd = DialogsUtility.CreateOpenFileDialog("Import");
            DialogsUtility.AddExtension(ofd, "XML File", "*.xml");

            ofd.ShowDialog();

            ImportPath = ofd.FileName;
        }

        public ICommand ImportCommand
        {
            get
            {
                return new CommandHelper(Import);
            }
        }

        public void Import()
        {
            try
            {
                if (string.IsNullOrEmpty(importPath) || !File.Exists(ImportPath))
                    throw new Exception("File not found.");

                Bundle bundle = Bundle.Deserialize(importPath);
                ds.Import(bundle);

                MessageBoxFactory.ShowInfo("Database has been imported.", "Imported");

            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        #endregion

        public ICommand ClearDatabaseCommand
        {
            get
            {
                return new CommandHelper(ClearDatabase);
            }
        }

        private void ClearDatabase()
        {
            if (MessageBoxFactory.ShowConfirmAsBool("Are you sure you want to clear the database?", "Confirm Clear Database", MessageBoxImage.Exclamation))
            {
                QuoteService.Instance.DataStore.ClearTables(); //TODO: Change all to use QuoteService
            }
        }
    }
}