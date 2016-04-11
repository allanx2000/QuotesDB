﻿using Innouvous.Utils.MVVM;
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
    }

    public class QuoteViewerViewModel : ViewModel
    {
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
    }
}