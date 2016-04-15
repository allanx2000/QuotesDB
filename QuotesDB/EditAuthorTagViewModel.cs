using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using QuotesDB.DAO;
using System;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace QuotesDB
{
    public class EditAuthorTagViewModel : ViewModel
    {
        private enum ItemType
        {
            Tag,
            Author
        }

        private ItemType type;
        private object item;
        private Window window;
        
        public EditAuthorTagViewModel(Window window, object item)
        {
            this.window = window;
            this.item = item;
            
            if (this.item is Tag)
                SetType(ItemType.Tag);
            else if (this.item is Author)
                SetType(ItemType.Author);
            else
                throw new Exception("Type not supported: " + this.item.GetType().Name);
        }

        private void SetType(ItemType type)
        {
            this.type = type;

            switch (type)
            {
                case ItemType.Tag:
                    WindowTitle = "Edit Tag";
                    Name = ((Tag)item).TagName;
                    break;
                case ItemType.Author:
                    WindowTitle = "Edit Author";
                    Name = ((Author)item).Name;
                    break;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChanged();
            }
        }

        public string WindowTitle { get; private set; }

        public ICommand SaveCommand
        {
            get
            {
                return new CommandHelper(Save);
            }
        }

        private readonly IQuoteStore ds = QuoteService.Instance.DataStore;

        private void Save()
        {
            try
            {
                if (string.IsNullOrEmpty(Name))
                    throw new Exception("Name is empty");

                switch (type)
                {
                    case ItemType.Author:
                        Author author = item as Author;
                        if (author.Name == Name)
                        {
                            Close();
                            return;
                        }
                        
                        if (ds.GetAuthors().Where(x => x.Name == Name).FirstOrDefault() != null)
                            throw new Exception(Name + " already exists");

                        author.Name = Name;
                        ds.UpdateAuthor(author);
                        break;
                    case ItemType.Tag:
                        Tag tag = item as Tag;
                        if (tag.TagName == Name)
                        {
                            Close();
                            return;
                        }
                        
                        if (ds.GetTags().Where(x => x.TagName == Name).FirstOrDefault() != null)
                            throw new Exception(Name + " already exists");

                        tag.TagName = Name;
                        ds.UpdateTag(tag);

                        break;
                }

                Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        private void Close()
        {
            window.Close();
        }

        public ICommand CancelCommand
        {
            get
            {
                return new CommandHelper(Close);
            }
        }
    }
}