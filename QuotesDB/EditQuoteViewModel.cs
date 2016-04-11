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
        private IQuoteStore ds;
        private Quote quote;
        private Window window;

        public EditQuoteViewModel(IQuoteStore ds, Window window)
        {
            this.ds = ds;
            this.window = window;
        }

        public void SetQuote(Quote quote)
        {
            this.quote = quote;

            Quote = quote.Text;
            Rating = quote.Rating;
            Author = quote.Author.Name;
            Displayed = quote.Displayed;

            List<Tag> tags = ds.GetTagsForQuote(quote);
            Tags = string.Join(", ", (from t in tags select t.TagName));
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
                bool isUpdate = quote != null;
                Quote qt = isUpdate ? quote : new DAO.Quote();
                qt.Text = Quote;

                var author = ds.GetAuthors(Author).FirstOrDefault();
                if (author == null)
                {
                    qt.Author = ds.CreateAuthor(new Author() { Name = Author });
                }
                else
                    qt.Author = author;

                qt.Text = Quote;
                qt.Rating = Rating;

                if (!isUpdate)
                {
                    int id = ds.InsertQuote(qt);
                    qt.ID = id;
                }

                //Tags
                if (!string.IsNullOrEmpty(Tags))
                {
                    var tags = from i in Tags.Split(',') select i.Trim();
                    ds.UpdateTags(qt, tags);
                }

                window.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }


        public List<Author> Authors
        {
            get { return ds.GetAuthors(); }
        }

        public string Author { get; set; }
        public string Quote { get; set; }
        public string Tags { get; set; }

        public int Rating { get; set; }
        public int Displayed { get; set; }
    }
}