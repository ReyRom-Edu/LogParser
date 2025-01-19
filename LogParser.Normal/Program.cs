using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LogParser.Normal
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = "access.log"; // Путь к файлу логов
            int chunkSize = 10_000; // Количество строк на блок

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл логов не найден.");
                return;
            }

            // Переменные для хранения результатов
            var requestTypes = new Dictionary<string, int>();
            var ipAddresses = new Dictionary<string, int>();
            int total4xxErrors = 0;


            // Регулярное выражение для разбора строки лога
            var logRegex = new Regex(@"^(?<ip>\S+) .* ""(?<method>GET|POST|PUT|DELETE) .*"" (?<status>\d{3})", RegexOptions.Compiled);

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                using (var fileReader = new StreamReader(filePath))
                {
                    var linesBuffer = new string[chunkSize];
                    int linesRead;

                    // Чтение файла по частям
                    while ((linesRead = ReadLines(fileReader, linesBuffer)) > 0)
                    {
                        // Обработка блока строк
                        foreach (var line in linesBuffer)
                        {
                            var match = logRegex.Match(line);
                            if (match.Success)
                            {
                                // Извлечение данных из строки лога
                                var ip = match.Groups["ip"].Value;
                                var method = match.Groups["method"].Value;
                                var status = int.Parse(match.Groups["status"].Value);

                                // Обновление результатов
                                if (ipAddresses.ContainsKey(ip))
                                {
                                    ipAddresses[ip] += 1;
                                }
                                else ipAddresses.Add(ip, 1);

                                if (requestTypes.ContainsKey(method))
                                {
                                    requestTypes[method] += 1;
                                }
                                else requestTypes.Add(method, 1);

                                if (status >= 400 && status < 500)
                                    total4xxErrors++;
                            }
                        }
                    }
                }

                // Наиболее частый IP-адрес
                var mostFrequentIp = ipAddresses.Max();

                // Вывод результатов
                Console.WriteLine("Анализ завершён:");
                Console.WriteLine("1. Количество запросов по типу:");
                foreach (var kvp in requestTypes)
                    Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
                Console.WriteLine($"2. Наиболее частый IP-адрес: {mostFrequentIp.Key} (встречался {mostFrequentIp.Value} раз).");
                Console.WriteLine($"3. Количество запросов с кодом ответа 4xx: {total4xxErrors}");
                sw.Stop();
                Console.WriteLine($"Времени затрачено: {sw.Elapsed}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки: {ex.Message}");
            }
        }

        // Чтение указанного количества строк из файла
        static int ReadLines(StreamReader reader, string[] buffer)
        {
            int count = 0;
            while (count < buffer.Length && !reader.EndOfStream)
            {
                buffer[count++] = reader.ReadLine();
            }
            return count;
        }
    }
}
