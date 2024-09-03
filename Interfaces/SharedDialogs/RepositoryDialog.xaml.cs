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

namespace Archiv10.Interfaces.SharedDialogs
{
    /// <summary>
    /// Interaction logic for RepositoryDialog.xaml
    /// </summary>
    public partial class RepositoryDialog : Window
    {
        public RepositoryDialog()
        {
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Url
        {
            get { return txtUrl.Text.Trim(); }
        }
        public string RepositoryName
        {
            get { return txtName.Text.Trim(); }
        }
        public string Token
        {
            get { return txtToken.Text.Trim(); }
        }
    }
}
