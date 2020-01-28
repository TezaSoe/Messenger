using Messenger.ChatHelper;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Services
{
    class ServerService : IServerService
    {
        private IDisposable _signalR;
        private readonly ParticipantInfo currentServerInfo;
        private readonly string _serverip = "";
        private readonly string _port = "1234";
        private readonly string _url = "";
        private readonly BackgroundWorker ScanningIPBackgroundWorker = null;
        private bool canServerStart = false;

        public static Action StartScanIP { get; set; }
        public static Action<string,List<string>,List<string>> FinishScanIP { get; set; }
        public static Action<string, List<string>, List<string>> StartCheckSignalRHost { get; set; }
        public static Action<string, Dictionary<string, ServerInfo>> FinishedCheckSignalRHost { get; set; }
        public static Action<string, int, int, Dictionary<string, ServerInfo>> FinishedCheckSignalRHostForEach { get; set; }

        //// For Singleton Instance, not quite as lazy, but thread-safe without using locks
        //private static readonly MainFormService instance = instance ?? new MainFormService();
        //public static MainFormService Instance
        //{
        //    get
        //    {
        //        return instance;
        //    }
        //}

        public ServerService()
        {
            var HostName = Dns.GetHostName();
            var IPAddress = _serverip = Dns.GetHostEntry(HostName).AddressList
                            .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            .ToString();
            var UserName = Environment.UserName;
            var DomainName = Environment.UserDomainName;
            var ComputerName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            _url = $"http://{IPAddress}:{_port}";

            currentServerInfo = new ParticipantInfo
            {
                UserIP = IPAddress,
                UserHost = HostName,
                UserLogInName = ComputerName,
                UserName = UserName,
                GroupName = DomainName
            };

            SelfIPList.Add(IPAddress);
            //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
            //{
            //    SendARP();
            //}, null);

            if (canServerStart)
            {
                ScanningIPBackgroundWorker = new BackgroundWorker();
                ScanningIPBackgroundWorker.DoWork += new DoWorkEventHandler(ScanningIPOnTheSameNetwork_DoWork);
                ScanningIPBackgroundWorker.RunWorkerAsync();
            }
        }

        private void ScanningIPOnTheSameNetwork_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SendARP();
            }
            catch (Exception)
            {
            }
        }

        public bool ServerStart()
        {
            restart:
            try
            {
                //Start SignalR server with the give URL address
                //Final server address will be "URL/signalr"
                //Startup.Configuration is called automatically
                _signalR = WebApp.Start<Startup>(_url);
                canServerStart = true;
            }
            catch (TargetInvocationException tie)
            {
                try
                {
                    string command = $"/C netsh http add urlacl {_url}/ user=EveryOne";//⇒/C is to close commandprompt and /K is to alive commandprompt
                    var info = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "CMD.exe",
                        Arguments = command,
                        UseShellExecute = true,
                        Verb = "runas", // indicates to eleavate privileges
                    };

                    //Process.Start("CMD.exe", "/C ipconfig");
                    var process = new Process
                    {
                        EnableRaisingEvents = true, // enable WaitForExit()
                        StartInfo = info
                    };

                    process.Start();
                    process.WaitForExit(); // sleep calling process thread until evoked process exit
                    goto restart;
                }
                catch (Exception e)
                {

                }
            }
            catch (HttpListenerException hle)
            {
                //MessageBox.Show(hle.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return canServerStart;
        }

        public bool ServerStop()
        {
            bool canStop = false;
            try
            {
                ChatHub.ClearState();

                if (_signalR != null)
                {
                    _signalR.Dispose();
                    _signalR = null;
                }
                canStop = true;
            }
            catch (Exception e)
            {
            }
            return canStop;
        }

        public string getServerIP()
        {
            return _serverip;
        }

        public string getUrl()
        {
            return _url;
        }

        public ParticipantInfo getCurrentServerInfo()
        {
            return currentServerInfo;
        }

        public void SendMessage(SendMessageType messageType, string message, string servername, ParticipantInfo participant)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            switch (messageType)
            {
                case SendMessageType.All:
                    hubContext.Clients.All.addMessage(servername, message);
                    break;
                case SendMessageType.Group:
                    hubContext.Clients.Group(participant.GroupName).addMessage(servername, message);
                    break;
                case SendMessageType.Client:
                    hubContext.Clients.Client(participant.UserId).addMessage(servername, message);
                    break;
            }
        }

        #region ARP -a Send
        private List<string> _selfIPList = new List<string>();
        public List<string> SelfIPList { get { return this._selfIPList; } set { this._selfIPList = value; } }

        private List<string> _allLANIPList = new List<string>();
        public List<string> AllLANIPList { get { return this._allLANIPList; } set { this._allLANIPList = value; } }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int destinationIP, int sourceIP, byte[] macAddressPointer, ref int physicalAddressLength);
        public void SendARP()
        {
            AllLANIPList.Clear();
            // デフォルトだとスレッド起動に時間がかかるため
            int workThreadsMin;
            int ioThreadsMin;
            ThreadPool.GetMinThreads(out workThreadsMin, out ioThreadsMin);
            ThreadPool.SetMinThreads(260, ioThreadsMin);
            //StartScanIP?.Invoke();
            // 1～254のホストへARP送信
            List<Task> allTasks = new List<Task>();

            // 自ネットワークを取得
            List<string> networkPartList = new List<string>();
            for (int i = 0; i < SelfIPList.Count; i++)
            {
                List<string> separateIP = SelfIPList[i].Split('.').ToList();
                separateIP.RemoveAt(3);
                string network = string.Join(".", separateIP);
                networkPartList.Add(network);
            }

            for (int i = 0; i < networkPartList.Count; i++)
            {
                string networkPart = networkPartList[i];
                for (int j = 1; j <= 254; j++)
                {
                    int hostPart = j;
                    allTasks.Add(Task.Run(() =>
                    {

                        // ネットワーク部 + ホスト部でLocalIPへARPを投げる
                        string destinationIP = networkPart + "." + hostPart;
                        int destinationIPBytes = BitConverter.ToInt32(IPAddress.Parse(destinationIP).GetAddressBytes(), 0);
                        byte[] macAddressPointer = new byte[6];
                        int physicalAddressLength = macAddressPointer.Length;

                        // ARP
                        int ret = SendARP(destinationIPBytes, 0, macAddressPointer, ref physicalAddressLength);
                        if (ret == 0)
                        {
                            // デバッグ用
                            AllLANIPList.Add(destinationIP);
                        }
                    }
                    ));
                }

                Task t = Task.WhenAll(allTasks);
                try
                {
                    t.Wait();
                }
                catch
                {
                    //MessageBox.Show("エラー終了");
                }

                if (t.Status == TaskStatus.RanToCompletion)
                {
                    // デバッグ用
                    //Debug.WriteLine("end.");
                }
                else if (t.Status == TaskStatus.Faulted)
                {
                    //Debug.WriteLine("erro end.");
                }
            }
            FinishScanIP?.Invoke(_serverip, SelfIPList, AllLANIPList);
        }

        private void SetSelfLocalIP()
        {
            // WiFi,LANアダプタ等のインタフェースをすべて取得
            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var interfaces in nis)
            {
                // 接続できて、ループバックインタフェース、トンネルインタフェースを除く
                if (interfaces.OperationalStatus == OperationalStatus.Up
                    && interfaces.NetworkInterfaceType != NetworkInterfaceType.Loopback
                    && interfaces.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                {
                    // インタフェースの中からさらにIPを見ていく
                    UnicastIPAddressInformationCollection unicastIPs = interfaces.GetIPProperties().UnicastAddresses;
                    foreach (var ip in unicastIPs)
                    {
                        // このPCのIPv4を登録
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            SelfIPList.Add(Convert.ToString(ip.Address));
                        }
                    }
                }
            }
        }

        public static bool? checkAppIsOpen(string ip, int port)
        {
            bool? ret = null;
            IPAddress ipa = (IPAddress)Dns.GetHostAddresses(ip)[0];

            try
            {
                System.Net.Sockets.Socket sock = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                sock.Connect(ipa, port);
                if (sock.Connected == true)  // Port is in use and connection is successful
                    ret = true;
                    //Console.WriteLine("Port is Open!");
                sock.Close();

            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (ex.ErrorCode == 10061)  // Port is unused and could not establish connection 
                    ret = false;
                //Console.WriteLine("Port is Closed");
                else
                    Console.WriteLine(ex.Message);
            }
            return ret;
        }
        #endregion

        #region Signal.R Host Check
        private List<string> _allSignalRHostList = new List<string>();
        public List<string> AllSignalRHostList { get { return this._allSignalRHostList; } set { this._allSignalRHostList = value; } }

        #endregion
    }
}
