﻿using System;
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

namespace Archiv10.Interfaces.ImportDialog
{
    /// <summary>
    /// Interaction logic for BagDialog.xaml
    /// </summary>
    public partial class BagDialog : Window
    {
        public BagDialog()
        {
            InitializeComponent();
        }
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Description
        {
            get { return txtDescription.Text; }
        }

    }
}
