using Microsoft.Owin.Cors;
using Owin;

namespace Messenger.ChatHelper
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ////CORS need to be enabled for calling SignalR service 
            //app.UseCors(CorsOptions.AllowAll);
            //Find and reigster SignalR hubs
            app.MapSignalR();
        }
    }
}
