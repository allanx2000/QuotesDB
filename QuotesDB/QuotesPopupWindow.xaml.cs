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
using System.Windows.Shapes;

namespace QuotesDB
{
    /// <summary>
    /// Interaction logic for QuotesPopupWindow.xaml
    /// </summary>
    public partial class QuotesPopupWindow : Window
    {
        public QuotesPopupWindow(IQuoteStore ds)
        {
            InitializeComponent();
            this.DataContext = this;

            WindowOpacity = 0;

            //QuotesViewer.SetDataStore(ds);
            QuotesViewer.SetQuote(ds.GetRandomQuote());
        }

        public double WindowOpacity
        {
            get; set;
        }
        
        public ICommand CloseCommand
        {
            get
            {
                return new CommandHelper(() => this.Close());
            }
        }
    }
}
