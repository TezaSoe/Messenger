using Messenger.ChatHelper;

namespace Messenger.Services
{
    interface IServerService
    {
        bool ServerStart();

        bool ServerStop();

        string getServerIP();

        string getUrl();

        ParticipantInfo getCurrentServerInfo();

        void SendMessage(SendMessageType messageType, string message, string servername, ParticipantInfo participant);

        void SendARP();
    }
}
