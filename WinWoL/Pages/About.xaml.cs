using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Net.Http;
using System.Threading;
using Windows.ApplicationModel;

namespace WinWoL.Pages
{
    public sealed partial class About : Page
    {
        private DispatcherQueue _dispatcherQueue;
        public About()
        {
            this.InitializeComponent();

            // 在构造函数或其他适当位置设置版本号
            var package = Package.Current;
            var version = package.Id.Version;
            // 获取UI线程的DispatcherQueue
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            APPVersion.Content = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            APPVersion.NavigateUri = new System.Uri($"https://github.com/Direct5dom/WinWoL/releases/tag/{version.Major}.{version.Minor}.{version.Build}.{version.Revision}");

            GetSponsorList();
        }

        private void AboutAliPay_Click(object sender, RoutedEventArgs e)
        {
            AboutAliPayTips.IsOpen = true;
        }
        private void AboutWePay_Click(object sender, RoutedEventArgs e)
        {
            AboutWePayTips.IsOpen = true;
        }
        private void GetSponsorList()
        {
            // 在子线程中执行任务
            Thread subThread = new Thread(new ThreadStart(async () =>
            {
                string nameList;
                using (HttpClient client = new HttpClient())
                {
                    // 发起GET请求以获取文件内容
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync($"https://raw.githubusercontent.com/Direct5dom/Direct5dom/main/README/Sponsor/List");
                        if (response.IsSuccessStatusCode)
                        {
                            // 从响应中读取文件内容
                            nameList = await response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            nameList = "Unable to connect to Github";
                        }
                    }
                    catch
                    {
                        nameList = "Unable to connect to Github";
                    }
                }
                _dispatcherQueue.TryEnqueue(() =>
                {
                    NameList.Text = nameList;
                });
            }));
            subThread.Start();
        }
    }
}
