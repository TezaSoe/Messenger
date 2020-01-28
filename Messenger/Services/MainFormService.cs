namespace Messenger.Services
{
    class MainFormService : IMainFormService
    {
        //// For Singleton Instance, not quite as lazy, but thread-safe without using locks
        //private static readonly MainFormService instance = instance ?? new MainFormService();
        //public static MainFormService Instance
        //{
        //    get
        //    {
        //        return instance;
        //    }
        //}

        public MainFormService()
        {
        }
    }
}
