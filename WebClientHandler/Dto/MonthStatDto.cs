using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClientHandler.Dto
{
    public class MonthStatDto
    {
        public string MonthName { get; set; }
        public int MonthNumber { get; set; }

        public List<LessonDayDto> Lessons { get; set; } 
    }
}
