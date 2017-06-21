using Postal;
using DataExtraction___MVC5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExtraction___MVC5.ViewModels
{
    public class DataExtractionConfirmationEmail : Email
    {
        public string To { get; set; }
        public string FullAddress { get; set; }
    }
}