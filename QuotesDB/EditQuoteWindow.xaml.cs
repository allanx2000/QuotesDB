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
    /// Interaction logic for EditQuoteWindow.xaml
    /// </summary>
    public partial class EditQuoteWindow : Window
    {
        private readonly EditQuoteViewModel vm;

        public EditQuoteWindow(IQuoteStore ds, Quote quote) : this(ds)
        {
            vm.SetQuote(quote);
        }


        public EditQuoteWindow(IQuoteStore ds)
        {
            InitializeComponent();

            vm = new EditQuoteViewModel(ds, this);
            this.DataContext = vm;


        }

        private void FocusOnComboBox()
        {   
            TextBox textBox = GetChildFromVisualTree(AuthorComboBox, typeof(TextBox)) as TextBox;
            
            if (textBox != null)
            {
                textBox.Focus();
            }
        }

        public DependencyObject GetChildFromVisualTree(DependencyObject parent, Type objectType)
        {
            if (parent == null)
                return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (objectType.IsInstanceOfType(child))
                {
                    return child;
                }

                DependencyObject innerChild = GetChildFromVisualTree(child, objectType);
                if (innerChild != null)
                    return innerChild;
            }
            
            return null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FocusOnComboBox();
        }
    }
}
