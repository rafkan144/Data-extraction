using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;

[assembly: OwinStartup(typeof(DataExtraction___MVC5.Startup))]

namespace DataExtraction___MVC5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);

            app.UseHangfire(config =>
            {
                //config.UseSqlServerStorage("StoreContext");
                //config.UseServer();
            });
        }
    }
}