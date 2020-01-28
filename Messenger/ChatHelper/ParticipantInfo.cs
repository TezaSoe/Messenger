using Prism.Mvvm;

namespace Messenger.ChatHelper
{
    public class ParticipantInfo : BindableBase
    {
        private string _UserId;
        public string UserId
        {
            get { return _UserId; }
            set { SetProperty(ref _UserId, value); }
        }

        private string _UserIP;
        public string UserIP
        {
            get { return _UserIP; }
            set { SetProperty(ref _UserIP, value); }
        }

        private string _UserHost;
        public string UserHost
        {
            get { return _UserHost; }
            set { SetProperty(ref _UserHost, value); }
        }

        private string _UserLogInName;
        public string UserLogInName
        {
            get { return _UserLogInName; }
            set { SetProperty(ref _UserLogInName, value); }
        }

        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set { SetProperty(ref _UserName, value); }
        }

        private string _GroupName;
        public string GroupName
        {
            get { return _GroupName; }
            set { SetProperty(ref _GroupName, value); }
        }
    }
    //public class HostInfo
    //{
    //    public string HostIP { get; set; }

    //    public string Port { get; set; } = "1234";

    //    public string HostName { get; set; }

    //    public string UserLogInName { get; set; }

    //    public string UserName { get; set; }

    //    public string GroupName { get; set; }
    //}
}
