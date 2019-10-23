namespace WebClientHandler.Dto
{
    using Enums;

    public class ConcreteLesson
    {
        /// <summary>
        /// Какого рода урок
        /// </summary>
        public LessonType LessonType { get; set; }

        public string LessonNameTemp { get; set; }
    }
}