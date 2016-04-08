using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuotesDB.DAO;
using System.Collections.ObjectModel;
using System.Windows.Input;

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
        }

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

        private object selectedItem;
        public object SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                LoadItem();
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Quote> QuotesList { get; private set; }

        private void LoadItem()
        {
            QuotesList.Clear();

            if (selectedItem is Author)
            {
                var quotes = dataStore.GetQuotes((Author)selectedItem);
                foreach (var q in quotes)
                    QuotesList.Add(q);
            }

            RaisePropertyChanged("QuotesList");
        }

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

        private void RefreshList()
        {
            switch(selectedList)
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
    }
}
