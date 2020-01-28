using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Messenger.ChatHelper
{
    public class ChatHub : Hub
    {
        static ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();

        public static Action<string,string> ClientConnected { get; set; }
        public static Action<string> ClientDisconnected { get; set; }
        public static Action<string, ParticipantInfo> ClientNameChanged { get; set; }
        public static Action<string, string> ClientJoinedToGroup { get; set; }
        public static Action<string, string> ClientLeftGroup { get; set; }
        public static Action<string, string> MessageReceived { get; set; }

        //private readonly HostInfo hostinfo;

        public static void ClearState()
        {
            _users.Clear();
        }

        public ChatHub()
        {
            //var HostName = Dns.GetHostName();
            //var IPAddress = Dns.GetHostEntry(HostName).AddressList
            //                .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            //                .ToString();
            //var UserName = Environment.UserName;
            //var DomainName = Environment.UserDomainName;
            //var ComputerName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            //hostinfo = new HostInfo
            //{
            //    HostIP = IPAddress,
            //    HostName = HostName,
            //    UserLogInName = ComputerName,
            //    UserName = UserName,
            //    GroupName = DomainName,
            //};
        }

        //Called when a client is connected
        public override Task OnConnected()
        {
            var _environment = Context.Request.Environment;
            string _clientip = _environment["server.RemoteIpAddress"].ToString();
            //var _host = Context.Headers["Host"];
            ////var _useragent = Context.Headers["User-Agent"];
            //var _hostarr = _host.Split(':');
            //string _clientip = _hostarr[0];
            ////var _clientip = "10.0.27.148";
            ////string _clientport = _hostarr[1];

            //var query = Context.Request.QueryString;
            //string pname = query["ParticipantName"] ?? "";
            //string ename = query["EntranceName"] ?? "";
            //string epassword = query["EntrancePassword"] ?? "";

            _users.TryAdd(Context.ConnectionId, Context.ConnectionId);

            ClientConnected?.Invoke(Context.ConnectionId, _clientip);
            Clients.Client(Context.ConnectionId).Connected(_clientip);
            return base.OnConnected();
        }

        //Called when a client is disconnected
        public override Task OnDisconnected(bool stopCalled)
        {
            string userName;
            _users.TryRemove(Context.ConnectionId, out userName);

            ClientDisconnected?.Invoke(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        #region Client Methods

        public void SetUserName(ParticipantInfo participantInfo)
        {
            _users[Context.ConnectionId] = participantInfo.UserName;

            ClientNameChanged?.Invoke(Context.ConnectionId, participantInfo);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.Add(Context.ConnectionId, groupName);

            ClientJoinedToGroup?.Invoke(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.Remove(Context.ConnectionId, groupName);

            ClientLeftGroup?.Invoke(Context.ConnectionId, groupName);
        }

        public void Send(string msg)
        {
            Clients.All.addMessage(_users[Context.ConnectionId], msg);

            MessageReceived?.Invoke(Context.ConnectionId, msg);
        }

        #endregion        
    }
}