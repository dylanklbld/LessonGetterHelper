using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClientHandler.Scripting
{
    public interface ITatarEduClient
    {
        void Logon(string login, string password);

        string GetJournalHtml();

        string GetJournalHtml(string className, int term = 1, int page = 1);

        Task<string> GetJournalHtmlAsync();

        Task<string> GetJournalHtmlAsync(string className, int term = 1, int page = 1);
    }
}
