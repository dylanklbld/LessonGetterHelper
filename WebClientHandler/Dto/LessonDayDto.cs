namespace WebClientHandler.Dto
{
    using System;
    using System.Collections.Generic;
    using Enums;

    public class LessonDayDto
    {
        private readonly ConcreteLesson _concreteLesson = new ConcreteLesson();

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        public int DayNum { get; set; }

        public List<ConcreteLesson> DayLessons { get; set; }
    }
}