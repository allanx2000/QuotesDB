using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using QuotesDB.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuotesDB
{
    public class EditQuoteViewModel : ViewModel
    {
        private readonly IQuoteStore ds = QuoteService.Instance.DataStore;

        private Quote existingQuote;
        private Window window;
        

        public EditQuoteViewModel(Window window)
        {
            this.window = window;
        }

        public string Author { get; set; }
        public string Quote { get; set; }

        public string Tags
        {
            get
            {
                return string.Join(", ", from t in TagsList select t.TagName);
            }
            
        }

        private List<Tag> TagsList = new List<Tag>();
        private bool TagsChanged = false;

        //public int Rating { get; set; }
        public int Displayed { get; set; }

        public List<Author> Authors
        {
            get { return ds.GetAuthors(); }
        }


        public void SetQuote(Quote quote)
        {
            this.existingQuote = quote;

            Quote = quote.Text;
            //Rating = quote.Rating;
            Author = quote.Author.Name;
            Displayed = quote.Displayed;

            TagsList = ds.GetTagsForQuote(quote);
            
        }

        public ICommand EditTagsCommand
        {
            get
            {
                return new CommandHelper(EditTags);
            }
        }

        private void EditTags()
        {
            TagsEditor editor = new TagsEditor(TagsList);
            editor.ShowDialog();

            if (!editor.Cancelled)
            {
                TagsList = editor.GetTags();
                TagsChanged = true;
                RaisePropertyChanged("Tags");
            }
        }

        public ICommand ResetCountCommand
        {
            get
            {
                return new CommandHelper(ResetCount);
            }
        }

        private void ResetCount()
        {
            if (existingQuote == null)
                return;

            existingQuote.Displayed = 0;
            ds.UpdateQuote(existingQuote);

            Displayed = 0;
            RaisePropertyChanged("Displayed");
        }

        public ICommand CloseCommand
        {
            get
            {
                return new CommandHelper(() => window.Close());
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new CommandHelper(Save);
            }
        }

        public void Save()
        {
            try
            {
                bool isUpdate = existingQuote != null;

                Quote qt = isUpdate ? existingQuote : new DAO.Quote();
                qt.Text = Quote;

                var author = ds.GetAuthors(Author).FirstOrDefault();
                if (author == null)
                {
                    qt.Author = ds.CreateAuthor(new Author() { Name = Author });
                }
                else
                    qt.Author = author;

                qt.Text = Quote;
                //qt.Rating = Rating;

                if (!isUpdate)
                {
                    qt = ds.InsertQuote(qt);
                }
                else
                    ds.UpdateQuote(qt);

                //Tags
                if (TagsChanged)
                    ds.UpdateTags(qt, TagsList);
                

                window.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }


        
    }
}