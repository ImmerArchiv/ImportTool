using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml;

namespace Archiv10.Interfaces.SharedDialogs
{
    /// <summary>
    /// Interaction logic for AbountBox.xaml
    /// </summary>
    public partial class AbountBox : Window
    {
        public AbountBox()
        {
            InitializeComponent();

            tbVersion.Text = GetVersionText(tbVersion.Text);
        }

        private string GetVersionText(string pattern)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("ProductInfo.xml");

            var version = doc.DocumentElement.SelectSingleNode("/info/version").InnerText;
            var productname = doc.DocumentElement.SelectSingleNode("/info/productname").InnerText;

            return pattern.Replace("PRODUCTNAME", productname).Replace("VERSION", version);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
