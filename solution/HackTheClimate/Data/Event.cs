using System;
using System.Globalization;

namespace HackTheClimate.Data
{
    public class Event
    {
        public DateTime Date;
        public string Description;
        public string ThirdField;

        /// <summary>
        /// </summary>
        /// <param name="input">i.e. 26/08/2011|Law passed||</param>
        /// <returns></returns>
        public static Event TryParse(string input)
        {
            var fields = input.Split("|");

            try
            {
                var date = DateTime.ParseExact(fields[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return new Event
                    {Date = date, Description = fields[1], ThirdField = fields.Length > 2 ? fields[2] : ""};
            }
            catch
            {
                return new Event {Date = new DateTime(2000, 1, 1), Description = "Data Quality Issue in " + input};
            }
        }
    }
}