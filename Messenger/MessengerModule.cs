using Messenger.Services;
using Messenger.ViewModels;
using Messenger.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Messenger
{
    public class MessengerModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public MessengerModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate("MainRegion", nameof(MainForm));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IServerService, ServerService>();
            containerRegistry.RegisterSingleton<IClientService, ClientService>();
            containerRegistry.Register<object, MainForm>(nameof(MainForm));
            containerRegistry.RegisterDialog<MessageBoxDialog, MessageBoxDialogViewModel>();
        }
    }
}
