
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CollabApp.Tests")]
namespace CollabApp.mvc.Validation
{
    // Can easily be bypassed.
    public static class ProfanityHandler 
    {

        private static readonly List<HashSet<string>> profanitiesList = new()
        {
            GetProfanities(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Content", "ProfanityLists", "en.txt")),
        };

        private static bool IsProfanityDetected(HashSet<string> profanities, string[] words)
        {
            foreach(string word in words)
            {
                if(profanities.Contains(word))
                    return true;
            }

            return false;
        }

        private static HashSet<string> GetProfanities(string filePath)
        {
            HashSet<string> profanities = new();
            using (StreamReader reader = new(filePath))
            {
                string? line = null;
                while((line = reader.ReadLine()) != null)
                    profanities.Add(line);
            }

            return profanities;
        }

        public static bool HasProfanity(string line)
        {
            string[] words = line.Split().Select(word => word.ToLower()).ToArray();
            foreach(var profanitySet in profanitiesList)
            {
                if(IsProfanityDetected(profanitySet, words))
                    return true;
            }

            return false;
        }

        public static string CensorProfanities(string line)
        {
            foreach(var profanitySet in profanitiesList)
            {
                line = Regex.Replace(line, @"\b(" + string.Join("|", profanitySet) + @")\b", "****", RegexOptions.IgnoreCase);
            }

            return line;
        }
    }
}