using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataExtraction___MVC5.Models
{
    /// <summary>
    /// klasa bazowa dla klas modelu</summary>
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreateDate { get; set; }
    }
}