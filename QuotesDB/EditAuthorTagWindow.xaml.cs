﻿using QuotesDB.DAO;
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
    /// Interaction logic for EditAuthorTagWindow.xaml
    /// </summary>
    public partial class EditAuthorTagWindow : Window
    {
        
        private readonly EditAuthorTagViewModel vm;
        public EditAuthorTagWindow(object item)
        {
            InitializeComponent();

            vm = new EditAuthorTagViewModel(this, item);
            this.DataContext = vm;
        }

    }
}
