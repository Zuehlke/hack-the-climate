using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Search.Documents.Indexes.Models;

namespace HackTheClimate.Tools.Service
{
    public static class Extensions
    {
        public static string[] SplitAndTrim(this string s, params string[] delimiter)
        {
            return s == null ? new string[0] : s.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        }

        public static SearchField Select(this IList<SearchField> fields, string name)
        {
            return fields.SingleOrDefault(x => x.Name == name);
        }
    }
}
