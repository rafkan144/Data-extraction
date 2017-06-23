using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataExtraction___MVC5.Models
{
    /// <summary>
    /// klasa modelu, reprezentująca mecz, dziedziczy z klasy bazowej</summary>
    public class Match : BaseEntity
    {
        public Team Host { get; set; }
        public Team Guest { get; set; }

        public DateTime MatchDate { get; set; }

        public string Competitions { get; set; }

        public string Result { get; set; }
        public string HalfResult { get; set; }

        public string IsWin { get; set; }
    }
}