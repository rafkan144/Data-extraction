using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataExtraction___MVC5.Infrastructure
{
    public class HangFirePostalMailService : IMailService
    {
        public void SendMail()
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string url = urlHelper.Action("SendStatusEmail", "Home", HttpContext.Current.Request.Url.Scheme);

            // Hangfire - nice one (if ASP.NET app will be still running)
            BackgroundJob.Enqueue(() => Helpers.CallUrl(url));
        }
    }
}