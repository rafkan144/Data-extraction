using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataExtraction___MVC5.Models
{
    /// <summary>
    /// klasa modelu, reprezentująca drużynę, dziedziczy z klasy bazowej</summary>
    public class Team : BaseEntity
    {
        public string Name { get; set; }
    }
}