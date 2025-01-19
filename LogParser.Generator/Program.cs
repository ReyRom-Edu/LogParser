namespace LogParser.Generator
{
    public class Program
    {
        static void Main(string[] args)
        {
            string filePath = "access.log"; // Имя выходного файла
            int numberOfLogs = 20_000_000;   // Количество записей
            Random random = new Random();

            // Наборы данных для генерации логов
            string[] methods = { "GET", "POST", "PUT", "DELETE" };
            int[] statuses = { 200, 201, 204, 400, 401, 403, 404, 500, 502, 503 };
            string[] ips = GenerateRandomIPs(100, random);

            // Стартовая дата для логов
            DateTime startDate = new DateTime(2024, 1, 1);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < numberOfLogs; i++)
                {
                    string ip = ips[random.Next(ips.Length)];
                    string method = methods[random.Next(methods.Length)];
                    int status = statuses[random.Next(statuses.Length)];
                    int size = random.Next(200, 5000);
                    DateTime timestamp = startDate.AddSeconds(random.Next(0, 31_536_000)); // максимальная разница между записями - 1 год
                    string timestampStr = timestamp.ToString("dd/MMM/yyyy:HH:mm:ss +0000");

                    // Формат строки лога
                    string logEntry = $"{ip} - - [{timestampStr}] \"{method} /path/to/resource HTTP/1.1\" {status} {size}";
                    writer.WriteLine(logEntry);
                }
            }

            Console.WriteLine($"Файл логов успешно создан: {filePath}");
        }

        // Генерация массива случайных IP-адресов
        static string[] GenerateRandomIPs(int count, Random random)
        {
            string[] ips = new string[count];
            for (int i = 0; i < count; i++)
            {
                ips[i] = $"{random.Next(192, 193)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}";
            }
            return ips;
        }
    }
}
