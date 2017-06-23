using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using DataExtraction___MVC5.Models.Views;

namespace DataExtraction___MVC5.Infrastructure
{
    public class BackgroundPostalMailService : IMailService
    {
        public void SendMail(DataExtractionConfirmationEmail email)
        {
            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                email.To = "project@project.pl";
                email.FullAddress = "my address";
                email.Date = DateTime.Now;
                email.Send();
            });
        }
    }
}