using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using System.Threading;
using WinWoL.Methods;
using WinWoL.Models;

namespace WinWoL.Pages.Dialogs
{
    public sealed partial class PingTools : ContentDialog
    {
        private DispatcherQueue _dispatcherQueue;
        public WoLModel WoLData { get; private set; }
        public PingTools(WoLModel wolModel)
        {
            this.InitializeComponent();

            // 获取UI线程的DispatcherQueue
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            if (wolModel.IPAddress != null)
            {
                // 在子线程中执行任务
                Thread subThread = new Thread(new ThreadStart(() =>
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        PingRef.Text = "测试中";
                    });
                    string PingRes;
                    while (true)
                    {
                        if (WoLMethod.PingTest(wolModel.IPAddress) == "TimedOut")
                        {
                            PingRes = "超时";
                        }
                        else
                        {
                            PingRes = WoLMethod.PingTest(wolModel.IPAddress);
                        }

                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            PingRef.Text = PingRes;
                        });
                        Thread.Sleep(1000);
                    }
                }));
                subThread.Start();
                // 在子线程中执行任务
                Thread subThread2 = new Thread(new ThreadStart(() =>
                {
                    string WoLPingRes;
                    // 提供了WoL端口
                    if (wolModel.WoLPort != null && wolModel.WoLPort != "")
                    {
                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            WoLPingRef.Text = "测试中";
                        });
                        while (true)
                        {
                            if (WoLMethod.TCPingTest(wolModel.IPAddress, wolModel.WoLPort) == -1)
                            {
                                WoLPingRes = "超时";
                            }
                            else
                            {
                                WoLPingRes = $"{WoLMethod.TCPingTest(wolModel.IPAddress, wolModel.WoLPort)} ms";
                            }
                            _dispatcherQueue.TryEnqueue(() =>
                            {
                                WoLPingRef.Text = WoLPingRes;
                            });
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            WoLPingRef.Text = "不适用";
                        });
                    }

                }));
                subThread2.Start();
                // 在子线程中执行任务
                Thread subThread3 = new Thread(new ThreadStart(() =>
                {
                    string RDPPingRes;
                    // 提供了RDP端口
                    if (wolModel.RDPPort != null && wolModel.RDPPort != "")
                    {
                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            RDPPingRef.Text = "测试中";
                        });
                        while (true)
                        {
                            if (WoLMethod.TCPingTest(wolModel.IPAddress, wolModel.RDPPort) == -1)
                            {
                                RDPPingRes = "超时";
                            }
                            else
                            {
                                RDPPingRes = $"{WoLMethod.TCPingTest(wolModel.IPAddress, wolModel.WoLPort)} ms";
                            }
                            _dispatcherQueue.TryEnqueue(() =>
                            {
                                RDPPingRef.Text = RDPPingRes;
                            });
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            RDPPingRef.Text = "不适用";
                        });
                    }
                }));
                subThread3.Start();
                // 在子线程中执行任务
                Thread subThread4 = new Thread(new ThreadStart(() =>
                {
                    string SSHPingRes;
                    // 提供了SSH端口
                    if (wolModel.SSHPort != null && wolModel.SSHPort != "")
                    {
                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            SSHPingRef.Text = "测试中";
                        });
                        while (true)
                        {
                            if (WoLMethod.TCPingTest(wolModel.IPAddress, wolModel.SSHPort) == -1)
                            {
                                SSHPingRes = "超时";
                            }
                            else
                            {
                                SSHPingRes = $"{WoLMethod.TCPingTest(wolModel.IPAddress, wolModel.WoLPort)} ms";
                            }
                            _dispatcherQueue.TryEnqueue(() =>
                            {
                                SSHPingRef.Text = SSHPingRes;
                            });
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            SSHPingRef.Text = "不适用";
                        });
                    }
                }));
                subThread4.Start();
            }
        }
    }
}