using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using DataExtraction___MVC5.ViewModels;

namespace DataExtraction___MVC5.Infrastructure
{
    public class BackgroundPostalMailService : IMailService
    {
        public void SendMail()
        {
            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                DataExtractionConfirmationEmail email = new DataExtractionConfirmationEmail();

                email.To = "project@project.pl";
                email.FullAddress = "my address";
                email.Send();
            });
        }
    }
}