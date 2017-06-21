using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataExtraction___MVC5.ViewModels;

namespace DataExtraction___MVC5.Infrastructure
{
    public class PostalMailService : IMailService
    {
        public void SendMail()
        {
            DataExtractionConfirmationEmail email = new DataExtractionConfirmationEmail();

            email.To = "project@project.pl";
            email.FullAddress = "my address";
            email.Send();
            email.Send();
        }
    }
}