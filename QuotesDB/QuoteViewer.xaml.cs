using Innouvous.Utils.MVVM;
using QuotesDB.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuotesDB
{
    /// <summary>
    /// Interaction logic for QuoteViewer.xaml
    /// </summary>
    public partial class QuoteViewer : UserControl
    {
        private readonly QuoteViewerViewModel vm;
        public QuoteViewer()
        {
            
            InitializeComponent();

            vm = new QuoteViewerViewModel();
            this.DataContext = vm;
        }

        public void SetQuote(Quote quote)
        {
            vm.SetQuote(quote);
        }

        public void SetDataStore(IQuoteStore dataStore)
        {
            vm.SetDataStore(dataStore);
        }
    }

    public class QuoteViewerViewModel : ViewModel
    {
        //TODO: Make this Singleton, DI, Global Service
        private IQuoteStore ds;

        private int quoteSize = 15;
        public int QuoteSize
        {
            get
            {
                return quoteSize;
            }
            set
            {
                quoteSize = value;
            }
        }
        
        private Quote quote;

        public string Quote
        {
            get
            {
                return quote == null ? "" : quote.Text;
            }
        }

        public string Author
        {
            get
            {
                return quote == null ? "" : "-" + quote.Author.Name;
            }
        }

        public int Rating
        {
            get
            {
                return quote == null ? 0 : quote.Rating;
            }
            set
            {
                if (quote != null)
                {
                    if (value == quote.Rating)
                        return;

                    quote.Rating = value;
                    ds.UpdateQuoteRating(quote);
                }

                RaisePropertyChanged();
                RaisePropertyChanged("RatingText");
            }
        }

        public string RatingText
        {
            get
            {
                return Rating == 0 ? "NA" : Rating.ToString();
            }
        }

        public void SetDataStore(IQuoteStore ds)
        {
            this.ds = ds;
        }

        public void SetQuote(Quote quote)
        {
            this.quote = quote;

            quote.Displayed += 1;
            ds.UpdateQuoteCount(quote);

            RaisePropertyChanged("Quote");
            RaisePropertyChanged("Author");
            RaisePropertyChanged("Rating");
            RaisePropertyChanged("RatingText");

        }

    }
}
