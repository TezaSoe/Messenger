using Messenger.ChatHelper;
using Messenger.Enum;
using Messenger.Services;
using Messenger.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Application = System.Windows.Application;

namespace Messenger.ViewModels
{
    class MainFormViewModel : BindableBase, INavigationAware
    {
        private readonly IServerService iServerService;
        private readonly IClientService iClientService;
        private readonly bool IsServerStart = false;
        private List<ParticipantInfo> _clients = new List<ParticipantInfo>();
        private List<string> _groups = new List<string>();
        private readonly string serverName = "";
        IDialogService dialogService;

        private readonly BackgroundWorker ARPCommand = null;
        //private Action<string, string> DimBackgroundColor { get; set; }

        private ObservableCollection<ParticipantInfo> _participantList = new ObservableCollection<ParticipantInfo>();
        public ObservableCollection<ParticipantInfo> ParticipantList
        {
            get { return _participantList; }
            set { SetProperty(ref _participantList, value); }
        }

        private string _logStatus = "";
        public string LogStatus
        {
            get { return _logStatus; }
            set { SetProperty(ref _logStatus, value); }
        }

        private string _rightLogStatus = "Scaning IP in LAN.";
        public string RightLogStatus
        {
            get { return _rightLogStatus; }
            set { SetProperty(ref _rightLogStatus, value); }
        }

        private string _DimBackgroundWhileLoading = "Visible";
        public string DimBackgroundWhileLoading
        {
            get { return _DimBackgroundWhileLoading; }
            set { SetProperty(ref _DimBackgroundWhileLoading, value); }
        }

        private int _clientCount = 0;
        public int ClientCount
        {
            get { return _clientCount; }
            set { SetProperty(ref _clientCount, value); }
        }

        private string _message = "";
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private int _listViewSelectedIndex = -1;
        public int ListViewSelectedIndex
        {
            get { return _listViewSelectedIndex; }
            set
            {
                SetProperty(ref _listViewSelectedIndex, value);
            }
        }

        public MainFormViewModel(IServerService _iServerService, IClientService _iClientService, IDialogService _dialogService)
        {
            iServerService = _iServerService;
            iClientService = _iClientService;
            dialogService = _dialogService;
            //var serverService = (ServerService)_iServerService;
            IsServerStart = iServerService.ServerStart();
            if (!IsServerStart) {
                DimBackgroundWhileLoading = "Hidden";
                RightLogStatus = "Your application can't start. Please retry with Administrator Right.";
                return;
            }

            serverName = iServerService.getCurrentServerInfo().UserName;
            var CurrentUserInfo = iClientService.getCurrentUserInfo();

            //DimBackgroundColor = (visibility, message) =>
            //{
            //    DimBackgroundWhileLoading = visibility;
            //    if(message != null)
            //        RightLogStatus = message;
            //};

            //Register to static hub events
            ChatHub.ClientConnected = (clientId, clientIp) =>
            {
                string serverip = "";
                // Check the client is already exist or not
                var oldParticipantIndex = _clients.FindIndex(d => d.UserIP == CurrentUserInfo.UserIP);
                if (oldParticipantIndex > -1)
                {
                    _clients[oldParticipantIndex].UserId = clientId;
                    serverip = _clients[oldParticipantIndex].UserIP;
                }
                else
                {
                    //Add client to our clients list
                    _clients.Add(new ParticipantInfo()
                    {
                        UserId = clientId,
                        UserIP = clientIp
                    });
                    serverip = CurrentUserInfo.UserIP;
                }

                ClientService.PossibleSignalRHostID?.Invoke(clientId, clientIp);
                LogStatus = $"Client connected:{clientId}";
            };

            ChatHub.ClientDisconnected = (clientId) =>
            {
                //Remove client from the list
                var client = _clients.FirstOrDefault(x => x.UserId == clientId);
                if (client != null)
                    _clients.Remove(client);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
                {
                    List<ParticipantInfo> oldList = ParticipantList.ToList();
                    bool a = oldList.All(_clients.Contains) && oldList.Count == _clients.Count;
                    if (!a)
                    {
                        ParticipantList.Clear();
                        ParticipantList.AddRange(_clients);
                        ClientCount = ParticipantList.Count;
                    }
                    RightLogStatus = $"Total Participant : {ParticipantList.Count}";
                    DimBackgroundWhileLoading = "Hidden";
                }, null);
                LogStatus = $"Client disconnected:{clientId}";
            };

            ChatHub.ClientNameChanged = (clientId, participantInfo) =>
            {
                //Update the client's name if it exists
                var client = _clients.FirstOrDefault(x => x.UserId == clientId);
                if (client != null)
                {
                    client.UserName = participantInfo.UserName;
                    client.UserHost = participantInfo.UserHost;
                    client.UserLogInName = participantInfo.UserLogInName;
                    client.GroupName = participantInfo.GroupName;
                    //LogStatus = $"Client name changed. Id:{clientId}, Name:{newName}";
                    LogStatus = $"{participantInfo.UserName} : Active. ";
                }

            };

            ChatHub.ClientJoinedToGroup = (clientId, groupName) =>
            {
                //Only add the groups name to our groups list
                var group = _groups.FirstOrDefault(x => x == groupName);
                if (group == null)
                    _groups.Add(groupName);
                LogStatus = $"Client joined to group. Id:{clientId}, Group:{groupName}";
            };

            ChatHub.ClientLeftGroup = (clientId, groupName) =>
            {
                LogStatus = $"Client left group. Id:{clientId}, Group:{groupName}";
            };

            ChatHub.MessageReceived = (senderClientId, message) =>
            {
                //One of the clients sent a message, log it
                string clientName = _clients.FirstOrDefault(x => x.UserId == senderClientId)?.UserName;
                LogStatus = $"{clientName}:{message}";
            };

            ServerService.StartScanIP = () =>
            {
                //DimBackgroundWhileLoading = "Hidden";
                //RightLogStatus = "Scaning IP in LAN.";
            };

            ServerService.FinishScanIP = (ServerIP, SelfIPList, AllLANIPList) =>
            {
                //RightLogStatus = $"Finished Scan IP in LAN. Server IP : {ServerIP} / Total IP Count : {AllLANIPList.Count}";
                RightLogStatus = $"Finished Scan IP in LAN. Total IP Count : {AllLANIPList.Count}.";
                ServerService.StartCheckSignalRHost?.Invoke(ServerIP, SelfIPList, AllLANIPList);
            };

            ServerService.FinishedCheckSignalRHost = (ServerIP, AllSignalRHostList) =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
                {
                    RightLogStatus = $"Total Participant : {AllSignalRHostList.Count}";
                    List<ParticipantInfo> oldList = ParticipantList.ToList();
                    bool a = oldList.All(_clients.Contains) && oldList.Count == _clients.Count;
                    if (!a)
                    {
                        ParticipantList.Clear();
                        ParticipantList.AddRange(_clients);
                        ClientCount = ParticipantList.Count;
                    }
                    DimBackgroundWhileLoading = "Hidden";
                }, null);
            };

            ServerService.FinishedCheckSignalRHostForEach = (ServerIP, FinishiedCount, TotalCount, AllSignalRHostList) =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
                {
                    RightLogStatus = $"Participant : {AllSignalRHostList.Count}, Checking Complete : {FinishiedCount}, Total : {TotalCount}";
                    List<ParticipantInfo> oldList = ParticipantList.ToList();
                    bool a = oldList.All(_clients.Contains) && oldList.Count == _clients.Count;
                    if (!a)
                    {
                        ParticipantList.Clear();
                        ParticipantList.AddRange(_clients);
                        ClientCount = ParticipantList.Count;
                    }
                }, null);
            };

            //ClientService.ReceiveMessageFromServer = (name, message) =>
            ClientService.ReceiveMessageFromServer = (name, message) =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
                {
                    var dialogType = MessageBoxButton.Yes.ToString();
                    var title = $"{name}'s Message";
                    //using the dialog service as-is
                    dialogService.ShowDialog(nameof(MessageBoxDialog), new DialogParameters($"dialogType={dialogType}&title={title}&message={message}"), r =>
                    {
                        switch (r.Result)
                        {
                            case ButtonResult.Yes:
                                break;
                            case ButtonResult.No:
                                break;
                            case ButtonResult.Cancel:
                                break;
                            case ButtonResult.None:// Default is None.
                                break;
                            default:
                                return;
                        }
                    });
                }, null);
            };

            ARPCommand = new BackgroundWorker();
            ARPCommand.DoWork += new DoWorkEventHandler(ARPCommand_DoWork);
            ARPCommand.RunWorkerAsync();
        }

        #region インターフェース実装
        /// <summary>
        /// 画面を使い回すか新たに作成するか
        /// true:以前の画面を使い回す
        /// false:新しく作り直す
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <returns></returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// 画面から離れる時の処理
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        /// <summary>
        /// 画面が描画された時
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
        #endregion

        /// <summary>
        /// 初期表示イベント
        /// </summary>
        private DelegateCommand _MainFormControlFormLoaded;
        public DelegateCommand MainFormControlFormLoaded =>
            _MainFormControlFormLoaded ?? (_MainFormControlFormLoaded = new DelegateCommand(MainFormControl_FormLoaded));
        void MainFormControl_FormLoaded()
        {
            try
            {
                if (IsServerStart)
                {
                    //var CurrentServerInfo = iServerService.getCurrentServerInfo();
                    //bool isConnected = await iClientService.connectAsync(iServerService.getUrl(), CurrentServerInfo);
                    //if (isConnected)
                    //{
                        
                    //}
                    //ParticipantList.Clear();
                    //ParticipantList.AddRange(_clients);
                    ClientCount = ParticipantList.Count;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private DelegateCommand _MessageSendCommand;
        public DelegateCommand MessageSendCommand =>
            _MessageSendCommand ?? (_MessageSendCommand = new DelegateCommand(MessageSendEvent));

        private void MessageSendEvent()
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                var dialogType = MessageBoxButton.Yes.ToString();
                var title = "Error Message";
                var message = "Please Write Something to sent.";
                //using the dialog service as-is
                dialogService.ShowDialog(nameof(MessageBoxDialog), new DialogParameters($"dialogType={dialogType}&title={title}&message={message}"), r =>
                {
                    switch (r.Result)
                    {
                        case ButtonResult.Yes:
                            break;
                        case ButtonResult.No:
                            break;
                        case ButtonResult.Cancel:
                            break;
                        case ButtonResult.None:// Default is None.
                            break;
                        default:
                            return;
                    }
                });
            }
            else
            {
                if(ListViewSelectedIndex > -1)
                    iServerService.SendMessage(SendMessageType.Client, Message, serverName, _clients[ListViewSelectedIndex]);
                else
                {
                    string dialogType = MessageBoxButton.Yes.ToString();
                    var title = "Error Message";
                    var message = "Please Select the User Name from List.";
                    //using the dialog service as-is
                    dialogService.ShowDialog(nameof(MessageBoxDialog), new DialogParameters($"dialogType={dialogType}&title={title}&message={message}"), r =>
                    {
                        switch (r.Result)
                        {
                            case ButtonResult.Yes:
                                break;
                            case ButtonResult.No:
                                break;
                            case ButtonResult.Cancel:
                                break;
                            case ButtonResult.None:// Default is None.
                                break;
                            default:
                                return;
                        }
                    });
                }
            }
        }

        private DelegateCommand _RefreshCommand;
        public DelegateCommand RefreshCommand =>
            _RefreshCommand ?? (_RefreshCommand = new DelegateCommand(RefreshEvent));

        private void RefreshEvent()
        {
            if (!IsServerStart)
            {
                RightLogStatus = "Sorry, Please retry with Administrator Right.";
                return;
            }
            //DimBackgroundColor?.Invoke("Visible", "Scaning IP in LAN.");
            DimBackgroundWhileLoading = "Visible";
            RightLogStatus = "Scaning IP in LAN.";
            ParticipantList.Clear();
            ARPCommand.RunWorkerAsync();
            //iServerService.SendARP();
        }

        private void ARPCommand_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                iServerService.SendARP();
            }
            catch (Exception)
            {
            }
        }

        //private DelegateCommand _PreRefreshCommand;
        //public DelegateCommand PreRefreshCommand =>
        //    _PreRefreshCommand ?? (_PreRefreshCommand = new DelegateCommand(PreRefreshEvent));

        //void PreRefreshEvent()
        //{
        //    DimBackgroundWhileLoading = "Visible";
        //    RightLogStatus = "Scaning IP in LAN.";
        //}
    }
}
