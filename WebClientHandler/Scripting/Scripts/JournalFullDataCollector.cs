using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClientHandler.Dto;
using WebClientHandler.Helpers;

namespace WebClientHandler.Scripting.Scripts
{
    using System.Globalization;
    using System.IO;

    public static class JournalFullDataCollector
    {
        private static List<CriteriaDto> AllSubjectsClasses { get; set; }
        private static ITatarEduClient CurrentClient { get; set; }

        public static void InitStuff(string html, ITatarEduClient client)
        {
            AllSubjectsClasses = JournalDataAnalyzerHelper.GetClassSubjectIdList(html);
            CurrentClient = client;
        }

        public static List<ClassSnapshotsListDto> CollectFullClassesData()
        {
            var result = new List<ClassSnapshotsListDto>();
            foreach (var className in AllSubjectsClasses)
            {
                var snapshotData = GetClassSnapshotList(className);
                result.Add(snapshotData);
                ProcessClassSnapshot(snapshotData);
            }
            return result;
        }

        //Collecting snapshot for particular class
        public static ClassSnapshotsListDto GetClassSnapshotList(CriteriaDto classSubjectInfo)
        {
            var nSnapshot = new ClassSnapshotsListDto
            {
                ClassSubjectId = classSubjectInfo,
                FullTermsData = new List<string>()
            };

            var classDefaultHtml = CurrentClient.GetJournalHtml(classSubjectInfo.Value);
            var terms = JournalDataAnalyzerHelper.GetTermsIdsList(classDefaultHtml);
            var maxPages = JournalDataAnalyzerHelper.GetJournalPagesCount(classDefaultHtml);

            foreach (var term in terms)
            {
                for (var i = 1; i <= maxPages; i++)
                {
                    var tableSpsh = CurrentClient.GetJournalHtml(classSubjectInfo.Value, term.Num, i);
                    nSnapshot.FullTermsData.Add(tableSpsh);
                }
            }

            return nSnapshot;

        }

        public static void ProcessClassSnapshot(ClassSnapshotsListDto snapshot)
        {
            var tableClass = "table";
            var htmlDoc = new HtmlDocument();

            foreach (var tableInfo in snapshot.FullTermsData)
            {
                htmlDoc.LoadHtml(tableInfo);
                HtmlNode tableData = htmlDoc.DocumentNode
                    .Descendants("table")
                    .First(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains(tableClass));

                var headerInfo = tableData.SelectNodes("thead").First();
                JournalHeaderParserHelper.HeaderParsers(headerInfo);
            }

        }

        public static async Task<ClassSnapshotsListDto> GetClassSnapshotListAsync(CriteriaDto classSubjectInfo)
        {
            var nSnapshot = new ClassSnapshotsListDto
            {
                ClassSubjectId = classSubjectInfo,
                FullTermsData = new List<string>()
            };

            var classDefaultHtml = CurrentClient.GetJournalHtml(classSubjectInfo.Value);
            var terms = JournalDataAnalyzerHelper.GetTermsIdsList(classDefaultHtml);
            var maxPages = JournalDataAnalyzerHelper.GetJournalPagesCount(classDefaultHtml);

            foreach (var term in terms)
            {
                for (var i = 1; i <= maxPages; i++)
                {
                    var tableSpsh = await CurrentClient.GetJournalHtmlAsync(classSubjectInfo.Value, term.Num, i);
                    nSnapshot.FullTermsData.Add(tableSpsh);
                    TempFileSave(tableSpsh, string.Format(@"{0}", classSubjectInfo.Value.Replace("_","v")), string.Format(@"{0}{1}", term.Num, i));
                }
            }

            return nSnapshot;

        }

        private static void TempFileSave(string tableSpsh, string folderName, string fileName)
        {
            string path = string.Format(@"E:\TempHtml\{0}", folderName);  // folder location

            if (!Directory.Exists(path))  // if it doesn't exist, create
                Directory.CreateDirectory(path);

            
            File.WriteAllText(Path.Combine(path, fileName), tableSpsh);
        }

        public static async Task<List<ClassSnapshotsListDto>> CollectFullClassesDataAsync()
        {
            var result = new List<ClassSnapshotsListDto>();
            foreach (var className in AllSubjectsClasses)
            {
                var snapshotData = await GetClassSnapshotListAsync(className);
                result.Add(snapshotData);
                ProcessClassSnapshot(snapshotData);
            }
            return result;
        }

    }
}
