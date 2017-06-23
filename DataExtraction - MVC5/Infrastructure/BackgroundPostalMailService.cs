using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using DataExtraction___MVC5.Models.Views;

namespace DataExtraction___MVC5.Infrastructure
{
    /// <summary>
    /// klasa wysyłająca maile w tle, implementuje interfejs IMailService </summary>
    public class BackgroundPostalMailService : IMailService
    {
        /// <summary>
        /// metoda wysyłająca maila w tle </summary>
        /// <param name="email">obiekt klasy view modelu uzupełniany w metodzie</param>
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