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
    /// Interaction logic for TagsEditor.xaml
    /// </summary>
    public partial class TagsEditor : Window
    {
        private readonly TagsEditorViewModel vm;

        public TagsEditor(IEnumerable<Tag> tags = null)
        {
            InitializeComponent();
            
            vm = new TagsEditorViewModel(this, tags);
            this.DataContext = vm;
        }

        public bool Cancelled
        {
            get
            {
                return vm.Cancelled;
            }
        }

        public List<Tag> GetTags()
        {
            return vm.CurrentList.ToList();
        }
    }
}
