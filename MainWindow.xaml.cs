using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GridSearchVisualization
{
    public partial class MainWindow : Window
    {
        private readonly SolidColorBrush StartColor = Brushes.Yellow;        
        private readonly SolidColorBrush GoalColor = Brushes.White;          
        private readonly SolidColorBrush PathColor = Brushes.Orange;         
        private readonly SolidColorBrush BackgroundColor = Brushes.DarkBlue; 
        private readonly SolidColorBrush WallColor = Brushes.Black;          

        
        private const int ROWS = 20;
        private const int COLS = 20;
        private const double CELL_SIZE = 20;
        private const double MARGIN = 1;


        private int[,] grid = new int[ROWS, COLS];
        private readonly (int, int) start = (0, 0);
        private readonly (int, int) goal = (19, 19); 
        private Dictionary<string, List<(int, int)>> algorithmPaths = new Dictionary<string, List<(int, int)>>()
        {
            { "BFS", new List<(int, int)>() },
            { "DFS", new List<(int, int)>() },
            { "IDS", new List<(int, int)>() }
        };
        private List<(int, int)> currentPath = new List<(int, int)>();

        private readonly List<(int, int)> directions = new List<(int, int)>
        {
            (-1, 0), 
            (1, 0),  
            (0, -1), 
            (0, 1)   
        };

        
        private string currentMode = "BFS";

        public MainWindow()
        {
            InitializeComponent();
            CurrentModeText.Text = $"Current Mode: {currentMode}";
            WallProbabilitySlider.ValueChanged += WallProbabilitySlider_ValueChanged;
            GenerateGrid();
            RunSearchAlgorithms();
            DrawGrid();
            UpdateMetricsDisplay();
        }





        private void WallProbabilitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WallProbabilityText != null)
            {
                WallProbabilityText.Text = e.NewValue.ToString("0.0");
            }
        }

        // Generate the grid with a guaranteed path from start to goal
        private void GenerateGrid(double wallProbability = 0.5)
        {
            // Initialize grid to empty
            for (int x = 0; x < ROWS; x++)
            {
                for (int y = 0; y < COLS; y++)
                {
                    grid[x, y] = 0; // 0 for open space
                }
            }

            // Generate a random path from start to goal
            List<(int, int)> path = GenerateRandomPath(start, goal);
            foreach (var cell in path)
            {
                grid[cell.Item1, cell.Item2] = 0; // Ensure path cells are open
            }

            // Randomly place walls, excluding the path
            Random rand = new Random();
            for (int x = 0; x < ROWS; x++)
            {
                for (int y = 0; y < COLS; y++)
                {
                    if ((x, y) == start || (x, y) == goal)
                    {
                        grid[x, y] = 0; // Ensure start and goal are open
                    }
                    else if (!path.Contains((x, y)))
                    {
                        grid[x, y] = rand.NextDouble() < wallProbability ? 1 : 0;
                    }
                }
            }

            // Clear previous paths
            foreach (var algo in algorithmPaths.Keys.ToList())
            {
                algorithmPaths[algo].Clear();
            }
            currentPath.Clear();
            DrawGrid();
        }

        // Generate a random path from start to goal
        private List<(int, int)> GenerateRandomPath((int, int) start, (int, int) goal)
        {
            List<(int, int)> path = new List<(int, int)>();
            (int, int) current = start;
            path.Add(current);
            Random rand = new Random();

            while (current != goal)
            {
                List<(int, int)> possibleMoves = new List<(int, int)>();

                // Determine possible moves that bring us closer to the goal
                if (current.Item1 < goal.Item1)
                {
                    possibleMoves.Add((current.Item1 + 1, current.Item2)); // Move Down
                }
                if (current.Item2 < goal.Item2)
                {
                    possibleMoves.Add((current.Item1, current.Item2 + 1)); // Move Right
                }
                if (current.Item1 > 0)
                {
                    possibleMoves.Add((current.Item1 - 1, current.Item2)); // Move Up
                }
                if (current.Item2 > 0)
                {
                    possibleMoves.Add((current.Item1, current.Item2 - 1)); // Move Left
                }

                // Filter moves that do not go out of bounds
                possibleMoves = possibleMoves.Where(move => move.Item1 >= 0 && move.Item1 < ROWS && move.Item2 >= 0 && move.Item2 < COLS).ToList();

                // Randomly select the next move
                if (possibleMoves.Count > 0)
                {
                    current = possibleMoves[rand.Next(possibleMoves.Count)];
                    // Avoid cycles by checking if the cell is already in the path
                    if (!path.Contains(current))
                    {
                        path.Add(current);
                    }
                    else
                    {
                        // If cycle detected, remove the last cell and continue
                        path.RemoveAt(path.Count - 1);
                    }
                }
                else
                {
                    // If no possible moves, backtrack
                    path.RemoveAt(path.Count - 1);
                    if (path.Count == 0)
                    {
                        // If path is empty, restart
                        current = start;
                        path.Add(current);
                    }
                    else
                    {
                        current = path[path.Count - 1];
                    }
                }
            }

            return path;
        }

        // Check if a cell is valid and not a wall
        private bool is_valid(int x, int y, int[,] grid)
        {
            bool valid = x >= 0 && x < ROWS && y >= 0 && y < COLS && grid[x, y] == 0;
            return valid;
        }

        // BFS Implementation with Parent Pointers
        private (List<(int, int)> path, int nodesVisited, int maxFrontier) BFS((int, int) start, (int, int) goal)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Queue<(int, int)> queue = new Queue<(int, int)>();
            Dictionary<(int, int), (int, int)> parents = new Dictionary<(int, int), (int, int)>();
            HashSet<(int, int)> visited = new HashSet<(int, int)>();

            queue.Enqueue(start);
            visited.Add(start);

            int nodesVisited = 0;
            int maxFrontier = 1;

            while (queue.Count > 0)
            {
                maxFrontier = Math.Max(maxFrontier, queue.Count);

                int currentLevelSize = queue.Count;
                for (int i = 0; i < currentLevelSize; i++)
                {
                    var current = queue.Dequeue();
                    nodesVisited++;

                    if (current == goal)
                    {
                        stopwatch.Stop();
                        List<(int, int)> path = ReconstructPath(parents, start, goal);
                        PerformanceDataRepository.PerformanceData["BFS"].ExecutionTimes.Add(stopwatch.Elapsed.TotalSeconds);
                        PerformanceDataRepository.PerformanceData["BFS"].NodesVisited.Add(nodesVisited);
                        PerformanceDataRepository.PerformanceData["BFS"].MaxFrontier.Add(maxFrontier);
                        PerformanceDataRepository.PerformanceData["BFS"].PathLengths.Add(path.Count);
                        algorithmPaths["BFS"] = path;
                        return (path, nodesVisited, maxFrontier);
                    }

                    foreach (var direction in directions)
                    {
                        int newX = current.Item1 + direction.Item1;
                        int newY = current.Item2 + direction.Item2;
                        var neighbor = (newX, newY);

                        if (is_valid(newX, newY, grid) && !visited.Contains(neighbor))
                        {
                            queue.Enqueue(neighbor);
                            visited.Add(neighbor);
                            parents[neighbor] = current;
                        }
                    }
                }
            }

            stopwatch.Stop();
            PerformanceDataRepository.PerformanceData["BFS"].ExecutionTimes.Add(stopwatch.Elapsed.TotalSeconds);
            PerformanceDataRepository.PerformanceData["BFS"].NodesVisited.Add(nodesVisited);
            PerformanceDataRepository.PerformanceData["BFS"].MaxFrontier.Add(maxFrontier);
            PerformanceDataRepository.PerformanceData["BFS"].PathLengths.Add(0); // No path found
            return (null, nodesVisited, maxFrontier);
        }

        // DFS Implementation with Parent Pointers
        private (List<(int, int)> path, int nodesVisited, int maxFrontier) DFS((int, int) start, (int, int) goal)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Stack<(int, int)> stack = new Stack<(int, int)>();
            Dictionary<(int, int), (int, int)> parents = new Dictionary<(int, int), (int, int)>();
            HashSet<(int, int)> visited = new HashSet<(int, int)>();

            stack.Push(start);
            visited.Add(start);

            int nodesVisited = 0;
            int maxFrontier = 1;

            while (stack.Count > 0)
            {
                maxFrontier = Math.Max(maxFrontier, stack.Count);
                var current = stack.Pop();
                nodesVisited++;

                if (current == goal)
                {
                    stopwatch.Stop();
                    List<(int, int)> path = ReconstructPath(parents, start, goal);
                    PerformanceDataRepository.PerformanceData["DFS"].ExecutionTimes.Add(stopwatch.Elapsed.TotalSeconds);
                    PerformanceDataRepository.PerformanceData["DFS"].NodesVisited.Add(nodesVisited);
                    PerformanceDataRepository.PerformanceData["DFS"].MaxFrontier.Add(maxFrontier);
                    PerformanceDataRepository.PerformanceData["DFS"].PathLengths.Add(path.Count);
                    algorithmPaths["DFS"] = path;
                    return (path, nodesVisited, maxFrontier);
                }

                foreach (var direction in directions)
                {
                    int newX = current.Item1 + direction.Item1;
                    int newY = current.Item2 + direction.Item2;
                    var neighbor = (newX, newY);

                    if (is_valid(newX, newY, grid) && !visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                        visited.Add(neighbor);
                        parents[neighbor] = current;
                    }
                }
            }

            stopwatch.Stop();
            PerformanceDataRepository.PerformanceData["DFS"].ExecutionTimes.Add(stopwatch.Elapsed.TotalSeconds);
            PerformanceDataRepository.PerformanceData["DFS"].NodesVisited.Add(nodesVisited);
            PerformanceDataRepository.PerformanceData["DFS"].MaxFrontier.Add(maxFrontier);
            PerformanceDataRepository.PerformanceData["DFS"].PathLengths.Add(0); // No path found
            return (null, nodesVisited, maxFrontier);
        }

        // IDS Implementation with Depth-Limited Search using Stack and Parent Pointers
        private (List<(int, int)> path, int nodesVisited, int maxFrontier, int iterations) IDS((int, int) start, (int, int) goal)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int totalNodesVisited = 0;
            int overallMaxFrontier = 0;
            int iterations = 0;
            List<(int, int)> foundPath = null;

            for (int depthLimit = 0; depthLimit <= ROWS * COLS; depthLimit++)
            {
                iterations++;
                int nodesVisited = 0;
                int maxFrontier = 0;

                Stack<Node> stack = new();
                stack.Push(new Node { Position = start, Parent = null });
                HashSet<(int, int)> visited = new HashSet<(int, int)>();
                visited.Add(start);

                while (stack.Count > 0)
                {
                    maxFrontier = Math.Max(maxFrontier, stack.Count);
                    var currentNode = stack.Pop();
                    nodesVisited++;

                    if (currentNode.Position == goal)
                    {
                        // Reconstruct path
                        foundPath = ReconstructPath(currentNode);
                        stopwatch.Stop();
                        PerformanceDataRepository.PerformanceData["IDS"].ExecutionTimes.Add(stopwatch.Elapsed.TotalSeconds);
                        PerformanceDataRepository.PerformanceData["IDS"].NodesVisited.Add(totalNodesVisited + nodesVisited);
                        PerformanceDataRepository.PerformanceData["IDS"].MaxFrontier.Add(Math.Max(overallMaxFrontier, maxFrontier));
                        PerformanceDataRepository.PerformanceData["IDS"].PathLengths.Add(foundPath.Count);
                        PerformanceDataRepository.PerformanceData["IDS"].Iterations.Add(iterations);
                        algorithmPaths["IDS"] = foundPath;
                        return (foundPath, totalNodesVisited + nodesVisited, Math.Max(overallMaxFrontier, maxFrontier), iterations);
                    }

                    // Calculate current depth based on path length
                    int pathLength = 0;
                    Node temp = currentNode;
                    while (temp != null)
                    {
                        pathLength++;
                        temp = temp.Parent;
                    }

                    if (pathLength < depthLimit)
                    {
                        foreach (var direction in directions)
                        {
                            int newX = currentNode.Position.Item1 + direction.Item1;
                            int newY = currentNode.Position.Item2 + direction.Item2;
                            var neighbor = (newX, newY);

                            if (is_valid(newX, newY, grid) && !visited.Contains(neighbor))
                            {
                                stack.Push(new Node { Position = neighbor, Parent = currentNode });
                                visited.Add(neighbor);
                            }
                        }
                    }
                }

                totalNodesVisited += nodesVisited;
                overallMaxFrontier = Math.Max(overallMaxFrontier, maxFrontier);
            }

            stopwatch.Stop();
            PerformanceDataRepository.PerformanceData["IDS"].ExecutionTimes.Add(stopwatch.Elapsed.TotalSeconds);
            PerformanceDataRepository.PerformanceData["IDS"].NodesVisited.Add(totalNodesVisited);
            PerformanceDataRepository.PerformanceData["IDS"].MaxFrontier.Add(overallMaxFrontier);
            PerformanceDataRepository.PerformanceData["IDS"].PathLengths.Add(0); // No path found
            PerformanceDataRepository.PerformanceData["IDS"].Iterations.Add(iterations);
            return (null, totalNodesVisited, overallMaxFrontier, iterations);
        }

        // Helper Method to Check if a Position is Already in the Path
        private bool IsInPath(Node node, (int, int) position)
        {
            Node temp = node;
            while (temp != null)
            {
                if (temp.Position == position)
                    return true;
                temp = temp.Parent;
            }
            return false;
        }

        // Reconstruct Path from Parent Pointers
        private List<(int, int)> ReconstructPath(Dictionary<(int, int), (int, int)> parents, (int, int) start, (int, int) goal)
        {
            List<(int, int)> path = new List<(int, int)>();
            var current = goal;

            while (current != start)
            {
                path.Add(current);
                if (parents.ContainsKey(current))
                {
                    current = parents[current];
                }
                else
                {
                    // No path found
                    return null;
                }
            }
            path.Add(start);
            path.Reverse();
            return path;
        }

        // Reconstruct Path from a Node's Parent Pointers
        private List<(int, int)> ReconstructPath(Node node)
        {
            List<(int, int)> path = new List<(int, int)>();
            while (node != null)
            {
                path.Add(node.Position);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }

        // Run all search algorithms
        private void RunSearchAlgorithms()
        {
            // BFS
            var bfsResult = BFS(start, goal);
            if (bfsResult.path != null)
            {
                algorithmPaths["BFS"] = bfsResult.path;
            }

            // DFS
            var dfsResult = DFS(start, goal);
            if (dfsResult.path != null)
            {
                algorithmPaths["DFS"] = dfsResult.path;
            }

            // IDS
            var idsResult = IDS(start, goal);
            if (idsResult.path != null)
            {
                algorithmPaths["IDS"] = idsResult.path;
            }

            // Set currentPath based on currentMode
            if (algorithmPaths.ContainsKey(currentMode) && algorithmPaths[currentMode].Count > 0)
            {
                currentPath = algorithmPaths[currentMode];
            }
            else
            {
                currentPath = new List<(int, int)>();
            }

            DrawGrid();
            UpdateMetricsDisplay();
        }

        // Drawing the grid and path
        private void DrawGrid()
        {
            GridCanvas.Children.Clear();
            GridCanvas.Width = COLS * (CELL_SIZE + MARGIN) + MARGIN;
            GridCanvas.Height = ROWS * (CELL_SIZE + MARGIN) + MARGIN;

            // Set background color
            GridCanvas.Background = BackgroundColor;

            for (int x = 0; x < ROWS; x++)
            {
                for (int y = 0; y < COLS; y++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = CELL_SIZE,
                        Height = CELL_SIZE,
                        Fill = grid[x, y] == 1 ? WallColor : BackgroundColor,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };

                    Canvas.SetLeft(rect, y * (CELL_SIZE + MARGIN) + MARGIN);
                    Canvas.SetTop(rect, x * (CELL_SIZE + MARGIN) + MARGIN);
                    GridCanvas.Children.Add(rect);
                }
            }

            // Highlight start and goal
            DrawCell(start.Item1, start.Item2, StartColor);
            DrawCell(goal.Item1, goal.Item2, GoalColor);

            // Draw path based on current mode
            if (currentPath != null && currentPath.Count > 0)
            {
                foreach (var cell in currentPath)
                {
                    // Avoid coloring start and goal again
                    if (cell != start && cell != goal)
                    {
                        DrawCell(cell.Item1, cell.Item2, PathColor);
                    }
                }
            }
        }

        // Draw individual cell with specified color
        private void DrawCell(int x, int y, SolidColorBrush color)
        {
            Rectangle rect = new Rectangle
            {
                Width = CELL_SIZE,
                Height = CELL_SIZE,
                Fill = color,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(rect, y * (CELL_SIZE + MARGIN) + MARGIN);
            Canvas.SetTop(rect, x * (CELL_SIZE + MARGIN) + MARGIN);
            GridCanvas.Children.Add(rect);
        }

        // Button Click Events
        private void BFSButton_Click(object sender, RoutedEventArgs e)
        {
            currentMode = "BFS";
            CurrentModeText.Text = $"Current Mode: {currentMode}";

            if (PerformanceDataRepository.PerformanceData["BFS"].PathLengths.Count > 0 && PerformanceDataRepository.PerformanceData["BFS"].PathLengths.Last() > 0)
            {
                currentPath = algorithmPaths["BFS"];
            }
            else
            {
                currentPath = new List<(int, int)>();
            }

            DrawGrid();
            UpdateMetricsDisplay();
        }

        private void DFSButton_Click(object sender, RoutedEventArgs e)
        {
            currentMode = "DFS";
            CurrentModeText.Text = $"Current Mode: {currentMode}";

            if (PerformanceDataRepository.PerformanceData["DFS"].PathLengths.Count > 0 && PerformanceDataRepository.PerformanceData["DFS"].PathLengths.Last() > 0)
            {
                currentPath = algorithmPaths["DFS"];
            }
            else
            {
                currentPath = new List<(int, int)>();
            }

            DrawGrid();
            UpdateMetricsDisplay();
        }

        private void IDSButton_Click(object sender, RoutedEventArgs e)
        {
            currentMode = "IDS";
            CurrentModeText.Text = $"Current Mode: {currentMode}";

            if (PerformanceDataRepository.PerformanceData["IDS"].PathLengths.Count > 0 && PerformanceDataRepository.PerformanceData["IDS"].PathLengths.Last() > 0)
            {
                currentPath = algorithmPaths["IDS"];
            }
            else
            {
                currentPath = new List<(int, int)>();
            }

            DrawGrid();
            UpdateMetricsDisplay();
        }

        private void RegenerateButton_Click(object sender, RoutedEventArgs e)
        {
            double wallProbability = WallProbabilitySlider.Value;
            GenerateGrid(wallProbability);
            RunSearchAlgorithms();
        }

        private void ShowResultsButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the Results Page
            Results resultsWindow = new();
            resultsWindow.Show();
            this.Hide();
            resultsWindow.Closed += (s, args) => this.Show();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Update Metrics Display in UI
        private void UpdateMetricsDisplay()
        {
            MetricsText.Text = "";

            if (!PerformanceDataRepository.PerformanceData.ContainsKey(currentMode))
            {
                return;
            }

            var metrics = PerformanceDataRepository.PerformanceData[currentMode];
            double avgTime = metrics.ExecutionTimes.Count > 0 ? metrics.ExecutionTimes.Average() : 0;
            double avgNodes = metrics.NodesVisited.Count > 0 ? metrics.NodesVisited.Average() : 0;
            double avgFrontier = metrics.MaxFrontier.Count > 0 ? metrics.MaxFrontier.Average() : 0;
            double avgPathLength = metrics.PathLengths.Count > 0 ? metrics.PathLengths.Average() : 0;

            string metricsInfo = $"{currentMode} Metrics:\n" +
                                 $"  Avg Time: {avgTime:F4}s\n" +
                                 $"  Avg Nodes Visited: {avgNodes:F1}\n" +
                                 $"  Avg Max Frontier: {avgFrontier:F1}\n" +
                                 $"  Avg Path Length: {avgPathLength:F1}";

            if (currentMode == "IDS")
            {
                double avgIterations = metrics.Iterations.Count > 0 ? metrics.Iterations.Average() : 0;
                metricsInfo += $"\n  Avg Iterations: {avgIterations:F1}";
            }

            MetricsText.Text = metricsInfo;
        }


    }

    // Define a class to hold performance metrics
    public class PerformanceMetrics
    {
        public List<double> ExecutionTimes { get; set; } = new List<double>();
        public List<int> NodesVisited { get; set; } = new List<int>();
        public List<int> MaxFrontier { get; set; } = new List<int>();
        public List<int> PathLengths { get; set; } = new List<int>();
        public List<int> Iterations { get; set; } = new List<int>(); // For IDS
    }

    // Define a class to represent a node with parent pointers
    public class Node
    {
        public (int, int) Position { get; set; }
        public Node Parent { get; set; }
    }

    
}
