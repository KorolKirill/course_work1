using System;
using System.Collections.Generic;
using System.Diagnostics;
using MPI;

public static class Helper
{
    public static Dictionary<int, List<int>> GenerateAdjacencyList(int levels, int childrenPerNode)
    {
        Dictionary<int, List<int>> adjacencyList = new Dictionary<int, List<int>>();

        if (levels <= 0 || childrenPerNode <= 0)
        {
            return null;
        }

        int nodeId = 0;
        int nodesInCurrentLevel = 1;

        for (int level = 0; level < levels - 1; level++)
        {
            int nodesInNextLevel = nodesInCurrentLevel * childrenPerNode;
            int nodeInNextLevel = nodeId + nodesInCurrentLevel;

            for (int i = 0; i < nodesInCurrentLevel; i++)
            {
                List<int> children = new List<int>();

                for (int j = 0; j < childrenPerNode; j++)
                {
                    children.Add(nodeInNextLevel++);
                }

                adjacencyList[nodeId++] = children;
            }

            nodesInCurrentLevel = nodesInNextLevel;
        }

        return adjacencyList;
    }
      
      public static List<int> DivideTree(Dictionary<int, List<int>> adjacencyList, int numSubtrees)
{
    List<int> subtreeRoots = new List<int>();

    Queue<int> queue = new Queue<int>();
    int rootNode = adjacencyList.Keys.First();
    queue.Enqueue(rootNode);

    while (subtreeRoots.Count < numSubtrees && queue.Count > 0)
    {
        int currentNode = queue.Dequeue();

        if (adjacencyList.ContainsKey(currentNode) && adjacencyList[currentNode].Count > 0)
        {
            foreach (int child in adjacencyList[currentNode])
            {
                subtreeRoots.Add(child);
                queue.Enqueue(child);
            }
        }
    }

    return subtreeRoots;
}


    
    public static List<int> DepthFirstSearchIterative(Dictionary<int, List<int>> adjacencyList, int startNodeIndex)
    {
        if (adjacencyList == null || !adjacencyList.ContainsKey(startNodeIndex))
        {
            return null;
        }

        Stack<int> stack = new Stack<int>();
        List<int> visitedNodes = new List<int>();
        stack.Push(startNodeIndex);

        while (stack.Count > 0)
        {
            int currentNodeIndex = stack.Pop();

            if (!visitedNodes.Contains(currentNodeIndex))
            {
                // Console.WriteLine($"Посещенный узел: {currentNodeIndex}");
                visitedNodes.Add(currentNodeIndex);

                if (adjacencyList.ContainsKey(currentNodeIndex))
                {
                    for (int i = adjacencyList[currentNodeIndex].Count - 1; i >= 0; i--)
                    {
                        stack.Push(adjacencyList[currentNodeIndex][i]);
                    }
                }
            }
        }

        return visitedNodes;
    }

    public static void PrintAdjacencyList(Dictionary<int, List<int>> adjacencyList)
    {
        Console.WriteLine("\nAdjacency List:");
        foreach (var entry in adjacencyList)
        {
            Console.WriteLine($"Node {entry.Key}: {string.Join(", ", entry.Value)}");
        }
    }
    public static void PrintTree(Dictionary<int, List<int>> adjacencyList, int nodeId = 0, int depth = 0)
    {
        Console.WriteLine(new string(' ', depth * 2) + "Node ID: " + nodeId);

        if (adjacencyList.ContainsKey(nodeId))
        {
            foreach (int childId in adjacencyList[nodeId])
            {
                PrintTree(adjacencyList, childId, depth + 1);
            }
        }
    }
    public static int[][] AdjacencyListToArray(Dictionary<int, List<int>> adjacencyList)
    {
        int maxNodeId = adjacencyList.Keys.Max();
        int[][] adjacencyArray = new int[maxNodeId + 1][];

        foreach (var entry in adjacencyList)
        {
            int nodeId = entry.Key;
            List<int> neighbors = entry.Value;
            adjacencyArray[nodeId] = neighbors.ToArray();
        }

        return adjacencyArray;
    }


}

public class Program2
{
    static void Main1(string[] args)
    {
        using (new MPI.Environment(ref args))
        {
//     int size = Communicator.world.Size;
//     int rank = Communicator.world.Rank;
//
//     Dictionary<int, List<int>> adjacencyList = null;
//     List<int> subtreeRoots = null;
//
//     if (rank == 0)
//     {
//         int levels = 5;
//         int childrenPerNode = 3;
//
//         adjacencyList = Helper. GenerateAdjacencyList(levels, childrenPerNode);
//
//         int rootNodeId = adjacencyList.Keys.First();
//         subtreeRoots = Helper. DivideTree(adjacencyList, size);
//
//         Console.WriteLine("Дерево:");
//         Helper.  PrintTree(adjacencyList, rootNodeId, 0);
//         Console.WriteLine("\nРазделение дерева на поддеревья:");
//         for (int i = 0; i < subtreeRoots.Count; i++)
//         {
//             Console.WriteLine($"Поддерево {i}:");
//             Helper. PrintTree(adjacencyList, subtreeRoots[i], 1);
//         }
//     }
//
//     int[] subtreeRootsArray = subtreeRoots?.ToArray() ?? new int[0];
//     int localRootNodeId =  Communicator.world.Scatter(subtreeRootsArray, 0);;
//
//     if (localRootNodeId != -1)
//     {
//         var adjacencyList1 = Communicator.world.Broadcast<int[]>( Helper.AdjacencyListToArray(adjacencyList), 0);
//         
//         List<int> localVisitedNodes = Helper. DepthFirstSearchIterative(adjacencyList, localRootNodeId);
//
//         Console.WriteLine($"\nПроцесс {rank}:");
//         Console.WriteLine($"Обход в глубину для поддерева с корнем {localRootNodeId}:");
//         foreach (int nodeId in localVisitedNodes)
//         {
//             Console.WriteLine("Посещенный узел: " + nodeId);
//         }
//     }
//     MPI.
// }


            // public static void Main(string[] args)
            // {
            //
            //     int[][] data_for_test2 = new int[][]
            //     {
            //         new int[] { 4, 4 },
            //         new int[] { 5, 5 },
            //         new int[] { 6, 6 },
            //         new int[] { 7, 7 },
            //         new int[] { 7, 8 },
            //         new int[] { 7, 9 },
            //         new int[] { 8, 8 },
            //     };
            //     // int levels = 3;
            //     // int childrenPerNode = 4;
            //     // var tree = Helper.GenerateAdjacencyList(levels, childrenPerNode);
            //     // Helper.PrintTree(tree);
            //     
            //     using (new MPI.Environment(ref args))
            //     {
            //         int size = Communicator.world.Size;
            //         int rank = Communicator.world.Rank;
            //
            //         Dictionary<int, List<int>> adjacencyList = null;
            //         List<int> subtreeRoots = null;
            //
            //         if (rank == 0)
            //         {
            //             int levels = 5;
            //             int childrenPerNode = 3;
            //
            //             adjacencyList = Helper.GenerateAdjacencyList(levels, childrenPerNode);
            //
            //             subtreeRoots = Helper.DivideTree(adjacencyList, size);
            //
            //             Console.WriteLine("Дерево:");
            //            Helper.PrintTree(adjacencyList);
            //             Console.WriteLine("\nРазделение дерева на поддеревья:");
            //             for (int i = 0; i < subtreeRoots.Count; i++)
            //             {
            //                 Console.WriteLine($"Поддерево {i}:");
            //                 Helper. PrintTree(adjacencyList, subtreeRoots[i], 1);
            //             }
            //         }
            //
            //         subtreeRoots = Communicator.world.Scatter(subtreeRoots, 0);
            //
            //         if (subtreeRoots != null && subtreeRoots.Count > rank)
            //         {
            //             int localRootNodeId = subtreeRoots[rank];
            //             List<int> localVisitedNodes = DepthFirstSearchIterative(adjacencyList, localRootNodeId);
            //
            //             Console.WriteLine($"\nПроцесс {rank}:");
            //             Console.WriteLine($"Обход в глубину для поддерева с корнем {localRootNodeId}:");
            //             foreach (int nodeId in localVisitedNodes)
            //             {
            //                 Console.WriteLine("Посещенный узел: " + nodeId);
            //             }
            //         }
            //     }
            // }

            // using (new MPI.Environment(ref args))
            // {
            //     Communicator world  = Communicator.world;
            //     // Получение ранга текущего процесса и общего количества процессов
            //     int rank = Communicator.world.Rank;
            //     int size = Communicator.world.Size;
            //     List<Node> subtrees = null;
            //     if (rank == 0 )
            //     {
            //         Console.WriteLine("Beginning of the program");
            //         // Разделение дерева на поддеревья для параллельной обработки
            //         subtrees = Tree.DivideTree(tree.Root, size);
            //     }
            //  
            //     
            //     // Распределение поддеревьев между процессами
            //     Node localSubtree = Communicator.world.Scatter(subtrees.ToArray(), 0);
            //
            //     // Выполнение итеративного алгоритма поиска в глубину для локального поддерева
            //     List<int> localVisitedNodes = Helper.DepthFirstSearchIterative(localSubtree);
            //
            //     // Сбор результатов от всех процессов
            //     List<int>[] allVisitedNodes = Communicator.world.Gather(localVisitedNodes, 0);
            //
            //     // Вывод результатов (только для процесса с рангом 0)
            //     if (rank == 0)
            //     {
            //         Console.WriteLine("Depth-First Search order:");
            //         Console.WriteLine($"Levels: {levels} Childrens: {childrenPerNode}");
            //         List<int> combinedVisitedNodes = new List<int>();
            //
            //         foreach (var visitedNodes in allVisitedNodes)
            //         {
            //             combinedVisitedNodes.AddRange(visitedNodes);
            //         }
            //
            //         Console.WriteLine("Visited nodes:");
            //         Console.WriteLine(string.Join(" -> ", combinedVisitedNodes));
            //     }
            // }
            // foreach (var data in data_for_test2)
            // {
            //     int levels = data[0];
            //     int childrenPerNode = data[1];
            //     Tree tree = new Tree(levels, childrenPerNode);
            //     
            //   //  PrintTree(tree.Root, 0);
            //   Console.WriteLine("\nDepth-First Search order:");
            //   Console.WriteLine("Levels: " + levels + " Childrens: " + childrenPerNode);
            //   int nodeCount = tree.CountNodes();
            //   Console.WriteLine($"\nTotal nodes in the tree: {nodeCount}");
            //     // Створення об'єкта Stopwatch для вимірювання часу
            //     Stopwatch stopwatch = new Stopwatch();
            //     // Запуск таймера
            //     stopwatch.Start();
            //     // Вивід результатів DFS та затраченого часу
            //     List<int> visitedNodes = tree.DepthFirstSearchIterative(tree.Root);
            //
            //     // Зупинка таймера
            //     stopwatch.Stop();
            //     //    Console.WriteLine(string.Join(" -> ", visitedNodes));
            //     Console.WriteLine("Час виконання: " + stopwatch.ElapsedMilliseconds / 1000.0 + " с");
            //     Console.WriteLine("Час виконання: " + stopwatch.ElapsedMilliseconds + " мс");
            //     // Dictionary<int, List<int>> adjacencyList = ConvertTreeToAdjacencyList(tree.Root);
            //     // PrintAdjacencyList(adjacencyList);
            //
            // }
            // Console.WriteLine("Finish of the program");
        }
    }
}