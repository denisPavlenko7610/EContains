namespace WordLogger
{
    class Program
    {
        static int count = 0;
        static int learnedCount = 0;
        const string filePathToWords = "words.txt";
        const string filePathToLearned = "learnedWords.txt";

        static void Main(string[] args)
        {
            List<string> words = new List<string>();
            List<string> learnedWords = new List<string>();

            count = GetAllWords(words, count, filePathToWords);
            learnedCount = GetAllWords(learnedWords, learnedCount, filePathToLearned);

            count = GetCommands(words, learnedWords, count, filePathToWords, filePathToLearned);

            //Console.WriteLine($"Total words: {count}");
            Console.ReadLine();
        }

        private static int GetAllWords(List<string> words, int count, string filePathToWords)
        {
            if (File.Exists(filePathToWords))
            {
                count = GetWords(words, count, filePathToWords).Result;

                Console.WriteLine($"Loaded {count} words.");
                SaveFile(words, filePathToWords);
            }
            else
            {
                Console.WriteLine($"File '{filePathToWords}' does not exist.");
            }

            return count;
        }

        private static int GetCommands(List<string> words, List<string> learnedWords, int count, string filePathToWords, string filePathToLearned)
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
                else if ((input?.ToLower() == "d")) //del
                {
                    count = DeleteWord(words, count, filePathToWords);
                    input = "";
                    continue;
                }
                else if (input?.ToLower() == "c") //clear
                {
                    Console.Clear();
                    continue;
                }
                else if (input?.ToLower() == "l")
                {
                    Console.WriteLine("What word is learned?");
                    string word = Console.ReadLine();
                    TryAddWord(words, learnedWords, count, word, true);
                    input = "";
                    continue;
                }

                count = TryAddWord(words, learnedWords, count, input, false);
            }

            return count;
        }

        private static int TryAddWord(List<string> words, List<string> learnedWords, int count, string? word, bool isLearnedWord)
        {
            if (isLearnedWord && words.Contains(word.ToLower()))
            {
                DeleteWord(filePathToWords, word, words);
                learnedCount = AddWord(learnedWords, learnedCount, filePathToLearned, word, isLearnedWord);
                Console.WriteLine("Deleted");
                return 0;
            }

            if (words.Contains(word.ToLower()) || learnedWords.Contains(word.ToLower()))
            {
                Console.WriteLine($"'{word}' already exists.");
            }
            else
            {
                count = AddWord(words, count, filePathToWords, word, isLearnedWord);
            }

            return count;
        }

        private static int AddWord(List<string> words, int count, string filePath, string input, bool isLearnedWord)
        {
            words.Add(input.ToLower());
            Console.WriteLine($"'{input}' added.");

            if (!isLearnedWord)
            {
                count++;
                Console.WriteLine($"Total words: {count}");
            }

            File.AppendAllTextAsync(filePath, $"{input}\n");
            return count;
        }

        private static int DeleteWord(List<string> words, int count, string filePath)
        {
            Console.WriteLine("Enter word to delete: ");
            string worldToDelete = Console.ReadLine();
            if (worldToDelete != null)
            {
                if (DeleteWord(filePath, worldToDelete, words))
                {
                    count--;
                    Console.WriteLine($"Total words: {count}");
                    words.Clear();
                    count = 0;
                    count = GetWords(words, count, filePath).Result;
                }
                else
                {
                    Console.WriteLine("Not found");
                }
            }

            return count;
        }

        private static async Task<int> GetWords(List<string> words, int count, string filePath)
        {
            string[] fileWords = await File.ReadAllLinesAsync(filePath);
            foreach (string word in fileWords)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    count++;
                    words.Add(word);
                }
            }

            return count;
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
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].ToLower().Contains(wordToDelete.ToLower()))
                {
                    lines[i] = lines[i].Replace(wordToDelete, "");
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

