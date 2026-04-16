using System.Text;

namespace TextAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            // složka s texty
            string directoryPath = "texty";

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Složka '{directoryPath}' nebyla nalezena.");
                return;
            }
            var textFiles = Directory.EnumerateFiles(directoryPath, "*.txt").ToList();
            
            long totalSize = 0;
            int totalLines = 0;
            int totalWords = 0;
            var globalWordCounts = new Dictionary<string, int>();

            using var csvWriter = new StreamWriter("output/analysis.csv", false, Encoding.UTF8);
			// hlavička
            csvWriter.WriteLine("FileName;Lines;Words;Chars;TopWords"); 

            // soubory
            foreach (var filePath in textFiles)
            {
                var fileInfo = new FileInfo(filePath);
                totalSize += fileInfo.Length;

                int linesCount = 0;
                int wordsCount = 0;
                int charsCount = 0;
                var localWordCounts = new Dictionary<string, int>();

                // řádky
                foreach (var line in File.ReadLines(filePath, Encoding.UTF8))
                {
                    linesCount++;
                    charsCount += line.Count(c => !char.IsWhiteSpace(c));
                    var words = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                    wordsCount += words.Length;

					// slova
                    foreach (var word in words)
                    {
                        string cleanWord = new string(word.Where(c => !char.IsPunctuation(c)).ToArray()).ToLower();

                        if (!string.IsNullOrWhiteSpace(cleanWord))
                        {
                            // přidání do local
                            if (!localWordCounts.ContainsKey(cleanWord))
                                localWordCounts[cleanWord] = 0;
                            localWordCounts[cleanWord]++;

                            // přidání do global
                            if (!globalWordCounts.ContainsKey(cleanWord))
                                globalWordCounts[cleanWord] = 0;
                            globalWordCounts[cleanWord]++;
                        }
                    }
                }

                totalLines += linesCount;
                totalWords += wordsCount;

                // 10 nejčastějších pro soubor 
                var top10WordsLocal = localWordCounts
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(10)
                    .Select(kvp => kvp.Key);

                string topWordsString = string.Join(", ", top10WordsLocal);
                csvWriter.WriteLine($"{fileInfo.Name};{linesCount};{wordsCount};{charsCount};\"{topWordsString}\"");
            }

            // konzole
            Console.WriteLine($"Nalezeno textových souborů: {textFiles.Count}");
            Console.WriteLine($"Celková velikost textových souborů: {totalSize} bajtů");

            // nejčastější slovo
            string mostFrequentWord = globalWordCounts
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .FirstOrDefault() ?? "žádné";

            // summary.txt
            var summary = new StringBuilder();
            summary.AppendLine($"Analyzováno souborů: {textFiles.Count}");
            summary.AppendLine($"Celkem řádků: {totalLines}");
            summary.AppendLine($"Celkem slov: {totalWords}");
            summary.AppendLine($"Nejčastější slovo: \"{mostFrequentWord}\"");

            // zápis
            File.WriteAllText("output/summary.txt", summary.ToString(), Encoding.UTF8);
        }
    }
}