using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuotesDB.DAO;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Innouvous.Utils;

namespace QuotesDB
{
    class ManagerViewModel : ViewModel
    {
        private const string Author = "Author";
        private const string Tag = "Tag";

        private IQuoteStore dataStore;

        public ManagerViewModel(IQuoteStore ds)
        {
            this.dataStore = ds;
            ListItems = new ObservableCollection<object>();
            QuotesList = new ObservableCollection<Quote>();
        }

        private void LoadItem()
        {
            QuotesList.Clear();

            if (selectedListItem is Author)
            {
                var quotes = dataStore.GetQuotes((Author)selectedListItem);
                foreach (var q in quotes)
                    QuotesList.Add(q);
            }

            RaisePropertyChanged("QuotesList");
        }
        
        private void RefreshList()
        {
            switch (selectedList)
            {
                case Author:
                    List<Author> authors = dataStore.GetAuthors();
                    ListItems.Clear();

                    //authors.Add(new DAO.Author() { ID = 1, Name = "TEST" });

                    foreach (var a in authors)
                        ListItems.Add(a);

                    ListPath = "Name";

                    break;
            }
        }

        #region Author/Tags List

        //DisplayMemberPath for the Authors/Tags List
        private string listPath;
        public string ListPath
        {
            get { return listPath; }
            set
            {
                listPath = value;
                RaisePropertyChanged();
            }
        }
        
        public List<string> ListBy
        {
            get
            {
                return new List<string>() {
                    Author,
                    Tag
                };
            }
        }

        //SelectedItem For the Author/Tags List
        private object selectedListItem;
        public object SelectedListItem
        {
            get
            {
                return selectedListItem;
            }
            set
            {
                selectedListItem = value;
                LoadItem();
                RaisePropertyChanged();
            }
        }

        //Selected Item from the Tags/Author ComboBox
        private string selectedList;
        public string SelectedList
        {
            get
            {
                return selectedList;
            }
            set
            {
                selectedList = value;
                RaisePropertyChanged();

                RefreshList();
            }
        }

        public ObservableCollection<object> ListItems
        {
            get; set;
        }

        #endregion

        #region Quotes

        #region Commands

        public ICommand DeleteQuoteCommand
        {
            get
            {
                return new CommandHelper(DeleteQuote);
            }
        }

        private void DeleteQuote()
        {
            if (SelectedQuote != null 
                && MessageBoxFactory.ShowConfirmAsBool("Are you sure you want to delete the selected quote?", "Confirm Delete"))
            {
                dataStore.DeleteQuote(SelectedQuote);
                LoadItem();
            }
        }

        public ICommand AddQuoteCommand
        {
            get
            {
                return new CommandHelper(AddQuote);
            }
        }

        private void AddQuote()
        {
            EditQuoteWindow edit = new EditQuoteWindow(dataStore);
            edit.ShowDialog();

            LoadItem();
        }

        #endregion

        public ObservableCollection<Quote> QuotesList { get; private set; }

        private Quote selectedQuote;
        public Quote SelectedQuote
        {
            get
            {
                return selectedQuote;
            }
            set
            {
                selectedQuote = value;
                RaisePropertyChanged();
            }
        }

        
        #endregion

    }
}
