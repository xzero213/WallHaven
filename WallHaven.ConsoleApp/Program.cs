﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WallHaven.ConsoleApp
{
    class Program
    {
        private const string regexUrl = "(?:(?:https?:\\/\\/)(?:alpha\\.wallhaven\\.cc\\/wallpaper\\/))([0-9]+(?=\"))";
        private const string regexImage = "(?:src=\"\\/\\/)(wallpapers\\.wallhaven\\.cc\\/wallpapers\\/full\\/wallhaven-{0}+\\.[a-zA-Z]+)";
        private const string requestUrl = "https://alpha.wallhaven.cc/search?q=&search_image=&categories=001&purity=001&ratios=16x9&sorting=date_added&order=desc&page={0}";
        private const string requestUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
        private const string requestContentType = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        private const string requestCookie_0 = "d120f21787983998fb1524ae8d97398841500506629";
        private const string requestCookie_1 = "eyJpdiI6Ik9nZjNGRUp6U1RDM25YaG1abFRGc0NERWxBTVM2RW9OT2VZU1hOd1Fwcnc9IiwidmFsdWUiOiJmTENnRkZXbkpqRlMxd01HbGk4TVBGWVdPbE1KQ3VhaXhtQTBPZ1FPQ3JrTjBYXC9BckgyUzJ2VWRGV2lGemxRUGZkbCtmOVRhNHl6M1FuYmdzRkpPM1h3bEg5QXpTT1hwMlpQRXJadWJ1c2tuaUxmcVdIK3dkTHNKcmNwZXhBV28iLCJtYWMiOiJhNzMwNWE3YTVmMzg4ZWY5ZDVmYWFlZWJhZTAwMGE5NTQ1MGZjNjEwYWNhNmVmZGRhNDViZjY0NjY5NDlkZWE0In0%3D";
        private const string requestCookie_2 = "1";
        private const string requestCookie_3 = "GA1.2.544989659.1500506632";
        private const string requestCookie_4 = "GA1.2.1281294653.1500506632";
        private const string outputFolder = "C:\\Users\\DHR\\Desktop\\Images\\";

        static void Main(string[] args)
        {
            var isCancelled = false;
            var imageList = new Dictionary<string, string>();
            ConsoleCancelEventHandler cancellationHandler = (s, e) => e.Cancel = isCancelled = true;
            Console.CancelKeyPress += cancellationHandler;

            try
            {
                var page = 1;
                var uri = new Uri("https://alpha.wallhaven.cc");
                var cookieContainer = new CookieContainer();
                var cookieCollection = new CookieCollection()
                {
                    new Cookie("__cfduid", requestCookie_0),
                    new Cookie("remember_82e5d2c56bdd0811318f0cf078b78bfc", requestCookie_1),
                    new Cookie("_gat", requestCookie_2),
                    new Cookie("_ga", requestCookie_3),
                    new Cookie("_gid", requestCookie_4),
                    new Cookie("wallhaven_session", string.Empty)
                };

                cookieContainer.Add(uri, cookieCollection);

                while (!isCancelled)
                {
                    var request = WebRequest.CreateDefault(new Uri(string.Format(requestUrl, page)));
                    request.Credentials = CredentialCache.DefaultCredentials;
                    request.ContentType = requestContentType;

                    (request as HttpWebRequest).UserAgent = requestUserAgent;
                    (request as HttpWebRequest).CookieContainer = cookieContainer;

                    using (var response = request.GetResponse())
                    {
                        cookieCollection["wallhaven_session"].Value = (response as HttpWebResponse).Cookies["wallhaven_session"].Value;

                        using (var responseStream = response.GetResponseStream())
                        using (var responseReader = new StreamReader(responseStream))
                            foreach (Match match in Regex.Matches(responseReader.ReadToEnd(), regexUrl))
                                imageList.Add(match.Groups[1].Value, match.Value);
                    }

                    page++;
                }

                using (var sw = new StreamWriter("C:\\Users\\dhr\\desktop\\imageList.txt", true))
                    foreach (var item in imageList)
                        sw.WriteLine(string.Format("{0},{1}", item.Key, item.Value));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\n**** UNHANDLED EXCEPTION ***");
                Console.WriteLine("\tMessage: {0}", ex.Message);
                Console.WriteLine("\tType: {0}", ex.GetType());
                Console.WriteLine("\tStack Trace: {0}", ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            finally
            {
                Console.CancelKeyPress -= cancellationHandler;

                Console.WriteLine();
                Console.WriteLine("Press any key to exit");
                Console.Read();
            }
        }
    }
}
