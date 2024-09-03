using Archiv10.Application.Shared.Locator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Archiv10.Interfaces.ServiceConfigurationDialog
{
    /// <summary>
    /// Interaction logic for LocalFolderDetails.xaml
    /// </summary>
    public partial class LocalFolderDetails : Window
    {
        public LocalFolderDetails()
        {
            InitializeComponent();


            trvMenu.ItemsSource = ApplicationLocator.GetApplication().RootFolderNodeList;

         
        }
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
 
}
