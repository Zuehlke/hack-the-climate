using System;

namespace HackTheClimate.Data
{
    // i.e. 26/08/2011|Law passed||
    public class Event
    {
        public DateTime Date;
        public string Description;
        public string ThirdField;

        public static Event TryParse(string input)
        {
            var fields = input.Split("|");
            if (DateTime.TryParse(fields[0], out var date))
                return new Event
                    {Date = date, Description = fields[1], ThirdField = fields.Length > 2 ? fields[2] : ""};
            return null;
        }
    }
}