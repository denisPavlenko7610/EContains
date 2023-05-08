using System.Text.RegularExpressions;

namespace WordLogger
{
    class Program
    {
        const string filePathToWords = "words.txt";
        const string filePathToLearned = "learnedWords.txt";

        static void Main(string[] args)
        {
            List<string> words = new List<string>();
            List<string> learnedWords = new List<string>();

            GetAllWords(words, filePathToWords);
            GetAllWords(learnedWords, filePathToLearned);

            Console.WriteLine($"Words {words.Count} words.");
            Console.WriteLine($"Learned {learnedWords.Count} words.");

            GetCommands(words, learnedWords, filePathToWords, filePathToLearned);

            Console.ReadLine();
        }

        private static async void GetAllWords(List<string> words, string filePathToWords)
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

        private static void GetCommands(List<string> words, List<string> learnedWords, string filePathToWords, string filePathToLearned)
        {
            while (true)
            {
                string? input = Console.ReadLine().Trim();

                if (input?.ToLower() == "q") //quit
                {
                    break;
                }
                else if (input?.ToLower() == "g") //get
                {
                    Console.WriteLine(GetRandomWord(words));
                    input = "";
                    continue;
                }
                else if (input?.ToLower() == "d") //del
                {
                    DeleteWord(words, filePathToWords);
                    input = "";
                    continue;
                }
                else if (input?.ToLower() == "c") //clear
                {
                    Console.Clear();
                    continue;
                }
                else if (input?.ToLower() == "l") //learn
                {
                    Console.WriteLine("What word is learned?");
                    string word = Console.ReadLine();
                    TryAddWord(words, learnedWords, word, true);
                    input = "";
                    continue;
                }

                TryAddWord(words, learnedWords, input, false);
            }
        }

        private static void TryAddWord(List<string> words, List<string> learnedWords, string? word, bool isLearnedWord)
        {
            if (isLearnedWord && words.Contains(word.ToLower()))
            {
                DeleteWord(filePathToWords, word, words);
                AddWord(learnedWords, filePathToLearned, word, isLearnedWord);
                Console.WriteLine("Deleted");
                return;
            }

            if (words.Contains(word.ToLower()) || learnedWords.Contains(word.ToLower()))
            {
                Console.WriteLine($"'{word}' already exists.");
            }
            else
            {
                AddWord(words, filePathToWords, word, isLearnedWord);
            }
        }

        private static async void AddWord(List<string> words, string filePath, string input, bool isLearnedWord)
        {
            words.Add(input.ToLower());
            Console.WriteLine($"'{input}' added.");

            if (!isLearnedWord)
            {
                Console.WriteLine($"Total words: {words.Where(s => !string.IsNullOrEmpty(s)).Count()}");
            }

            await File.AppendAllTextAsync(filePath, $"{input}\n");
        }

        private static void DeleteWord(List<string> words, string filePath)
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
                }
                else
                {
                    Console.WriteLine("Not found");
                }
            }
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

        public static string GetRandomWord(List<string> words)
        {
            Random rand = new Random();
            int index = rand.Next(words.Count);
            return words[index];
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
                    Console.WriteLine("Del");
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

