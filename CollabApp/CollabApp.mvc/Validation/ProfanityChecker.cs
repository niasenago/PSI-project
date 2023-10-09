
using CollabApp.mvc.Collections;

namespace CollabApp.mvc.Validation
{
    // Can easily be bypassed.
    public static class ProfanityChecker {

        private static readonly List<Trie> profanitiesList = new()
        {
            GetProfanities("Content/ProfanityLists/en.txt"),
        };

        private static bool IsProfanityDetected(Trie profanities, string[] words)
        {
            foreach(string word in words)
            {
                if(profanities.Find(word))
                    return true;
            }

            return false;
        }

        private static Trie GetProfanities(string filePath)
        {
            Trie profanities = new();
            using (StreamReader reader = new(filePath))
            {
                string? line = null;
                while((line = reader.ReadLine()) != null)
                    profanities.Insert(line);
            }

            return profanities;
        }

        public static bool HasProfanity(string line)
        {
            string[] words = line.Split().Select(word => word.ToLower()).ToArray();
            foreach(var profanityTrie in profanitiesList)
            {
                if(IsProfanityDetected(profanityTrie, words))
                    return true;
            }

            return false;
        }
    }
}