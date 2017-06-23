using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace DataExtraction___MVC5.Infrastructure
{
    /// <summary>
    /// klasa helperów aplikacji</summary>
    public class Helpers
    {
        /// <summary>
        /// statyczna metoda wywołująca asynchronicznie url</summary>
        /// <param name="serviceUrl">url, który ma zostać wywołany</param>
        public static void CallUrl(string serviceUrl)
        {
            // Here we can't use Url.Action - called out of process
            var req = HttpWebRequest.Create(serviceUrl);
            req.GetResponseAsync();
        }
    }
}