using Messenger.ChatHelper;
using Microsoft.AspNet.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Messenger.Services
{
    interface IClientService
    {
        ParticipantInfo getCurrentUserInfo();

        //Task<bool> connectAsync(ParticipantInfo participantInfo);

        ConnectionState getCurrentConnectionState();

        List<string> getMessageList();
    }
}
