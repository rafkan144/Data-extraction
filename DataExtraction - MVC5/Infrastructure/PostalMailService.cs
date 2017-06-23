using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataExtraction___MVC5.Models.Views;

namespace DataExtraction___MVC5.Infrastructure
{
    public class PostalMailService : IMailService
    {
        public void SendMail(DataExtractionConfirmationEmail email)
        {
            email.To = "project@project.pl";
            email.FullAddress = "my address";
            email.Send();
        }
    }
}