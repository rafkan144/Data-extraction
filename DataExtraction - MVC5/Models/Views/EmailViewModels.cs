using Postal;
using DataExtraction___MVC5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExtraction___MVC5.Models.Views
{
    /// <summary>
    /// klasa view modelu, dziedzicząca po klasie Email</summary>
    public class DataExtractionConfirmationEmail : Email
    {
        public string To { get; set; }
        public string FullAddress { get; set; }
        public string UserIP { get; set; }
        public DateTime Date { get; set; }
        public string Query { get; set; }
    }
}