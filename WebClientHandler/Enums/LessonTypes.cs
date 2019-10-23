namespace WebClientHandler.Enums
{
    using System.ComponentModel;
    using Extensions.Attribute;

    public enum LessonType
    {
        [Description("Домашняя работа")]
        [Abbreviation("ДР")]
        Homework = 10,

        [Description("Ответ на уроке")]
        [Abbreviation("ОУ")]
        BasicLesson = 15,

        [Description("Контрольная работа")]
        [Abbreviation("КР")]
        GeneralTest = 20,

        [Description("Срез знаний")]
        [Abbreviation("СЗ")]
        LessonsLearnedTest = 25,

        [Description("Лабораторная работа")]
        [Abbreviation("ЛР")]
        Labwork = 30,

        [Description("Самостоятельная работа")]
        [Abbreviation("СР")]
        Test = 40,

        [Description("Проект")]
        [Abbreviation("П")]
        ProjectLesson = 50,

        [Description("Реферат")]
        [Abbreviation("Р")]
        Referat = 60
    }
}