using Archiv10.Application.Shared.BO;
using Archiv10.Application.Shared.Locator;
using Archiv10.Interfaces.ServiceConfigurationDialog;
using Archiv10.Interfaces.SharedDialogs;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServiceConfigurationDialog
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow()
        {
            InitializeComponent();
            log.Info("Initialize Application");
            ApplicationLocator.GetApplication().ReadConfig(); //Read Config

            lbRepositories.ItemsSource = ApplicationLocator.GetApplication().RepositoryList;
            lbRootFolder.ItemsSource = ApplicationLocator.GetApplication().RootFolderList;

            UpdateRepositoryStates();


            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            backgroundWorker.ProgressChanged += BackgroundWorkerCycleUpdate;
            backgroundWorker.RunWorkerAsync();
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void mnuAbount_Click(object sender, RoutedEventArgs e)
        {
            AbountBox dialog = new AbountBox();
            dialog.ShowDialog();
        }

        private void btnNewRepository_Click(object sender, RoutedEventArgs e)
        {
            RepositoryDialog dialog = new RepositoryDialog();
            if (dialog.ShowDialog() == true)
            {
                ApplicationLocator.GetApplication().AddRepository(dialog.Url, dialog.RepositoryName, dialog.Token);
            }
        }

        private void btnNewRootFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new RootFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                ApplicationLocator.GetApplication().AddRootFolder(dialog.SelectedPath, dialog.Filter, dialog.Naming);
                UpdateRepositoryStates();
            }
        }

        private int selectedRootFolderIndex()
        {
            var item = lbRootFolder.SelectedItem as UIRootFolder;
            return ApplicationLocator.GetApplication().RootFolderList.IndexOf(item);
        }

        private void MenuItemDetails_Click(object sender, RoutedEventArgs e)
        {
            var index = selectedRootFolderIndex();
            if (index == -1) return;

            var dialog = new LocalFolderDetails();
            ApplicationLocator.GetApplication().ReadRootFolderTree(index);

            dialog.ShowDialog();
          
        }
        private void MenuItemProperties_Click(object sender, RoutedEventArgs e)
        {
            var index = selectedRootFolderIndex();
            if (index == -1) return;

            var dialog = new RootFolderDialog();
            var rootfolder = ApplicationLocator.GetApplication().RootFolderList[index].Source;
            dialog.SelectedPath = rootfolder.Path;
            dialog.Naming = rootfolder.Naming;
            dialog.Filter = rootfolder.Filter;
            dialog.DisablePath();
            if (dialog.ShowDialog() == true)
            {
                ApplicationLocator.GetApplication().UpdateRootFolder(dialog.SelectedPath, dialog.Filter, dialog.Naming);
                UpdateRepositoryStates();
            }

        }

        private void UpdateRepositoryStates()
        {
            //Update
            ApplicationLocator.GetApplication().ReadRepositories();
            ApplicationLocator.GetApplication().ReadStatus();

            //refresh
            CollectionViewSource.GetDefaultView(ApplicationLocator.GetApplication().RepositoryList).Refresh();
        }


        private void BackgroundWorkerCycleUpdate(object sender, ProgressChangedEventArgs e)
        {
            //Read ServiceStatus
            ServiceStatus state = ApplicationLocator.GetApplication().ReadServiceStatus();
            if(state == null)
            {
                tbServiceStatus.Text = "Could not read state of service";
                return;
            }
            tbServiceStatus.Text = state.State;
            tbServiceMessage.Text  = state.Message;
            tbServiceDate.Text = state.Date.ToString();
            pathHeart.Fill = state.Working ? Brushes.Green : Brushes.Gray;

            //ReadRootFolderStatus
            if (ApplicationLocator.GetApplication().UpdateSyncFolderStatus())
            {
                CollectionViewSource.GetDefaultView(ApplicationLocator.GetApplication().RootFolderList).Refresh();
            }

        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
            {
                //Do your stuff here
                Thread.Sleep(1000);
                worker.ReportProgress(0);
            }
        }


    }
}
