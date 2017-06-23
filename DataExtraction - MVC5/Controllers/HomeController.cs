using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataExtraction___MVC5.Infrastructure;
using DataExtraction___MVC5.Models;
using DataExtraction___MVC5.Models.Views;
using HtmlAgilityPack;

namespace DataExtraction___MVC5.Controllers
{
    /// <summary>
    /// klasa kontrolera, posiada wszystkie akcje aplikacji oraz metody dodatkowe/pomocnicze,
    /// które są niezbędne do funkcjonowania aplikacji </summary>
    public class HomeController : Controller
    {
        private readonly IMailService mailService;

        /// <summary>
        /// konstruktor klasy kontrolera </summary>
        /// <param name="mailService">obiekt serwisu maili, który jest przypisywany w konstruktorze
        /// do pola zadeklarowanego wyżej </param>
        public HomeController(IMailService mailService)
        {
            this.mailService = mailService;
        }

        /// <summary>
        /// główna akcja aplikacji, przekierowuje do akcji Query</summary>
        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("Query");
        }

        /// <summary>
        /// akcja wygenerowana automatycznie, zostawiona na przyszłe potrzeby</summary>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        /// <summary>
        /// akcja wygenerowana automatycznie, zostawiona na przyszłe potrzeby</summary>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// akcja zwracająca widok, w którym jest formualrz wyszukiwania </summary>
        public ActionResult Query()
        {
            return View();
        }

        /// <summary>
        /// akcja POST metody wyżej, przesyła formularz </summary>
        /// <param name="viewModel">obiekt klasy view modelu
        /// wypełniony danymi z formularza </param>
        /// <returns>przekierowanie do akcji Matches z parametrem viewModel</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(MatchesViewModel viewModel)
        {
            return RedirectToAction("Matches", viewModel);
        }

        /// <summary>
        /// akcja przyjmuje parametr przekazany z akcji Query
        /// wypełnia viewModel kolejnymi danymi, wysyła maila z logiem wyszukiwania </summary>
        /// <param name="viewModel">obiekt klasy view modelu </param>
        /// <returns>widok z parametrem viewModel</returns>
        public ActionResult Matches(MatchesViewModel viewModel)
        {
            if (viewModel.QueryTeam.Contains(" "))
            {
                viewModel.QueryTeam = viewModel.QueryTeam.Replace(" ", "_");
            }

            viewModel.Matches = GetTeamMatches(viewModel.QueryTeam.ToLower());

            DataExtractionConfirmationEmail email = new DataExtractionConfirmationEmail();

            email.Query = viewModel.QueryTeam;
            email.UserIP = GetVisitorIPAddress();

            mailService.SendMail(email);

            return View(viewModel);
        }

        /// <summary>
        /// prywatna metoda, ustawia adres url strony, z której pobierane są dane.
        /// wywołuje kolejne przywatne metody</summary>
        /// <param name="query">nazwa drużyny, której mecze są wyszukiwane </param>
        /// <returns>liste meczy</returns>
        private IList<Match> GetTeamMatches(string query)
        {
            var url = "http://www.tabelepilkarskie.com/" + "druzyna/" + query + "/";

            var rootContent = GetHTMLDocument(url);

            var matchList = GetMatchList(rootContent);

            return matchList;
        }

        /// <summary>
        /// wywołuje metodę ładującą dokument html </summary>
        /// <param name="url">adres url strony z danymi przekazany z innej metody</param>
        /// <returns>węzeł dokumentu html</returns>
        private HtmlNode GetHTMLDocument(string url)
        {
            var document = LoadDocument(url);

            return document.DocumentNode;
        }

        /// <summary>
        /// tworzy obiekt dokumentu html i przypisuje do niego kod html ze wskazanej strony </summary>
        /// <param name="url">adres url strony z danymi przekazany z innej metody</param>
        /// <returns>w razie powodzenia zwraca dokument html z kodem strony html
        /// w razie nie powodzenia zwraca pusty dokument html</returns>
        public HtmlDocument LoadDocument(string url)
        {
            Uri uri = new Uri(url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var streamResponse = response.GetResponseStream();
                MemoryStream memoryStream = new MemoryStream();
                using (streamResponse)
                {
                    streamResponse.CopyTo(memoryStream);
                }
                memoryStream.Position = 0;
                var document = new HtmlDocument();
                document.Load(memoryStream, Encoding.UTF8);

                return document;
            }
            catch (WebException e)
            {
                var  tusCode = ((HttpWebResponse)e.Response).StatusCode;
            }

            var nullDocument = new HtmlDocument();

            return nullDocument;
        }

        /// <summary>
        /// przeszukuje węzeł dokumentu html, przypisuje odpowiednie dane do zmiennych, tworzy obiekty klas
        /// i wypełnia pola tych obiektów</summary>
        /// <param name="root"> węzeł dokumentu html</param>
        /// <returns>listę meczy </returns>
        private List<Match> GetMatchList(HtmlNode root)
        {
            var list = new List<HtmlNode>();

            try
            {
                list = GetNodesViaAttribute(root, "table", "id", "results")[0].Descendants("tr").ToList();
            }
            catch (Exception e)
            {
            }

            var matchesList = new List<Match>();

            if (list.Count() != 0)
            {
                foreach (var tr in list.Skip(1))
                {
                    if (tr.ChildNodes.Count != 5)
                    {
                        var number = tr.ChildNodes[1].InnerText;
                        int numberInt;
                        Int32.TryParse(number, out numberInt);
                        var teams = tr.ChildNodes[5].ChildNodes[0].InnerText;
                        var host = teams.Substring(0, teams.IndexOf("-") - 1);
                        var guest = teams.Substring(teams.IndexOf("-") + 2);
                        var date = tr.ChildNodes[3].InnerHtml;
                        var competitions = tr.ChildNodes[9].InnerHtml.ToString();
                        var result = tr.ChildNodes[13].InnerHtml;
                        var resultToHalf = tr.ChildNodes[15].ChildNodes[0].InnerHtml;
                        var isWin = "";
                        if (result == " - ")
                        {
                            isWin = "-";
                        }
                        else
                        {
                            isWin = tr.ChildNodes[11].ChildNodes[1].InnerHtml;
                        }

                        var match = new Match()
                        {
                            Host = new Team()
                            {
                                Name = host,
                                CreateDate = DateTime.UtcNow,
                                Id = new Random().Next(100)
                            },

                            Guest = new Team()
                            {
                                Name = guest,
                                CreateDate = DateTime.UtcNow,
                                Id = new Random().Next(100)
                            },

                            MatchDate = DateTime.Parse(date),
                            Result = result,
                            HalfResult = resultToHalf,
                            IsWin = isWin,
                            CreateDate = DateTime.UtcNow,
                            Id = numberInt,
                            Competitions = competitions,
                        };

                        matchesList.Add(match);
                    }
                }
            }
            return matchesList;
        }

        /// <summary>
        /// przeszukuje węzeł dokuemntu html
        /// tworzy listę kolejnych węzłów (podwęzłów)</summary>
        /// <param name="root"> węzeł dokumentu html</param>
        /// <param name="nodename"> nazwa węzła</param>
        /// <param name="attributeName"> nazwa atrybutu</param>
        /// <param name="attributeValue"> wartość atrybutu</param>
        /// <returns>listę węzłów dokumentu html </returns>
        private List<HtmlNode> GetNodesViaAttribute(HtmlNode root, string nodename = "", string attributeName = "", string attributeValue = "")
        {
            var list = root.Descendants(nodename).Where(d => d.Attributes.Contains(attributeName) && d.Attributes[attributeName].Value.Contains(attributeValue)).ToList();

            return list;
        }

        /// <summary>
        /// akcja przygotowana do użycia w hangfire
        /// wysyła maila</summary>
        /// <returns>kod statusu OK</returns>
        [AllowAnonymous]
        public ActionResult SendStatusEmail()
        {
            DataExtractionConfirmationEmail email = new DataExtractionConfirmationEmail();

            email.To = "project@project.pl";
            email.FullAddress = "my address";
            email.Send();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        /// <summary>
        /// metoda pobierająca adres IP aktualnego użytkownika </summary>
        /// <param name="GetLan"> zmienna bool, domyślnie ustawiona na false</param>
        /// <returns>adres IP użytkownika</returns>
        private static string GetVisitorIPAddress(bool GetLan = false)
        {
            string visitorIPAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (String.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = System.Web.HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
            {
                GetLan = true;
                visitorIPAddress = string.Empty;
            }

            if (GetLan && string.IsNullOrEmpty(visitorIPAddress))
            {
                //This is for Local(LAN) Connected ID Address
                string stringHostName = Dns.GetHostName();
                //Get Ip Host Entry
                IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                //Get Ip Address From The Ip Host Entry Address List
                IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                try
                {
                    visitorIPAddress = arrIpAddress[arrIpAddress.Length - 2].ToString();
                }
                catch
                {
                    try
                    {
                        visitorIPAddress = arrIpAddress[0].ToString();
                    }
                    catch
                    {
                        try
                        {
                            arrIpAddress = Dns.GetHostAddresses(stringHostName);
                            visitorIPAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            visitorIPAddress = "127.0.0.1";
                        }
                    }
                }
            }

            return visitorIPAddress;
        }
    }
}