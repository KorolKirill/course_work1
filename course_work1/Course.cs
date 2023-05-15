using System.Diagnostics;
using MPI;

namespace course_work1;

public class Course
{
    private static String prepared_data_path = "C:\\Users\\korol\\RiderProjects\\course_work1\\prepared_data\\";
    private static String results_data_path = "C:\\Users\\korol\\RiderProjects\\course_work1\\test_results\\";
    static void prepareData()
    {
        int[][] data_for_test2 = new int[][]
        {
            new int[] { 4, 4 },
            new int[] { 4, 8 },
            new int[] { 6, 4 },
            new int[] { 6, 8 },
            new int[] { 7, 4 },
            new int[] { 7, 8 },
        };
        foreach (var data in data_for_test2)
        {
            int levels = data[0];
            int childrenPerNode = data[1];
            var adjacencyList = GenerateAdjacencyList(levels, childrenPerNode);
            var filePath = prepared_data_path + levels + "-" + childrenPerNode + ".txt";
            FileWorker.WriteAdjacencyListToFile(filePath,adjacencyList);
            Console.WriteLine("Дерево з рiвнями: "+levels+", та дiтьми: " +childrenPerNode+", має узлiв: " +adjacencyList.Length);
        }
    }

    static void notParalel()
    {
        int[][] data_for_test2 = new int[][]
        {
            new int[] { 4, 4 },
            new int[] { 4, 8 },
            new int[] { 6, 4 },
            new int[] { 6, 8 },
            new int[] { 7, 4 },
            new int[] { 7, 8 },
        };
          foreach (var data in data_for_test2)
          {
              int levels = data[0];
              int childrenPerNode = data[1];
              var filePath = prepared_data_path + levels + "-" + childrenPerNode + ".txt";
              var adjacencyList = FileWorker.ReadAdjacencyListFromFile(filePath);
              Console.WriteLine("\nDepth-First Search order:");
              Console.WriteLine("Дерево з рiвнями: "+levels+", та дiтьми: " +childrenPerNode);
              Console.WriteLine(adjacencyList.Length);
             // Створення об'єкта Stopwatch для вимірювання часу
              Stopwatch stopwatch = new Stopwatch();
              stopwatch.Start();
              // Вивід результатів DFS та затраченого часу
              List<int> visitedNodes = DepthFirstSearchIterative(adjacencyList);
              // Зупинка таймера
              stopwatch.Stop();
              Console.WriteLine("Час виконання: " + stopwatch.ElapsedMilliseconds+ " мс");
              Console.WriteLine("Nodes visited: "+ visitedNodes.Count);
              //Save results to file.
              String typeOfalgo = "consistent";
              var filePathResult = results_data_path + typeOfalgo+  "-"+ levels + "-" + childrenPerNode + ".txt";
              FileWorker.WriteResultOfTest(filePathResult, stopwatch.ElapsedMilliseconds+ "мс", visitedNodes);

          }
    }
    static void Main(string[] args)
    {
        using (new MPI.Environment(ref args))
        {
            int size = Communicator.world.Size;
            int rank = Communicator.world.Rank;
            int levels = 0;
            int childrenPerNode = 0;
            int[][] adjacencyList;
            int[] subtreeRoots = null;
            List<int> localVisitedNodes = null;
            Stopwatch stopwatch = null;
            
            if (rank == 0)
            {
                levels = 7;
                childrenPerNode = size;
                var filePath = prepared_data_path + levels + "-" + childrenPerNode + ".txt";
                adjacencyList = FileWorker.ReadAdjacencyListFromFile(filePath);
                // Запуск таймера
                stopwatch = new Stopwatch();
                stopwatch.Start();
                subtreeRoots = DivideTree(adjacencyList, size);
                for (int i = 1; i < size; i++)
                {
                    Communicator.world.Send<int[][]>(adjacencyList, i, i);
                }
            }
            else
            {
                adjacencyList = Communicator.world.Receive<int[][]>(0, rank); // Whole tree receive.
            }
            
            var subtree_id = Communicator.world.Scatter(subtreeRoots, 0); // Node id to process.
            localVisitedNodes = DepthFirstSearchIterative(adjacencyList, subtree_id);

            var results = Communicator.world.Gather(localVisitedNodes.ToArray(), 0);
            if (rank == 0)
            {
                var visitedNodesGlobal = new List<int>();
                visitedNodesGlobal.Add(0);
                foreach (var result in results)
                {
                    visitedNodesGlobal.AddRange(result);
                }
                // Зупинка таймера
                stopwatch.Stop();
                Console.WriteLine("\nDepth-First Search order:");
                Console.WriteLine("Дерево з рiвнями: "+levels+", та дiтьми: " +childrenPerNode);
                Console.WriteLine("Час виконання: " + stopwatch.ElapsedMilliseconds+ " мс");
                Console.WriteLine("Nodes visited: "+ visitedNodesGlobal.Count);
                //Save results to file.
                String typeOfalgo = "paralel";
                var filePathResult = results_data_path + typeOfalgo+  "-"+ levels + "-" + childrenPerNode + ".txt";
                FileWorker.WriteResultOfTest(filePathResult, stopwatch.ElapsedMilliseconds+ "мс", visitedNodesGlobal);
            }
        }
    }


    static int[][] GenerateAdjacencyList(int levels, int childrenPerNode)
    {
        levels++;
        int totalNodes = (int)(Math.Pow(childrenPerNode, levels) - 1) / (childrenPerNode - 1);
        int[][] adjacencyList = new int[totalNodes][];

        int currentId = 1;
        int parentNodeId = 0;
        int nodesOnPrevLevels = 0;
        for (int level = 0; level < levels - 1; level++)
        {
            int nodesOnCurrentLevel = (int)Math.Pow(childrenPerNode, level);
            for (int nodeIndex = 0; nodeIndex < nodesOnCurrentLevel; nodeIndex++)
            {
                int[] children = new int[childrenPerNode];

                for (int i = 0; i < childrenPerNode; i++)
                {
                    children[i] = currentId++;
                }

                adjacencyList[parentNodeId++] = children;
            }

            nodesOnPrevLevels += nodesOnCurrentLevel;
            if (nodesOnPrevLevels >= totalNodes)
            {
                break;
            }
        }

        return adjacencyList;
    }



    static void PrintTree(int[][] adjacencyList, int nodeId = 0, int depth = 0)
    {
        Console.WriteLine(new string(' ', depth * 2) + "Node ID: " + nodeId);

        if (nodeId < adjacencyList.Length && adjacencyList[nodeId] != null)
        {
            foreach (int childId in adjacencyList[nodeId])
            {
                PrintTree(adjacencyList, childId, depth + 1);
            }
        }
    }


    static void PrintAdjacencyList(int[][] adjacencyList)
    {
        Console.WriteLine("\nAdjacency List:");
        for (int i = 0; i < adjacencyList.Length; i++)
        {
            if (adjacencyList[i] != null)
            {
                Console.WriteLine($"Node {i}: {string.Join(", ", adjacencyList[i])}");
            }
        }
    }

    static List<int> DepthFirstSearchIterative(int[][] adjacencyList, int  startNodeIndex= 0)
    {
        Stack<int> stack = new Stack<int>();
        List<int> visitedNodes = new List<int>();
        stack.Push(startNodeIndex);

        while (stack.Count > 0)
        {
            int currentNodeIndex = stack.Pop();

            if (!visitedNodes.Contains(currentNodeIndex))
            {
                visitedNodes.Add(currentNodeIndex);

                if (currentNodeIndex < adjacencyList.Length)
                {
                    for (int i = adjacencyList[currentNodeIndex].Length - 1; i >= 0; i--)
                    {
                        stack.Push(adjacencyList[currentNodeIndex][i]);
                    }
                }
            }
        }

        return visitedNodes;
    }

    static int[] DivideTree(int[][] adjacencyList, int numSubtrees)
    {
        List<int> subtreeRoots = new List<int>();
        Queue<int> queue = new Queue<int>();
        int rootNode = 0;
        queue.Enqueue(rootNode);

        while (subtreeRoots.Count < numSubtrees && queue.Count > 0)
        {
            int currentNode = queue.Dequeue();

            if (adjacencyList[currentNode].Length > 0)
            {
                foreach (int child in adjacencyList[currentNode])
                {
                    subtreeRoots.Add(child);
                    queue.Enqueue(child);
                }
            }
        }

        return subtreeRoots.ToArray();
    }
}