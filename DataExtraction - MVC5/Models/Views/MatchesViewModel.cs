using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataExtraction___MVC5.Models.Views
{
    /// <summary>
    /// klasa view modelu, wypełniana danymi w kontrolerze</summary>
    public class MatchesViewModel
    {
        public IList<Match> Matches { get; set; }
        public string QueryTeam { get; set; }
    }
}