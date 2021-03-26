using System.Collections.Generic;
using System.IO;
using System.Linq;
using HackTheClimate.Tools.Data;

namespace HackTheClimate.Tools.Service
{
    class TextExtraction
    {
        public static void Extract_Abstracts_and_Texts(List<SearchModel> searchModels)
        {
            File.WriteAllLines("new-abstracts.txt", searchModels.Select(x => $"{x.Id};{x.Description.Replace(';', ',').Replace("\n", " ").Replace("\r", " ")}"));
            File.WriteAllLines("new-texts.txt",
                searchModels.Select(x =>
                    $"{x.Id};{x.PolicyTexts.OrderByDescending(y => y.Length).FirstOrDefault()?.Replace(';', ',').Replace("\n", " ").Replace("\r", " ")}"));
        }
    }
}
