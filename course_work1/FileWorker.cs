namespace course_work1;

public class FileWorker
{
    public static void WriteResultOfTest(string filename, String time, List<int> visitedNodes)
    {
        // Перевіряємо, чи існує файл
        if (!File.Exists(filename))
        {
            // Створюємо файл, якщо він не існує
            var fileStream = File.Create(filename);
            fileStream.Close();
        }
    
        // Записуємо список у файл
        using (StreamWriter writer = new StreamWriter(filename))
        {
            writer.WriteLine("Algoritm time: " + time);
            writer.WriteLine("Nodes visited " + visitedNodes.Count + "\n");
            writer.WriteLine("Order of visiting:");
            writer.WriteLine(string.Join("->", visitedNodes));
        }
    }
    public static void WriteAdjacencyListToFile(string filename, int[][] adjacencyList)
    {
        // Перевіряємо, чи існує файл
        if (!File.Exists(filename))
        {
            // Створюємо файл, якщо він не існує
            var fileStream = File.Create(filename);
            fileStream.Close();
        }
    
        // Записуємо список у файл
        using (StreamWriter writer = new StreamWriter(filename))
        {
            foreach (var row in adjacencyList)
            {
                if (row != null && row.Length > 0 )
                {
                    writer.WriteLine(string.Join(" ", row));
                }
            }
        }
    }

    public static int[][] ReadAdjacencyListFromFile(string filename)
    {
        using (StreamReader reader = new StreamReader(filename))
        {
            var lines = reader
                .ReadToEnd()
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(' ').Select(int.Parse).ToArray())
                .ToArray();

            // Знаходимо максимальний індекс вузла
            int maxIndex = lines.SelectMany(line => line).Max();

            // Створюємо масив для всіх вузлів від 0 до максимального індексу
            int[][] fullAdjacencyList = new int[maxIndex + 1][];
            for (int i = 0; i <= maxIndex; i++)
            {
                fullAdjacencyList[i] = new int[0];
            }

            // Заповнюємо список суміжності
            for (int i = 0; i < lines.Length; i++)
            {
                fullAdjacencyList[i] = lines[i];
            }

            return fullAdjacencyList;
        }
    }
}
