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
using QuotesDB.DAO;

namespace QuotesDB
{
    /// <summary>
    /// Interaction logic for Manager.xaml
    /// </summary>
    public partial class Manager : Window
    {
        private readonly ManagerViewModel vm;

        public Manager()
        {
            InitializeComponent();
        }

        public Manager(IQuoteStore dataStore) : this()
        {
            vm = new ManagerViewModel(dataStore);
            this.DataContext = vm;
        }
    }
}
