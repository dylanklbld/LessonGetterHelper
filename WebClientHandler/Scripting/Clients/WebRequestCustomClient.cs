using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebClientHandler.Scripting.Clients
{
    public class WebRequestCustomClient : ITatarEduClient
    {
        private string baseUrl = "https://edu.tatar.ru/logon";


        public WebRequestCustomClient()
        {

        }

        public void Logon(string login, string password)
        {
            string formUrl = string.Format("{0}?{1}", baseUrl, "Action=/logon"); // NOTE: This is the URL the form POSTs to, not the URL of the form (you can find this in the "action" attribute of the HTML's form tag
            string formParams = string.Format("main_login={0}&main_password={1}", login, password);
            string cookieHeader;

            WebRequest req = WebRequest.Create(formUrl);

            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(formParams);
            req.ContentLength = bytes.Length;

            using (Stream os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }

            WebResponse resp = req.GetResponse();
            cookieHeader = resp.Headers["Set-Cookie"];
        }

        public string GetJournalHtml()
        {
            throw new NotImplementedException();
        }

        public string GetJournalHtml(string className, int term = 1, int page = 1)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetJournalHtmlAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetJournalHtmlAsync(string className, int term = 1, int page = 1)
        {
            throw new NotImplementedException();
        }
    }
}
