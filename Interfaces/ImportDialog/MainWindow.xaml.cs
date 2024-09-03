using Archiv10.Application.Shared;
using Archiv10.Application.Shared.BO;
using Archiv10.Application.Shared.Locator;
using Archiv10.Domain.Shared.BO;
using Archiv10.Interfaces.ImportDialog;
using Archiv10.Interfaces.SharedDialogs;
using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace ImportDialog
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
            lbBags.ItemsSource = ApplicationLocator.GetApplication().BagList;
            lbData.ItemsSource = ApplicationLocator.GetApplication().DataList;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lbBags.ItemsSource);
            view.Filter = BagsFilter;

            CollectionView view2 = (CollectionView)CollectionViewSource.GetDefaultView(lbData.ItemsSource);
            view2.Filter = DataFilter;

            btnFilter.IsEnabled = false;
            btnDataFilter.IsEnabled = false;
            btnAddBag.IsEnabled = false;
            btnAddData.IsEnabled = false;
            btnConnectRepository.IsEnabled = ApplicationLocator.GetApplication().RepositoryList.Count > 0;
        }






        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void mnuAbount_Click(object sender, RoutedEventArgs e)
        {
            AbountBox dialog = new AbountBox();
            dialog.ShowDialog();
        }


        private void btnConnectRepository_Click(object sender, RoutedEventArgs e)
        {
            //Update
            ApplicationLocator.GetApplication().ReadRepositories();
            ApplicationLocator.GetApplication().ReadStatus();

            //refresh
            CollectionViewSource.GetDefaultView(ApplicationLocator.GetApplication().RepositoryList).Refresh();
            CollectionViewSource.GetDefaultView(ApplicationLocator.GetApplication().BagList).Refresh();

            //Update ButtonState
            foreach (var repo in ApplicationLocator.GetApplication().RepositoryList)
            {
                if (repo.State != RepositoryState.Connected) continue;
                btnAddBag.IsEnabled = true;
                break;
            }
        }

        private void btnNewRepository_Click(object sender, RoutedEventArgs e)
        {
            RepositoryDialog dialog = new RepositoryDialog();
            if (dialog.ShowDialog() == true)
            {
                ApplicationLocator.GetApplication().AddRepository(dialog.Url, dialog.RepositoryName, dialog.Token);
                btnConnectRepository.IsEnabled = true;
            }
        }


        private bool Match(string SearchString, string txtfilter)
        {
            if (String.IsNullOrEmpty(txtfilter))
                return true;

            return SearchString.IndexOf(txtfilter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool DataFilter(object item)
        {
              return Match((item as UIData).SearchString, txtDataFilter.Text);
        }


        private void txtDataFilterChanged(object sender, RoutedEventArgs e)
        {
            var txt = txtDataFilter.Text;
            btnDataFilter.IsEnabled = !string.IsNullOrEmpty(txt);
            CollectionViewSource.GetDefaultView(lbData.ItemsSource).Refresh();
        }

        private void btnDataFilter_Click(object sender, RoutedEventArgs e)
        {
            txtDataFilter.Clear();
        }


        private bool BagsFilter(object item)
        {
            return Match((item as UIBagSnippet).SearchString, txtFilter.Text);
        }

        private void txtFilterChanged(object sender, RoutedEventArgs e)
        {
            var txt = txtFilter.Text;
            btnFilter.IsEnabled = !string.IsNullOrEmpty(txt);
            CollectionViewSource.GetDefaultView(lbBags.ItemsSource).Refresh();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            txtFilter.Clear();
        }


        private int selectedBagIndex()
        {
            var item = lbBags.SelectedItem as UIBagSnippet;
            return ApplicationLocator.GetApplication().BagList.IndexOf(item);
        }

        private void lbBags_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var index = selectedBagIndex();
            ApplicationLocator.GetApplication().ReadFiles(index);
            btnAddData.IsEnabled = true;
        }


        private void MenuItemSyncBag_Click(object sender, RoutedEventArgs e)
        {
            var index = selectedBagIndex();
            if (index == -1) return;

            var application = ApplicationLocator.GetApplication();
            var bag = application.BagList[index];

            try
            {
                ApplicationLocator.GetApplication().SyncBag(bag);

            }
            catch (Exception exc)
            {
                log.Error(exc);
                ErrorBox dialog = new ErrorBox(exc);
                dialog.ShowDialog();
            }

            //Update
            ApplicationLocator.GetApplication().ReadRepositories();
            ApplicationLocator.GetApplication().ReadStatus();
            //refresh
            CollectionViewSource.GetDefaultView(ApplicationLocator.GetApplication().RepositoryList).Refresh();

            //Select Index
            for (int i = 0; i < application.BagList.Count; i++)
            {
                if (application.BagList[i].SourceId.ToString() == bag.SourceId.ToString())
                {
                    lbBags.SelectedIndex = i;
                    return;
                }
            }
            throw new Exception("Never reaching position");

        }


        private void btnAddBag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BagDialog dialog = new BagDialog();
                if (dialog.ShowDialog() == true)
                {
                    var bagid = BagId.CreateId();
                    ApplicationLocator.GetApplication().CreateBag(bagid, dialog.Description);
                    ApplicationLocator.GetApplication().ReadRepositories();
                    ApplicationLocator.GetApplication().ReadStatus();
                    //refresh
                    CollectionViewSource.GetDefaultView(ApplicationLocator.GetApplication().RepositoryList).Refresh();
                }
            }
            catch (Exception exc)
            {
                log.Error(exc);
                ErrorBox dialog = new ErrorBox(exc);
                dialog.ShowDialog();
            }
        }


        private void btnAddData_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                var index = selectedBagIndex();

                var filenames = openFileDialog.FileNames;

                int[] partsForFile = new int[filenames.Length];
                int[] transferedPartsForFile = new int[filenames.Length];

                for (int i = 0; i < filenames.Length; i++)
                {
                    transferedPartsForFile[i] = 0;
                    var app = ApplicationLocator.GetApplication();
                    partsForFile[i] = app.CountRepositories(index) * app.CountPartsOfFile(filenames[i]);
                }

                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                worker.DoWork += (object _sender, DoWorkEventArgs _e) =>
                {
                    (_sender as BackgroundWorker).ReportProgress(0);
                    for (int i = 0; i < filenames.Length; i++)
                    {
                        ApplicationLocator.GetApplication().AppendFile(index, filenames[i],
                            () =>
                            {
                                transferedPartsForFile[i]++;

                                //calulate percentage
                                int sumParts = partsForFile.Sum();
                                int transferedParts = transferedPartsForFile.Sum();
                                int percentage = (100 * transferedParts) / sumParts;

                                (_sender as BackgroundWorker).ReportProgress(percentage);
                                return null;
                            });
                    }
                };

                worker.ProgressChanged += (object _sender, ProgressChangedEventArgs _e) =>
                {
                    pbTransfer.Value = _e.ProgressPercentage;
                };

                worker.RunWorkerCompleted += (object _sender, RunWorkerCompletedEventArgs _e) =>
                {

                    if (_e.Error != null)
                    {
                        log.Error(_e.Error);
                        ErrorBox dialog = new ErrorBox(_e.Error);
                        dialog.ShowDialog();
                    }

                    ApplicationLocator.GetApplication().ReadFiles(index);
                    ApplicationLocator.GetApplication().ReadStatus();
                    //refresh
                    CollectionViewSource.GetDefaultView(ApplicationLocator.GetApplication().RepositoryList).Refresh();

                };

                worker.RunWorkerAsync();
            }

        }



        private void MenuItemDownloadData_Click(object sender, RoutedEventArgs e)
        {
            var index = lbData.SelectedIndex;

            if (index == -1) return;

            var application = ApplicationLocator.GetApplication();
            var data = application.DataList[index];


            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathDownload = Path.Combine(pathUser, "Downloads");

            var totalBytes = data.SourceData.Length;


            if (!Directory.Exists(pathDownload))
            {
                MessageBox.Show("Verzeichnis " + pathDownload + " existiert nicht");
                return;
            }

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += (object _sender, DoWorkEventArgs _e) =>
            {
                (_sender as BackgroundWorker).ReportProgress(0);
                var _data = _e.Argument as UIData;

                application.DownloadFile(_data, pathDownload, (long transferedBytes) =>
                {
                    //calulate percentage
                    int percentage = (int)((100 * transferedBytes) / totalBytes);
                    (_sender as BackgroundWorker).ReportProgress(percentage);
                    return null;
                });
            };

            worker.ProgressChanged += (object _sender, ProgressChangedEventArgs _e) =>
            {
                pbTransfer.Value = _e.ProgressPercentage;
            };

            worker.RunWorkerCompleted += (object _sender, RunWorkerCompletedEventArgs _e) =>
            {

                if (_e.Error != null)
                {
                    log.Error(_e.Error);
                    ErrorBox dialog = new ErrorBox(_e.Error);
                    dialog.ShowDialog();
                }
                //Open Explorer
                Process.Start(pathDownload);
            };

            worker.RunWorkerAsync(data);

        }


        private void MenuItemSyncData_Click(object sender, RoutedEventArgs e)
        {
            var index = lbData.SelectedIndex;
            if (index == -1) return;

            var application = ApplicationLocator.GetApplication();
            var data = ApplicationLocator.GetApplication().DataList[index];


            var partsForFile = application.CountActiveRepositoriesToSync(data) * application.CountPartsOfFile(data);
            var transferedParts = 0;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += (object _sender, DoWorkEventArgs _e) =>
            {
                (_sender as BackgroundWorker).ReportProgress(0);
                var _data = _e.Argument as UIData;
                ApplicationLocator.GetApplication().SyncData(_data,
                     () =>
                     {
                         //incr transfered parts
                         transferedParts++;

                         //calulate percentage
                         int percentage = (100 * transferedParts) / partsForFile;
                         (_sender as BackgroundWorker).ReportProgress(percentage);

                         return null;
                     });
            };

            worker.ProgressChanged += (object _sender, ProgressChangedEventArgs _e) =>
            {
                pbTransfer.Value = _e.ProgressPercentage;
            };

            worker.RunWorkerCompleted += (object _sender, RunWorkerCompletedEventArgs _e) =>
            {

                if (_e.Error != null)
                {
                    log.Error(_e.Error);
                    ErrorBox dialog = new ErrorBox(_e.Error);
                    dialog.ShowDialog();
                }

                //Update
                for (int i = 0; i < application.BagList.Count; i++)
                {
                    if (application.BagList[i].SourceId.ToString() == data.SourceBag.ToString())
                    {
                        application.ReadFiles(i);
                    }

                }
                //Select Index
                for (var i = 0; i < application.DataList.Count; i++)
                {
                    if (application.DataList[i].SourceData.CheckSum == data.SourceData.CheckSum)
                    {
                        lbData.SelectedIndex = i;
                    }

                }

                //update
                ApplicationLocator.GetApplication().ReadStatus();
                //refresh
                CollectionViewSource.GetDefaultView(ApplicationLocator.GetApplication().RepositoryList).Refresh();
            };

            worker.RunWorkerAsync(data);


        }

    }
}
