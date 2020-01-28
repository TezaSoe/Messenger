using Messenger.ChatHelper;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Services
{
    class ClientService : IClientService
    {
        ////Connection to a SignalR server
        //HubConnection _signalRConnection;
        ////Proxy object for a hub hosted on the SignalR server
        //IHubProxy _hubProxy;
        ConnectionState currentConnectionState = ConnectionState.Disconnected;
        Dictionary<string, ServerInfo> _signalRConnectionList = new Dictionary<string, ServerInfo>();
        private List<string> MessageList = new List<string>();
        private ObservableCollection<string> PossibleSignalRHostIDList = new ObservableCollection<string>();
        private ObservableCollection<string> PossibleSignalRHostInfoList = new ObservableCollection<string>();
        private ConcurrentQueue<string> concurrentQueue = new ConcurrentQueue<string>();

        private readonly ParticipantInfo currentUserInfo;
        private readonly BackgroundWorker CheckingSignalRHostBackgroundWorker = null;
        private readonly BackgroundWorker ConcurrentQueueBackgroundWorker = null;
        //private bool CheckingSignalRHostRunWorkerCompleted = false;

        public static Action<string, string> ReceiveMessageFromServer { get; set; }
        public static Action<string, string> PossibleSignalRHostID { get; set; }
        public static Action<ParticipantInfo> PossibleSignalRHostInfo { get; set; }

        public ClientService(IServerService _iServerService)
        {
            currentUserInfo = _iServerService.getCurrentServerInfo();

            CheckingSignalRHostBackgroundWorker = new BackgroundWorker();
            CheckingSignalRHostBackgroundWorker.DoWork += new DoWorkEventHandler(CheckingSignalRHostOnTheSameNetwork_DoWork);
            CheckingSignalRHostBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CheckingSignalRHostOnTheSameNetwork_RunWorkerCompleted);

            ConcurrentQueueBackgroundWorker = new BackgroundWorker();
            ConcurrentQueueBackgroundWorker.DoWork += new DoWorkEventHandler(ConnectingSignalRHostOnTheSameNetwork_DoWork);

            PossibleSignalRHostIDList.CollectionChanged += PossibleSignalRHostInfoList_CollectionChanged;
            ServerService.StartCheckSignalRHost = (ServerIP, SelfIPList, AllLANIPList) =>
            {
                CheckingSignalRHostBackgroundWorker.RunWorkerAsync(new Tuple<string, List<string>, List<string>>(ServerIP, SelfIPList, AllLANIPList));
            };

            PossibleSignalRHostID = (connectionID, HostIp) =>
            {
                var existedIP = PossibleSignalRHostIDList.Where(d => d == HostIp).FirstOrDefault();
                if (existedIP == null)
                {
                    PossibleSignalRHostIDList.Add(HostIp);
                    concurrentQueue.Enqueue(HostIp);
                }
            };

            PossibleSignalRHostInfo = (participant) =>
            {
                //var existedIP = PossibleSignalRHostIDList.Where(d => d == serverip).FirstOrDefault();
                //if (existedIP == null)
                //{
                //    PossibleSignalRHostIDList.Add(serverip);
                //    concurrentQueue.Enqueue(serverip);
                //}
            };
        }

        private void PossibleSignalRHostInfoList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!ConcurrentQueueBackgroundWorker.IsBusy)
            {
                ConcurrentQueueBackgroundWorker.RunWorkerAsync();
            }
        }
        private async void ConnectingSignalRHostOnTheSameNetwork_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //while (!CheckingSignalRHostRunWorkerCompleted)
                //{
                //    Thread.Sleep(100);
                //}
                //_signalRConnectionList.Clear();
                while (concurrentQueue.Count > 0)
                {
                    string ServerIP = "";
                    if(concurrentQueue.TryDequeue(out ServerIP))
                    {
                        try
                        {
                            var result = await connectAsync(ServerIP);
                            if (result.Item1)
                            {
                                _signalRConnectionList[ServerIP] = result.Item2;
                                _signalRConnectionList[ServerIP].ReceiveMessageForEachServer = ReceiveMessageFromServer;
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void CheckingSignalRHostOnTheSameNetwork_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;
        }

        private async void CheckingSignalRHostOnTheSameNetwork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //CheckingSignalRHostRunWorkerCompleted = false;
                PossibleSignalRHostIDList.CollectionChanged -= PossibleSignalRHostInfoList_CollectionChanged;
                var tuple = (Tuple<string, List<string>, List<string>>)e.Result;
                string ServerIP = tuple.Item1;
                List<string> SelfIPList = tuple.Item2;
                List<string> AllLANIPList = tuple.Item3;
                //List<string> AllSignalRHostList = new List<string>();

                //var result = await connectAsync(currentUserInfo.UserIP);
                //if (result.Item1)
                //{
                //    _signalRConnectionList[currentUserInfo.UserIP] = result.Item2;
                //    _signalRConnectionList[currentUserInfo.UserIP].ReceiveMessageForEachServer = ReceiveMessageFromServer;
                //}
                int i = 0;
                PossibleSignalRHostIDList.Clear();
                foreach (string ip in AllLANIPList)
                {
                    i++;
                    var result = await checkPossibleSignalRHostAsync(ip);
                    if (result.Item1)
                    {
                        _signalRConnectionList[ip] = result.Item2;
                        _signalRConnectionList[ip].ReceiveMessageForEachServer = ReceiveMessageFromServer;
                    }
                    //bool? ret = ServerService.checkAppIsOpen(ip,1234);
                    //if(ret == null)
                    //{

                    //}
                    //else if(ret == true)
                    //{

                    //}
                    ServerService.FinishedCheckSignalRHostForEach(ServerIP, i, AllLANIPList.Count, _signalRConnectionList);
                }
                
                _signalRConnectionList.Clear();
                foreach (string serverip in PossibleSignalRHostIDList.Select(d => d).ToList())
                {
                    try
                    {
                        var result = await connectAsync(serverip);
                        if (result.Item1)
                        {
                            _signalRConnectionList[serverip] = result.Item2;
                            _signalRConnectionList[serverip].ReceiveMessageForEachServer = ReceiveMessageFromServer;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                ServerService.FinishedCheckSignalRHost?.Invoke(ServerIP, _signalRConnectionList);
                PossibleSignalRHostIDList.CollectionChanged += PossibleSignalRHostInfoList_CollectionChanged;
                //CheckingSignalRHostRunWorkerCompleted = true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        public ParticipantInfo getCurrentUserInfo()
        {
            return currentUserInfo;
        }

        private async Task<Tuple<bool, ServerInfo>> checkPossibleSignalRHostAsync(string serverIP)
        {
            bool isConnected = false;
            ServerInfo newServer = new ServerInfo(serverIP, currentUserInfo);
            try
            {
                //Connect to the server
                var task = newServer.signalRConnection.Start();
                if (await Task.WhenAny(task, Task.Delay(50)) == task)
                {
                    // task completed within timeout
                    // Very important in order to propagate exceptions
                    await task;
                }
                else
                {
                    // timeout logic
                    throw new TimeoutException();
                }

                ////Send user name for this client, so we won't need to send it with every message
                //await newServer.hubProxy?.Invoke("SetUserName", currentUserInfo);
                isConnected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ⇒⇒⇒ {ex.Message}");
            }
            return new Tuple<bool, ServerInfo>(isConnected, newServer);
        }

        private async Task<Tuple<bool, ServerInfo>> connectAsync(string serverIP)
        {
            bool isConnected = false;
            ServerInfo newServer = new ServerInfo(serverIP, currentUserInfo);
            try
            {
                //Connect to the server
                await newServer.signalRConnection.Start();

                //Send user name for this client, so we won't need to send it with every message
                await newServer.hubProxy?.Invoke("SetUserName", currentUserInfo);
                isConnected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ⇒⇒⇒ {ex.Message}");
            }
            return new Tuple<bool, ServerInfo>(isConnected, newServer);
        }

        //public async Task<bool> connectAsync(ParticipantInfo participant)
        //{
        //    bool isConnected = false;
        //    string url = string.Format("http://{0}:{1}", participant.UserIP, "1234");
        //    //Create a connection for the SignalR server
        //    _signalRConnection = new HubConnection(url);
        //    _signalRConnection.StateChanged += HubConnection_StateChanged;

        //    //Get a proxy object that will be used to interact with the specific hub on the server
        //    //Ther may be many hubs hosted on the server, so provide the type name for the hub
        //    _hubProxy = _signalRConnection.CreateHubProxy("ChatHub");

        //    //Reigster to the "AddMessage" callback method of the hub
        //    //This method is invoked by the hub
        //    _hubProxy.On<string, string>("AddMessage", (name, message) =>
        //    {
        //        //writeToLog($"{name}:{message}");
        //        ReceiveMessageFromServer?.Invoke(name, message);
        //    });

        //    try
        //    {
        //        //Connect to the server
        //        await _signalRConnection.Start();

        //        //Send user name for this client, so we won't need to send it with every message
        //        await _hubProxy.Invoke("SetUserName", participant);
        //        isConnected = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        //writeToLog($"Error:{ex.Message}");
        //    }
        //    return isConnected;
        //}

        private void HubConnection_StateChanged(StateChange obj)
        {
            currentConnectionState = obj.NewState;
        }

        public ConnectionState getCurrentConnectionState()
        {
            return currentConnectionState;
        }

        public List<string> getMessageList()
        {
            return MessageList;
        }
    }
}
