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
using System.Net;
using System.Windows.Shapes;

namespace WPF_Async_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {   
            executeSync.IsEnabled= false;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            RunDownLoadSync();

            watch.Stop();
            double elapsedS = watch.ElapsedMilliseconds / 1000.0;

            resultSync.Text += $"Total exec time: {elapsedS} seconds";
            executeSync.IsEnabled = true;

        }

        private List<string> PrepData()
        {
            List<string> websites = new List<string>();


            websites.Add("https://www.yahoo.com");
            websites.Add("https://www.google.com");
            websites.Add("https://www.stackoverflow.com");
            websites.Add("https://www.icrontech.com");

            return websites;
        }

        private void RunDownLoadSync()
        {
            resultSync.Text = "";

            var websites = PrepData();
            //Console.WriteLine(websites);

            foreach (var site in websites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
               // Console.WriteLine(results.WebsiteData.Length);
                resultSync.Text += $"{results.WebsiteUrl} downloaded: {results.WebsiteData.Length} characters long. \n";
            } 

        }

        private WebsiteDataModel DownloadWebsite(string siteUrl)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = siteUrl;
            output.WebsiteData = client.DownloadString(siteUrl);
            return output;
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            resultAsync.Text = "";

            await RunDownLoadParallelAsync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds / 1000.0;

            resultAsync.Text += $"Total exec time: {elapsedMs} seconds";
        }

        private async Task RunDownLoadAsync()
        {

            var websites = PrepData();
            //Console.WriteLine(websites);

            foreach ( var site in websites)
            {
                WebsiteDataModel results = await Task.Run(() => DownloadWebsite(site));
                // Console.WriteLine(results.WebsiteData.Length);
                resultAsync.Text += $"{results.WebsiteUrl} downloaded: {results.WebsiteData.Length} characters long. \n";
            }

        }
        private async Task RunDownLoadParallelAsync()
        {

            var websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();
            //Console.WriteLine(websites);

            foreach ( var site in websites)
            {

                tasks.Add(Task.Run(() => DownloadWebsite(site)));

                //WebsiteDataModel results = await Task.Run(() => DownloadWebsite(site));
                // Console.WriteLine(results.WebsiteData.Length);
                //resultAsync.Text += $"{results.WebsiteUrl} downloaded: {results.WebsiteData.Length} characters long. \n";
            }

            var results = await Task.WhenAll(tasks);

            foreach (var item in results) 
            {
                resultAsync.Text += $"{item.WebsiteUrl} downloaded: {item.WebsiteData.Length} characters long. \n";

            }

        }

    }
}
