using Microsoft.AspNet.SignalR.Client;
using Prism.Mvvm;
using System;

namespace Messenger.ChatHelper
{
    public class ServerInfo : BindableBase
    {
        private string port = "1234";

        public Action<string, string> ReceiveMessageForEachServer { get; set; }

        public HubConnection signalRConnection { get; set; } //Connection to a SignalR server

        public IHubProxy hubProxy { get; set; } //Proxy object for a hub hosted on the SignalR server

        public ConnectionState currentConnectionState = ConnectionState.Disconnected;

        public string ConnectionId { get; set; }

        public string ServerIP { get; set; }

        public string Url { get; set; }

        public HubConnection SignalRConnection { get; set; }

        private void HubConnection_StateChanged(StateChange obj)
        {
            currentConnectionState = obj.NewState;
        }

        public ServerInfo(string ip, ParticipantInfo currentUserInfo)
        {
            ServerIP = ip;
            Url = String.Format("http://{0}:{1}/signalr", ip, port);
            signalRConnection = new HubConnection(Url);
            signalRConnection.StateChanged += HubConnection_StateChanged;

            //HubConnection with queryString
            //IDictionary<string, string> queryString = new Dictionary<string, string>();
            //queryString["ParticipantName"] = "TestName";
            //queryString["EntranceName"] = "ServerName";
            //queryString["EntrancePassword"] = "pc004";

            ////Create a connection for the SignalR server
            //_signalRConnection = new HubConnection(txtUrl.Text, queryString);

            //Get a proxy object that will be used to interact with the specific hub on the server
            //Ther may be many hubs hosted on the server, so provide the type name for the hub
            hubProxy = signalRConnection.CreateHubProxy("ChatHub");

            //Reigster to the "Connected" callback method of the hub
            //This method is invoked by the hub
            hubProxy.On<string>("Connected", (result) =>
            {

            });

            //Reigster to the "AddMessage" callback method of the hub
            //This method is invoked by the hub
            hubProxy.On<string, string>("AddMessage", (name, message) =>
            {
                //writeToLog($"{name}:{message}");
                ReceiveMessageForEachServer?.Invoke(name, message);
            });
        }
    }
}
