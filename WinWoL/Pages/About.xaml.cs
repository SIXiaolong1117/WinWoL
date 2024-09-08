using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Gaming.Preview.GamesEnumeration;
using Windows.Media.Protection.PlayReady;
using static System.Net.WebRequestMethods;

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

            APPVersion.Text = $"{version.Major}.{version.Minor}.{version.Build}";
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GetList();
        }
        private void AboutAliPay_Click(object sender, RoutedEventArgs e)
        {
            AboutAliPayTips.IsOpen = true;
        }
        private void AboutWePay_Click(object sender, RoutedEventArgs e)
        {
            AboutWePayTips.IsOpen = true;
        }
        private async Task<string> HTTPResponse(string http)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(http);
                if (response.IsSuccessStatusCode)
                {
                    // 从GitHub的响应中读取文件内容
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return "";
                }
            }
        }
        private async void GetList()
        {
            string nameList = null;
            string stringList = null;
            try
            {
                nameList = await HTTPResponse("https://raw.githubusercontent.com/SIXiaolong1117/SIXiaolong1117/main/README/Sponsor/List");
            }
            catch (Exception ex)
            {
                try
                {
                    nameList = await HTTPResponse("https://gitee.com/XiaolongSI/SIXiaolong1117/raw/main/README/Sponsor/List");
                }
                catch (Exception ex2)
                {
                    nameList = "无法连接至 Github 或 Gitee。";
                }
            }
            try
            {
                stringList = await HTTPResponse("https://raw.githubusercontent.com/SIXiaolong1117/SIXiaolong1117/main/README/Text/List");
            }
            catch (Exception ex)
            {
                try
                {
                    stringList = await HTTPResponse("https://gitee.com/XiaolongSI/SIXiaolong1117/raw/main/README/Text/List");
                }
                catch (Exception ex2)
                {
                    stringList = "";
                }
            }

            string randomLine = null;
            try
            {
                // 使用换行符分割字符串成数组
                string[] lines = stringList.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                // 使用随机数生成器生成一个随机索引
                Random rand = new Random();
                int randomIndex = rand.Next(0, lines.Length);

                // 随机选择一个字符串
                randomLine = lines[randomIndex];
            }
            catch (Exception ex) { }

            NameList.Text = nameList;
            TipsTips.Text = randomLine;



            //// 在子线程中执行任务
            //Thread subThread = new Thread(new ThreadStart(async () =>
            //{
            //    string nameList = null;
            //    using (HttpClient client = new HttpClient())
            //    {
            //        // 发起GET请求以获取文件内容
            //        // 首先尝试从GitHub获取数据
            //        try
            //        {
            //            HttpResponseMessage response = await client.GetAsync($"https://raw.githubusercontent.com/SIXiaolong1117/SIXiaolong1117/main/README/Sponsor/List");
            //            if (response.IsSuccessStatusCode)
            //            {
            //                // 从GitHub的响应中读取文件内容
            //                nameList = await response.Content.ReadAsStringAsync();
            //            }
            //            else
            //            {
            //                nameList = "Try Gitee";
            //            }
            //        }
            //        catch
            //        {
            //            nameList = "Try Gitee";
            //        }

            //        // 如果GitHub通信失败，尝试从Gitee获取数据
            //        if (nameList == "Try Gitee")
            //        {
            //            try
            //            {
            //                HttpResponseMessage response = await client.GetAsync($"https://gitee.com/XiaolongSI/SIXiaolong1117/raw/main/README/Sponsor/List");
            //                if (response.IsSuccessStatusCode)
            //                {
            //                    // 从Gitee的响应中读取文件内容
            //                    nameList = await response.Content.ReadAsStringAsync();
            //                }
            //                else
            //                {
            //                    nameList = "无法连接至 Github 或 Gitee 获取赞助者名单。(0)";
            //                }
            //            }
            //            catch
            //            {
            //                nameList = "无法连接至 Github 或 Gitee 获取赞助者名单。(1)";
            //            }
            //        }
            //    }
            //    _dispatcherQueue.TryEnqueue(() =>
            //    {
            //        NameList.Text = nameList;
            //    });
            //}));
            //subThread.Start();
        }
    }
}
