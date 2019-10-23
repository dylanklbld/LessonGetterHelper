using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebClientHandler.Scripting.Clients
{
    public class HttpClientCustomClient : ITatarEduClient
    {
        private string baseUrl = "https://edu.tatar.ru/logon";
        private CookieContainer _eduCookies;
        private HttpClient _eduClient;

        private WebProxy GetNewProxy()
        {
            System.UriBuilder urlBuilder = new UriBuilder();
            // First create a proxy object
            urlBuilder.Host = "195.190.124.202";
            urlBuilder.Port = 8080;

            WebProxy proxy = new WebProxy()
            {
                Address = urlBuilder.Uri,
                UseDefaultCredentials = true
            };

            return proxy;
        }

        public HttpClientCustomClient()
        {
            _eduCookies = new CookieContainer();
            _eduClient = new HttpClient(new HttpClientHandler()
            {
                CookieContainer = _eduCookies
            });

            _eduClient.DefaultRequestHeaders.Accept.Clear();
            _eduClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        }

        public void Logon(string login, string password)
        {
            //actions due to fiddler
            var getRequestFirst = _eduClient.GetStringAsync(baseUrl).Result;

            var values = new Dictionary<string, string>
            {
               { "main_login", login },
               { "main_password", password }
            };

            var loginRequest = new HttpRequestMessage(HttpMethod.Post, baseUrl);
            var content = new FormUrlEncodedContent(values);

            loginRequest.Content = content;
            loginRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
            loginRequest.Headers.Referrer = new Uri("https://edu.tatar.ru/logon");
            loginRequest.Headers.Host = "edu.tatar.ru";
            loginRequest.Headers.Connection.Add("keep-alive");
            loginRequest.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue() { MaxAge = new TimeSpan(0) };
            loginRequest.Headers.AcceptLanguage.ParseAdd("ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            loginRequest.Headers.AcceptEncoding.ParseAdd("gzip, deflate, br");
            loginRequest.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

            var response =  _eduClient.SendAsync(loginRequest).Result;
            var responseString =  response.Content.ReadAsStringAsync().Result;

            var cookies = _eduCookies.GetCookies(new Uri(baseUrl));


        }

        public string GetJournalHtml()
        {
            var journalGetDefault = _eduClient.GetStringAsync("https://edu.tatar.ru/school/journal").Result;
            return journalGetDefault;
        }

        public string GetJournalHtml(string className, int term = 1, int page = 1)
        {
            var journalGetDefault = _eduClient.GetStringAsync(string.Format("https://edu.tatar.ru/school/journal/index?term={0}&criteria={2}&edu_class_id=&show_moved_pupils=0&page={1}", term, page, className)).Result;
            return journalGetDefault;
        }

        public async Task<string> GetJournalHtmlAsync()
        {
            var journalGetDefault = _eduClient.GetStringAsync("https://edu.tatar.ru/school/journal");
            return await journalGetDefault;
        }

        public async Task<string> GetJournalHtmlAsync(string className, int term = 1, int page = 1)
        {
            var journalGetDefault = _eduClient.GetStringAsync(string.Format("https://edu.tatar.ru/school/journal/index?term={0}&criteria={2}&edu_class_id=&show_moved_pupils=0&page={1}",
                term, page, className));
            return await journalGetDefault;
        }


    }
}
