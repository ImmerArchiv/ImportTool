using Archiv10.Domain.Shared.BO;
using Archiv10.Domain.Shared.Locator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Archiv10.Interfaces.ServiceConfigurationDialog
{
    /// <summary>
    /// Interaction logic for RootFolderDialog.xaml
    /// </summary>
    public partial class RootFolderDialog : Window
    {
       

        public RootFolderDialog()
        {
            InitializeComponent();
            btnDialogOk.IsEnabled = false;

        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
             var dialog = new FolderBrowserDialog();
             DialogResult result = dialog.ShowDialog();
             if (result == System.Windows.Forms.DialogResult.OK)
             {
                txtFolder.Text = dialog.SelectedPath;
             }
        }

        private void tbFolder_Changed(object sender, RoutedEventArgs e)
        {
            if(Directory.Exists(txtFolder.Text))
            {
                var subfolders = Directory.GetDirectories(txtFolder.Text);
                var subfolder = txtFolder.Text;
                if (subfolders != null && subfolders.Length > 0)
                    subfolder = subfolders[0];

                var nameService = DomainLocator.GetNameService();
                var nameRelativeName = nameService.CreateBagitName(txtFolder.Text, subfolder, NameType.NameFromRelativePath);
                var nameFolderName = nameService.CreateBagitName(txtFolder.Text, subfolder, NameType.NameFromFolderName);

                replaceName(rbFolderName, nameFolderName);
                replaceName(rbRelativeName, nameRelativeName);
                btnDialogOk.IsEnabled = true;
                return;
            }

            replaceName(rbFolderName, "");
            replaceName(rbRelativeName, "");
            btnDialogOk.IsEnabled = false;
        }



        public string SelectedPath
        {
            get
            {
                return txtFolder.Text;
            }
            set
            {
                txtFolder.Text = value;
            }
        }
        public string[] Filter
        {
            get
            {
                if (rbFilterAll.IsChecked == true)
                    return new string[] { "*" };
                if (rbFilterImages.IsChecked == true)
                    return new string[] { "*.jpg" , "*.jpeg", "*.png", "*.gif" };
                if (rbFilterPdfs.IsChecked == true)
                    return new string[] { "*.pdf" };
                if (rbFilterIndividual.IsChecked == true)
                    return txtFilter.Text.Split(',').Select(p => p.Trim()).ToArray();

                throw new NotImplementedException();

            }
            set
            {
                if(value.SequenceEqual(new string[] { "*" }))
                {
                    rbFilterAll.IsChecked = true;
                    return;
                }
                if (value.SequenceEqual(new string[] { "*.jpg", "*.jpeg", "*.png", "*.gif" }))
                {
                    rbFilterImages.IsChecked = true;
                    return;
                }
                if (value.SequenceEqual(new string[] { "*.pdf" }))
                {
                    rbFilterPdfs.IsChecked = true;
                    return;
                }

                rbFilterIndividual.IsChecked = true;
                txtFilter.Text = string.Join(", ", value);
            }
        }

        internal void DisablePath()
        {
            txtFolder.IsEnabled = false;
            btnOpen.IsEnabled = false;
        }

        public NameType Naming
        {
            get
            {
                if (rbFolderName.IsChecked == true)
                    return NameType.NameFromFolderName;

                if (rbRelativeName.IsChecked == true)
                    return NameType.NameFromRelativePath;

                throw new NotImplementedException();
            }
            set
            {
                switch(value)
                {
                    case NameType.NameFromFolderName:
                        rbFolderName.IsChecked = true;
                        break;
                    case NameType.NameFromRelativePath:
                        rbRelativeName.IsChecked = true;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }





        private void replaceName(System.Windows.Controls.RadioButton rb, string value)
        {
            var text = (string)rb.Content;
            int s = text.IndexOf("(");
            int e = text.LastIndexOf(")");
            if ((e < s) || (s < 0)) return;
            text = text.Substring(0, s + 1) + value + text.Substring(e);
            rb.Content = text;
        }
    }
}
