using System.IO;
using System.Linq;

namespace HackTheClimate.Tools.Service
{
    class TextFileCleaning
    {
        public void Clean(string rawTextFileDirectory, string destinationDirectory)
        {
            var files = new DirectoryInfo(rawTextFileDirectory).GetFiles();
            foreach (var fileInfo in files)
            {
                var lines = File.ReadAllLines(fileInfo.FullName);

                var cleanedLines = lines.Where(x => !string.IsNullOrWhiteSpace(x))
                    .Where(x => x.Length > 5);

                File.WriteAllLines(Path.Combine(destinationDirectory, fileInfo.Name), cleanedLines);
            }
        }
    }
}
