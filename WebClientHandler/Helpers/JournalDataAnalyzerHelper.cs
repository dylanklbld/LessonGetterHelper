using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClientHandler.Dto;
using WebClientHandler.Scripting;

namespace WebClientHandler.Helpers
{
    public static class JournalDataAnalyzerHelper
    {
        private static string ClassSubjectId = "criteria";
        private static string TermsId = "term";
        private static string PagesClass = "pages";

        //public static void Test(string html)
        //{
        //    var a = GetClassSubjectIdList(html);
        //    var b = GetTermsIdsList(html);
        //    var pgs = GetJournalPagesCount(html);
        //}

        /// <summary>
        /// Это для списка класс/предмет
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<CriteriaDto> GetClassSubjectIdList(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            HtmlNode select = htmlDoc.GetElementbyId(ClassSubjectId);

            var options = select.SelectNodes("option");
            var result = new List<CriteriaDto>();

            foreach (var op in options)
            {
                result.Add(new CriteriaDto
                {
                    ClassName = op.NextSibling.InnerText,
                    Value = op.Attributes["value"].Value
                });
            }

            return result;
        }

        public static List<TermDto> GetTermsIdsList(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            HtmlNode select = htmlDoc.GetElementbyId(TermsId);

            var options = select.SelectNodes("option");
            var result = new List<TermDto>();

            foreach (var op in options)
            {
                result.Add(new TermDto
                {
                    Name = op.NextSibling.InnerText,
                    Num = Convert.ToInt32(op.Attributes["value"].Value)
                });
            }

            return result;
        }

        public static int GetJournalPagesCount(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            //выбрали элемент с количеством страниц
            var select = htmlDoc.DocumentNode
                .Descendants("p")
                .Where(d =>
                   d.Attributes.Contains("class")
                   &&
                   d.Attributes["class"].Value.Contains(PagesClass)
                );

            int result = 0;

            if (select.FirstOrDefault() != null)
            {
                result = Convert.ToInt32(select.FirstOrDefault().LastChild.InnerText);
            }

            return result;
        }
    }
}
