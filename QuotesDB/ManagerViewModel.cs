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
            else if (selectedListItem is Tag)
            {
                var quotes = dataStore.GetQuotes((Tag)selectedListItem);
                foreach (var q in quotes)
                    QuotesList.Add(q);
            }
            else
                return;
                //throw new NotImplementedException("Not implemented for: " + selectedListItem.GetType().Name);

            RaisePropertyChanged("QuotesList");
        }

        private void RefreshList()
        {
            int id = 0;

            if (SelectedListItem != null)
            {
                var author = SelectedListItem as Author;
                if (author != null)
                {
                    id = author.ID;
                }
                else
                {
                    var tag = SelectedListItem as Tag;
                    if (tag != null)
                    {
                        id = tag.ID;
                    }
                }
            }
            
            switch (selectedList)
            {
                case Author:
                    
                    var authors = dataStore.GetAuthors(); //.OrderBy(x => x.Name);
                    
                    ListItems.Clear();
                    foreach (var a in authors)
                        ListItems.Add(a);

                    ListPath = "Name";
                    
                    if (id != 0)
                    {
                        var author = ListItems.FirstOrDefault(x => ((Author)x).ID == id);
                        SelectedListItem = author;
                    }
                    break;
                case Tag:
                    var tags = dataStore.GetTags(); 

                    ListItems.Clear();
                    foreach (var t in tags)
                        ListItems.Add(t);

                    ListPath = "TagName";

                    if (id != 0)
                    {
                        var tag = ListItems.FirstOrDefault(x => ((Tag)x).ID == id);
                        SelectedListItem = tag;
                    }
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
            EditQuoteWindow edit = new EditQuoteWindow();
            edit.ShowDialog();

            RefreshList();
            
            //LoadItem();
        }

        public ICommand EditQuoteCommand
        {
            get
            {
                return new CommandHelper(EditQuote);
            }
        }

        private void EditQuote()
        {
            if (SelectedQuote == null)
                return;

            EditQuoteWindow edit = new EditQuoteWindow(SelectedQuote);
            edit.ShowDialog();

            LoadItem();
        }


        public ICommand DeleteFromListCommand
        {
            get
            {
                return new CommandHelper(DeleteFromList);
            }

        }

        private void DeleteFromList()
        {
            try
            {
                switch (SelectedList)
                {
                    case Author:
                        Author author = SelectedListItem as Author;
                        if (author != null && MessageBoxFactory.ShowConfirmAsBool("Delete author: " + author.Name, "Delete Author"))
                        {
                            dataStore.DeleteAuthor(author);
                        }
                        break;
                    case Tag:
                        Tag tag = selectedListItem as Tag;

                        if (tag != null)
                        {
                            int count = dataStore.GetQuotesCount(tag);
                            if (MessageBoxFactory.ShowConfirmAsBool(String.Format("{0} has {1} quotes associated with it. Continue delete?", tag.TagName , count), 
                                "Delete Tag"))
                            {
                                dataStore.DeleteTag(tag);
                            }
                        }
                        break;
                }

                RefreshList();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }


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
