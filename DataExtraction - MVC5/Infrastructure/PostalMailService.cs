using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataExtraction___MVC5.Models.Views;

namespace DataExtraction___MVC5.Infrastructure
{
    /// <summary>
    /// metoda wysyłająca maila tradycyjny sposób (nie w tle), implementuje interfejs IMailService </summary>
    public class PostalMailService : IMailService
    {
        /// <summary>
        /// metoda wysyłająca maila w tradycyjny sposób (nie w tle)</summary>
        /// <param name="email">obiekt klasy view modelu uzupełniany w metodzie</param>
        public void SendMail(DataExtractionConfirmationEmail email)
        {
            email.To = "project@project.pl";
            email.FullAddress = "my address";
            email.Send();
        }
    }
}