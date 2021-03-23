using System;

namespace HackTheClimate.Data
{
    public class Document
    {
        public string Description;
        public string Language;
        public Uri Uri;

        /// <summary>
        /// </summary>
        /// <param name="input">i.e. "Full text (PDF)|https://climate-laws.org/rails/active_storage/blobs/lsPdfPrint.do.pdf|ko"</param>
        /// <returns></returns>
        public static Document TryParse(string input)
        {
            var fields = input.Split("|");
            if (fields.Length >= 2 && Uri.TryCreate(fields[1], UriKind.Absolute, out var uri))
                return new Document
                    {Description = fields[0], Uri = uri, Language = fields[2]};
            return null;
        }
    }
}