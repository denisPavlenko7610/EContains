using System.Text.RegularExpressions;

namespace WordLogger
{
    class Program
    {
        const string filePathToWords = "words.txt";

        static void Main(string[] args)
        {
            List<string> words = new List<string>();

            GetAllWords(words, filePathToWords);

            Console.WriteLine($"Words {words.Count} words.");

            GetCommands(words, filePathToWords);

            Console.ReadLine();
        }

        private static async Task GetAllWords(List<string> words, string filePathToWords)
        {
            if (File.Exists(filePathToWords))
            {
                await GetWords(words, filePathToWords);
                SaveFile(words, filePathToWords);
            }
            else
            {
                Console.WriteLine($"File '{filePathToWords}' does not exist.");
            }
        }

        private static void GetCommands(List<string> words, string filePathToWords)
        {
            while (true)
            {
                string? input = Console.ReadLine().Trim().ToLower();

                if (input == "q") //quit
                {
                    break;
                }
                else if (input == "d") //del
                {
                    if(DeleteWord(words, filePathToWords))
                        Console.WriteLine("Del");

                    input = "";
                    continue;
                }
                else if (input == "c") //clear
                {
                    Console.Clear();
                    continue;
                }

                TryAddWord(words, input);
            }
        }

        private static void TryAddWord(List<string> words, string word)
        {
            if (words.Contains(word, new CaseInsensitiveEqualityComparer()))
            {
                Console.WriteLine($"'{word}' already exists.");
            }
            else
            {
                AddWord(words, filePathToWords, word);
            }
        }

        public class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode(string obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj);
        }

        private static async void AddWord(List<string> words, string filePath, string input)
        {
            words.Add(input.ToLower());
            Console.WriteLine($"'{input}' added.");

            Console.WriteLine($"Total words: {words.Where(s => !string.IsNullOrEmpty(s)).Count()}");
            await File.AppendAllTextAsync(filePath, $"{input}\n");
        }

        private static bool DeleteWord(List<string> words, string filePath)
        {
            Console.WriteLine("Enter word to delete: ");
            string worldToDelete = Console.ReadLine();
            if (worldToDelete != null)
            {
                if (DeleteWord(filePath, worldToDelete, words))
                {
                    Console.WriteLine($"Total words: {words.Where(s => !string.IsNullOrEmpty(s)).Count()}");
                    words.Clear();
                    GetWords(words, filePath);
                    return true;
                }
                else
                {
                    Console.WriteLine("Not found");
                }
            }

            return false;
        }

        private static async Task GetWords(List<string> words, string filePath)
        {
            string[] fileWords = await File.ReadAllLinesAsync(filePath);
            foreach (string word in fileWords)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    words.Add(word);
                }
            }
        }

        public static bool DeleteWord(string filePath, string wordToDelete, List<string> listOfWords)
        {
            string[] lines = File.ReadAllLines(filePath);
            bool isDeleted = false;
            Regex pattern = new Regex($@"\b{Regex.Escape(wordToDelete)}\b", RegexOptions.IgnoreCase);

            for (int i = 0; i < lines.Length; i++)
            {
                if (pattern.IsMatch(lines[i]) && lines[i].ToLower().Contains(wordToDelete.ToLower()))
                {
                    lines[i] = pattern.Replace(lines[i], string.Empty);
                    isDeleted = true;
                    listOfWords.Remove(lines[i]);
                }
            }

            File.WriteAllLines(filePath, lines);
            return isDeleted;
        }

        public static void SaveFile(List<string> words, string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (StreamWriter sw = File.CreateText(filePath))
            {
                foreach (string word in words)
                {
                    sw.WriteLine(word);
                }
            }
        }
    }
}

