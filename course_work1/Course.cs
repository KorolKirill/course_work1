using System.Diagnostics;
using MPI;

namespace course_work1;

public class Course
{

    static void notParalel()
    {
          int[][] data_for_test2 = new int[][]
        {
            // new int[] { 4, 4 },
            // new int[] { 4, 8 },
            // new int[] { 6, 4 },
            // new int[] { 6, 8 },
            new int[] { 7, 4 },
            new int[] { 7, 8 },
          //  new int[] { 10, 4 },
      
        };
          foreach (var data in data_for_test2)
          {
              int levels = data[0];
              int childrenPerNode = data[1];
              var adjacencyList = GenerateAdjacencyList(levels,childrenPerNode);
              
            Console.WriteLine("\nDepth-First Search order:");
            Console.WriteLine("Levels: " + levels + " Childrens: " + childrenPerNode);
            int nodeCount = adjacencyList.Length;
            Console.WriteLine($"\nTotal nodes in the tree: {nodeCount}");
              // Створення об'єкта Stopwatch для вимірювання часу
              Stopwatch stopwatch = new Stopwatch();
              // Запуск таймера
              stopwatch.Start();
              // Вивід результатів DFS та затраченого часу
              List<int> visitedNodes = DepthFirstSearchIterative(adjacencyList);
          
              // Зупинка таймера
              stopwatch.Stop();
              //    Console.WriteLine(string.Join(" -> ", visitedNodes));
              Console.WriteLine("Час виконання: " + stopwatch.ElapsedMilliseconds / 1000.0 + " с");
              Console.WriteLine("Час виконання: " + stopwatch.ElapsedMilliseconds + " мс");
              // Dictionary<int, List<int>> adjacencyList = ConvertTreeToAdjacencyList(tree.Root);
              // PrintAdjacencyList(adjacencyList);
          
          }
          Console.WriteLine("Finish of the program");
    }
    static void Main(string[] args)
    {
         notParalel();
        return;
        using (new MPI.Environment(ref args))
        {
            int size = Communicator.world.Size;
            int rank = Communicator.world.Rank;

            int[][] adjacencyList = null;
            int[] subtreeRoots = null;
            List<int> localVisitedNodes = null;
            Stopwatch stopwatch = null;

            
            if (rank == 0)
            {
                // Запуск таймера
                stopwatch = new Stopwatch();
                stopwatch.Start();
                int levels = 7;
                int childrenPerNode = size;
                adjacencyList = GenerateAdjacencyList(levels, childrenPerNode);
                Console.WriteLine("Nodes total: " + adjacencyList.Length);

                subtreeRoots = DivideTree(adjacencyList, size);

                for (int i = 1; i < size; i++)
                {
                    Communicator.world.Send<int[][]>(adjacencyList, i, i);
                }

                Console.WriteLine("Sended");
                localVisitedNodes = new List<int>(subtreeRoots);
                localVisitedNodes.Add(0); // 0 - index of Tree Root.
            }
            else
            {
                adjacencyList = Communicator.world.Receive<int[][]>(0, rank); // Whole tree receive.
            }

            // Communicator.world.Broadcast(ref childrenPerNode, 0); // childrens in tree
            var subtree_id = Communicator.world.Scatter(subtreeRoots, 0); // Node id to process.
            Console.WriteLine("task for tr: " + rank + ", node id to proceed: " + subtree_id);

            if (localVisitedNodes == null)
            {
                localVisitedNodes = DepthFirstSearchIterative(adjacencyList, subtree_id);
            }
            else
            {
                localVisitedNodes.AddRange(DepthFirstSearchIterative(adjacencyList, subtree_id));
            }

            Console.WriteLine("Algo is completed");

            var results = Communicator.world.Allgather(localVisitedNodes.ToArray());
            if (rank == 0)
            {
                var fullpath = new List<int>();
                foreach (var result in results)
                {
                    fullpath.AddRange(result);
                }

                //      Console.WriteLine(string.Join(" -> ", fullpath));
                // Зупинка таймера
                stopwatch.Stop();
                Console.WriteLine("Час виконання: " + stopwatch.ElapsedMilliseconds / 1000.0 + " с");
                Console.WriteLine("Час виконання: " + stopwatch.ElapsedMilliseconds + " мс");
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
        if (adjacencyList == null || startNodeIndex >= adjacencyList.Length ||
            adjacencyList[startNodeIndex] == null)
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

                if (currentNodeIndex < adjacencyList.Length && adjacencyList[currentNodeIndex] != null)
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
        int rootNode = Array.FindIndex(adjacencyList, arr => arr != null);
        queue.Enqueue(rootNode);

        while (subtreeRoots.Count < numSubtrees && queue.Count > 0)
        {
            int currentNode = queue.Dequeue();

            if (adjacencyList[currentNode] != null && adjacencyList[currentNode].Length > 0)
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