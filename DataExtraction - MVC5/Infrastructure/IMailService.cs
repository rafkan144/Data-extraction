using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataExtraction___MVC5.Models.Views;

namespace DataExtraction___MVC5.Infrastructure
{
    /// <summary>
    /// interfejs </summary>
    public interface IMailService
    {
        /// <summary>
        /// metoda interfejsu, wszystkie klasy implementujące ten interfejs muszą implementować tą metodą</summary>
        /// <param name="email">obiekt klasy view modelu</param>
        void SendMail(DataExtractionConfirmationEmail email);
    }
}