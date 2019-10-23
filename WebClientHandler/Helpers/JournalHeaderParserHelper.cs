using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClientHandler.Helpers
{
    using System.Globalization;
    using Dto;
    using HtmlAgilityPack;

    public static class JournalHeaderParserHelper
    {
        private static string[] MonthNames { get; set; }
        private static int DateNumberRowNum = 2;
        private static int LessonsTypeRowNum = 3;

        private static void Init()
        {
            var rusCulture = CultureInfo.GetCultureInfo("ru-RU");
            MonthNames = rusCulture.DateTimeFormat.MonthNames;
        }

        private static string[] GetPageMonthList(HtmlNode headerFullNode)
        {
            var monthList =
                headerFullNode.SelectNodes("//tr/td")
                    .Where(x => x.Attributes["colspan"] != null && MonthNames.Contains(x.InnerHtml))
                    .ToList();

            return monthList.Select(x => x.InnerText).ToArray();
        }

        private static List<int> GetDatesOnPage(HtmlNode headerFullNode)
        {
            var htmlDateNodesRow = headerFullNode.SelectSingleNode(string.Format("tr[{0}]", DateNumberRowNum));
            var dateRowData = htmlDateNodesRow.SelectNodes(".//td");
            var result = new List<int>();

            foreach (var data in dateRowData)
            {
                var attrs = data.SelectSingleNode("a").Attributes;
                if (attrs["colspan"] != null && Convert.ToInt32(attrs["colspan"].Value) > 1)
                {
                    int colspanVal = Convert.ToInt32(attrs["colspan"].Value);

                    // Делаем тупо копии элементов, чтобы легче было потом считать и сопоставлять
                    for (int i = 0; i < colspanVal; i++)
                    {
                        result.Add(Convert.ToInt32(data.InnerText));
                    }
                }
                else
                {
                    result.Add(Convert.ToInt32(data.InnerText));
                }
            }

            //Just FI
            //var datesWithFewLessons = dateRowData.Where(x => x.Attributes["colspan"] != null && Convert.ToInt32(x.Attributes["colspan"].Value) > 1).ToList();
            //if (datesWithFewLessons.Count > 0)
            //{
            //    Console.WriteLine(string.Join(";",datesWithFewLessons.Select(x => Convert.ToInt32(x.InnerText)).ToList()));
            //}

            return result;
        }

        private static List<string> GetLessonsNamesOnPage(HtmlNode headerFullNode)
        {
            var htmlDateNodesRow = headerFullNode.SelectSingleNode(string.Format("tr[{0}]", LessonsTypeRowNum));
            var dateRowDate = htmlDateNodesRow.SelectNodes(".//td");

            return dateRowDate.Select(x => x.InnerText).ToList();
        }

        private static Dictionary<string, List<int>> MonthToDaysList(List<int> dates, string[] monthNames)
        {
            Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();
            var splitOnNewMonth = dates.Aggregate(
                                    new List<List<int>> { new List<int>() },
                                   (list, value) =>
                                   {
                                       var lastList = list.Last();
                                       if (lastList.Count > 0 && value < lastList[lastList.Count - 1])
                                       {
                                           list.Add(new List<int>());
                                           list.Last().Add(value);
                                       }
                                       else
                                       {
                                           list.Last().Add(value);
                                       }
                                       return list;
                                   });

            foreach (var m in monthNames.Select((month, index) => new { month, index }))
            {
                result.Add(monthNames[m.index], splitOnNewMonth[m.index]);
            }
            return result;
        }

        private static List<MonthStatDto> GetMonthStatByData(string[] monthNames, List<int> datesFull, List<string> lessons)
        {
            List<MonthStatDto> resultMonthStatList = new List<MonthStatDto>();
            //chunk days
            var splitOnNewMonth = datesFull.Aggregate(
                                    new List<List<int>> { new List<int>() },
                                   (list, value) =>
                                   {
                                       var lastList = list.Last();
                                       if (lastList.Count > 0 && value < lastList[lastList.Count - 1])
                                       {
                                           list.Add(new List<int>());
                                           list.Last().Add(value);
                                       }
                                       else
                                       {
                                           list.Last().Add(value);
                                       }
                                       return list;
                                   });

            var lessonsSplitted = new List<List<string>>();

            //chunk lessons
            foreach (var monthDates in splitOnNewMonth)
            {
                for (int i = 0; i < monthDates.Count; i += monthDates.Count)
                {
                    lessonsSplitted.Add(lessons.GetRange(i, Math.Min(monthDates.Count, lessons.Count - i)));
                }
            }

            foreach (var monthInfo in splitOnNewMonth.Select((value, index) => new { value, index }))
            {
                string currentMonthName = monthNames[monthInfo.index];
                var lessonsPerDayGrouping = monthInfo.value.GroupBy(i => i).ToList();
                var schoolDaysList = new List<LessonDayDto>();

                foreach (var dateInfo in monthInfo.value.Select((day, index) => new { day, index }))
                {
                    LessonDayDto schoolDay;

                    if (schoolDaysList.Any(x => x.DayNum == monthInfo.value[dateInfo.index]))
                    {
                        continue;
                    }

                    if (lessonsPerDayGrouping[dateInfo.index].Count() > 1)
                    {
                        int i = 0;
                        int groupCount = lessonsPerDayGrouping[dateInfo.index].Count();
                        schoolDay = new LessonDayDto
                        {
                            DayNum = dateInfo.day,
                            DayLessons = new List<ConcreteLesson>()
                        };
                        while (i < groupCount)
                        {
                            schoolDay.DayLessons.Add(
                                  new ConcreteLesson
                                  {
                                      LessonNameTemp = lessonsSplitted[monthInfo.index][dateInfo.index + i]
                                  });
                            i++;
                        }

                    }
                    else
                    {
                        schoolDay = new LessonDayDto
                        {
                            DayNum = dateInfo.day,
                            DayLessons = new List<ConcreteLesson>()
                            {
                                new ConcreteLesson
                                {
                                    LessonNameTemp = lessonsSplitted[monthInfo.index][dateInfo.index]
                                }
                            }
                        };
                    }

                    schoolDaysList.Add(schoolDay);
                }

                resultMonthStatList.Add(new MonthStatDto
                {
                    MonthName = currentMonthName,
                    Lessons = schoolDaysList
                });
            }

            return resultMonthStatList;
        }

        public static void HeaderParsers(HtmlNode headerFullNode)
        {
            Init();
            try
            {
                string[] monthList = GetPageMonthList(headerFullNode);
                List<int> datesList = GetDatesOnPage(headerFullNode);
                List<string> lessonsTypesList = GetLessonsNamesOnPage(headerFullNode);

                // Order mus be the same, so no need to worry about it I guess
                //var monthDays = MonthToDaysList(datesList, monthList);
                var fullMonthsInfo = GetMonthStatByData(monthList, datesList, lessonsTypesList);
                List<MonthStatDto> monthStatCollection = new List<MonthStatDto>();

                //Dictionary<int, string> dateLessonDictionary = new Dictionary<int, string>();

                //foreach (var date in datesList.Select((dateValue, index) => new {dateValue, index}))
                //{
                //    dateLessonDictionary.Add(datesList[date.index], lessonsTypesList[date.index]);
                //}
            }
            catch (Exception e)
            {
                throw new NullReferenceException("Something just wrong");
            }
        }
    }
}
