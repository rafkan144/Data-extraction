﻿using System;
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
    public class HomeController : Controller
    {
        private readonly IMailService mailService;

        public HomeController(IMailService mailService)
        {
            this.mailService = mailService;
        }

        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("Query");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Query()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(MatchesViewModel viewModel)
        {
            return RedirectToAction("Matches", viewModel);
        }

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

        private IList<Match> GetTeamMatches(string query)
        {
            var url = "http://www.tabelepilkarskie.com/" + "druzyna/" + query + "/";

            var rootContent = GetHTMLDocument(url);

            var matchList = GetMatchList(rootContent);

            return matchList;
        }

        private HtmlNode GetHTMLDocument(string url)
        {
            var document = LoadDocument(url);

            return document.DocumentNode;
        }

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

        private List<HtmlNode> GetNodesViaAttribute(HtmlNode root, string nodename = "", string attributeName = "", string attributeValue = "")
        {
            var list = root.Descendants(nodename).Where(d => d.Attributes.Contains(attributeName) && d.Attributes[attributeName].Value.Contains(attributeValue)).ToList();

            return list;
        }

        [AllowAnonymous]
        public ActionResult SendStatusEmail()
        {
            DataExtractionConfirmationEmail email = new DataExtractionConfirmationEmail();

            email.To = "project@project.pl";
            email.FullAddress = "my address";
            email.Send();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

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