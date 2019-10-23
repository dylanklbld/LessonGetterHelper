namespace Extensions.Attribute
{
    using System;

    [AttributeUsage(AttributeTargets.All)]
    public class AbbreviationAttribute : Attribute
    {
        public string Abbreviation { get; private set; }
        public AbbreviationAttribute(string abbreviation)
        {
            Abbreviation = abbreviation;
        }
    }
}
