using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridSearchVisualization
{
    public static class PerformanceDataRepository
    {
        public static Dictionary<string, PerformanceMetrics> PerformanceData { get; set; } = new Dictionary<string, PerformanceMetrics>()
        {
            { "BFS", new PerformanceMetrics() },
            { "DFS", new PerformanceMetrics() },
            { "IDS", new PerformanceMetrics() }
        };
        
        // Optionally, you can add methods to reset or manipulate the data if needed
        public static void Reset()
        {
            foreach (var metrics in PerformanceData.Values)
            {
                metrics.ExecutionTimes.Clear();
                metrics.NodesVisited.Clear();
                metrics.MaxFrontier.Clear();
                metrics.PathLengths.Clear();
                metrics.Iterations.Clear();
            }
        }
    }
}
