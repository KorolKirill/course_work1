namespace course_work1;

public class FileWorker
{
    
    public static void WriteResultOfTest(string filename, String time, List<int> visitedNodes)
    {
        // Проверяем, существует ли файл
        if (!File.Exists(filename))
        {
            // Создаем файл, если он не существует
            var fileStream = File.Create(filename);
            fileStream.Close();
        }
    
        // Записываем список в файл
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
        // Проверяем, существует ли файл
        if (!File.Exists(filename))
        {
            // Создаем файл, если он не существует
            var fileStream = File.Create(filename);
            fileStream.Close();
        }
    
        // Записываем список в файл
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

            // Находим максимальный индекс узла
            int maxIndex = lines.SelectMany(line => line).Max();

            // Создаем массив для всех узлов от 0 до максимального индекса
            int[][] fullAdjacencyList = new int[maxIndex + 1][];
            for (int i = 0; i <= maxIndex; i++)
            {
                fullAdjacencyList[i] = new int[0];
            }

            // Заполняем список смежности
            for (int i = 0; i < lines.Length; i++)
            {
                fullAdjacencyList[i] = lines[i];
            }

            return fullAdjacencyList;
        }
    }

}