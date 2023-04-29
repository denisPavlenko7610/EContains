namespace WordLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> words = new List<string>();
            int count = 0;
            string filePath = "words.txt";

            if (File.Exists(filePath))
            {
                count = GetWords(words, count, filePath);

                Console.WriteLine($"Loaded {words.Count} words.");
                SaveFile(words, filePath);
            }
            else
            {
                Console.WriteLine($"File '{filePath}' does not exist. Creating new file.");
            }

            while (true)
            {
                string? input = Console.ReadLine();

                if (input?.ToLower() == "q") //quit
                {
                    break;
                }
                else if (input?.ToLower() == "g") //get
                {
                    Console.WriteLine(GetRandomWord(words));
                    continue;
                }
                else if ((input?.ToLower() == "d")) //del
                {
                    count = DeleteWord(words, count, filePath);

                    continue;
                }
                else if (input?.ToLower() == "c") //clear
                {
                    Console.Clear();
                }

                if (words.Contains(input?.ToLower()))
                {
                    Console.WriteLine($"'{input}' already exists.");
                }
                else
                {
                    count = AddWord(words, count, filePath, input);
                }
            }

            Console.WriteLine($"Total words: {count}");
            Console.ReadLine();
        }

        private static int AddWord(List<string> words, int count, string filePath, string? input)
        {
            words.Add(input.ToLower());
            count++;
            Console.WriteLine($"'{input}' added.");
            Console.WriteLine($"Total words: {count}");
            File.AppendAllTextAsync(filePath, $"{input}\n");
            return count;
        }

        private static int DeleteWord(List<string> words, int count, string filePath)
        {
            Console.WriteLine("Enter word to delete: ");
            string worldToDelete = Console.ReadLine();
            if (worldToDelete != null)
            {
                if (DeleteWord(filePath, worldToDelete))
                {
                    count--;
                    Console.WriteLine($"Total words: {count}");
                    words.Clear();
                    count = 0;
                    count = GetWords(words, count, filePath);
                }
                else
                {
                    Console.WriteLine("Not found");
                }
            }

            return count;
        }

        private static int GetWords(List<string> words, int count, string filePath)
        {
            string[] fileWords = File.ReadAllLines(filePath);
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

        public static bool DeleteWord(string filePath, string wordToDelete)
        {
            string[] lines = File.ReadAllLines(filePath);
            bool isDeleted = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].ToLower() == wordToDelete.ToLower())
                {
                    lines[i] = lines[i].Replace(wordToDelete, "");
                    isDeleted = true;
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

