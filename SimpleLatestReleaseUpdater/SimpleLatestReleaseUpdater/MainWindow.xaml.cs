using Ionic.Zip;
using SimpleLatestReleaseUpdater.Models;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleLatestReleaseUpdater
{
    public partial class MainWindow : Window
    {
        ExeConfigurationFileMap configMap;
        Configuration config;

        private string username;
        private string repository;
        private string updateNamePrefix;
        private string installPath;
        private string downloadPath;

        private MainViewModel mainViewModel = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();

            configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = "config.xml";
            try
            {
                config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            }
            catch (Exception)
            {
                mainViewModel.Text = "Error: Could not load config";
                Exit();
            }

            this.DataContext = this.mainViewModel;
            this.username = config.AppSettings.Settings["Username"].Value;
            this.repository = config.AppSettings.Settings["Repository"].Value;
            this.updateNamePrefix = config.AppSettings.Settings["UpdateNamePrefix"].Value;
            this.installPath = config.AppSettings.Settings["InstallPath"].Value;
            this.downloadPath = config.AppSettings.Settings["DownloadPath"].Value;
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            string downloadUrl;
            mainViewModel.Text = "Fetching update...";
            mainViewModel.Progress = 0;
            downloadUrl = await UpdateGetter.GetLatestReleaseLink(username, repository, updateNamePrefix);
            if (downloadUrl.Contains("Error"))
            {
                mainViewModel.Text = downloadUrl;
                Exit();
            }
            mainViewModel.Text = "Downloading...";
            DownloadUpdate(downloadUrl);
        }

        public void DownloadUpdate(string address)
        {
            WebClient client = new WebClient();
            Uri uri = new Uri(address);

            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
            client.DownloadFileCompleted += Client_DownloadFileCompleted;
            try
            {
                client.DownloadFileAsync(uri, downloadPath);
            }
            catch (Exception)
            {
                mainViewModel.Text = "Download failed";
                Exit();
            }
        }

        private async void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            mainViewModel.Progress = 0;
            mainViewModel.Text = "Installing...";
            await Task.Delay(100);
            Extract(downloadPath, installPath, mainViewModel);
        }

        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            mainViewModel.Progress = e.ProgressPercentage;
        }

        private void Extract(string zipPath, string directoryPath, MainViewModel model)
        {
            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                zip.ExtractProgress += new EventHandler<ExtractProgressEventArgs>(zip_ExtractProgress);

                try
                {
                    zip.ExtractAll(directoryPath, ExtractExistingFileAction.OverwriteSilently);
                }
                catch (Exception)
                {
                    mainViewModel.Text = "Installation failed";
                    Exit();
                }

            }

            void zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
            {
                if (e.TotalBytesToTransfer > 0)
                {
                    model.Progress = (100 * e.BytesTransferred / e.TotalBytesToTransfer);
                }
                else
                {
                    model.Text = "Done";
                    Exit();
                }
            }
        }

        private async void Exit()
        {
            await Task.Delay(1200);
            System.Windows.Application.Current.Shutdown();
        }
    }
}
