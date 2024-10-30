using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GridSearchVisualization
{
    public partial class Results : Window
    {
        // Labels for the algorithms and runs
        public List<string> AlgorithmLabels { get; set; } = new List<string> { "BFS", "DFS", "IDS" };
        public List<string> RunLabels { get; set; } = new List<string>();

        public Results()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadAverageMetrics();
            LoadDetailedMetrics();
        }

        /// <summary>
        /// Event handler for the Back Button.
        /// Closes the Results window and returns to the main grid.
        /// </summary>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Loads and displays the average performance metrics for each algorithm.
        /// </summary>
        private void LoadAverageMetrics()
        {
            var algorithms = PerformanceDataRepository.PerformanceData.Keys.ToList();

            // Calculate averages
            var avgExecutionTimes = algorithms.Select(algo =>
                PerformanceDataRepository.PerformanceData[algo].ExecutionTimes.Count > 0
                ? PerformanceDataRepository.PerformanceData[algo].ExecutionTimes.Average()
                : 0
            ).ToList();

            var avgNodesVisited = algorithms.Select(algo =>
                PerformanceDataRepository.PerformanceData[algo].NodesVisited.Count > 0
                ? PerformanceDataRepository.PerformanceData[algo].NodesVisited.Average()
                : 0
            ).ToList();

            var avgMaxFrontier = algorithms.Select(algo =>
                PerformanceDataRepository.PerformanceData[algo].MaxFrontier.Count > 0
                ? PerformanceDataRepository.PerformanceData[algo].MaxFrontier.Average()
                : 0
            ).ToList();

            var avgPathLengths = algorithms.Select(algo =>
                PerformanceDataRepository.PerformanceData[algo].PathLengths.Count > 0
                ? PerformanceDataRepository.PerformanceData[algo].PathLengths.Average()
                : 0
            ).ToList();

            // Create series for each metric
            var executionTimeSeries = new ColumnSeries
            {
                Title = "Exec Time (s)",
                Values = new ChartValues<double>(avgExecutionTimes),
                Fill = System.Windows.Media.Brushes.Blue
            };

            var nodesVisitedSeries = new ColumnSeries
            {
                Title = "Nodes Visited",
                Values = new ChartValues<double>(avgNodesVisited),
                Fill = System.Windows.Media.Brushes.Green
            };

            var maxFrontierSeries = new ColumnSeries
            {
                Title = "Max Frontier",
                Values = new ChartValues<double>(avgMaxFrontier),
                Fill = System.Windows.Media.Brushes.Orange
            };

            var pathLengthSeries = new ColumnSeries
            {
                Title = "Path Length",
                Values = new ChartValues<double>(avgPathLengths),
                Fill = System.Windows.Media.Brushes.Purple
            };

            // Add series to the chart
            AverageMetricsChart.Series = new SeriesCollection
            {
                executionTimeSeries,
                nodesVisitedSeries,
                maxFrontierSeries,
                pathLengthSeries
            };
        }

        /// <summary>
        /// Loads and displays detailed performance metrics for each run.
        /// Each run corresponds to a grid generation with a unique wall configuration.
        /// </summary>
        private void LoadDetailedMetrics()
        {
            // Determine the number of runs based on the length of ExecutionTimes for BFS
            // Assuming all algorithms have the same number of runs
            int numberOfRuns = PerformanceDataRepository.PerformanceData["BFS"].ExecutionTimes.Count;

            // Populate RunLabels
            RunLabels = Enumerable.Range(1, numberOfRuns).Select(i => $"Run {i}").ToList();

            // Execution Time Chart
            ExecutionTimeChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "BFS",
                    Values = new ChartValues<double>(PerformanceDataRepository.PerformanceData["BFS"].ExecutionTimes),
                    Stroke = System.Windows.Media.Brushes.Blue,
                    Fill = System.Windows.Media.Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "DFS",
                    Values = new ChartValues<double>(PerformanceDataRepository.PerformanceData["DFS"].ExecutionTimes),
                    Stroke = System.Windows.Media.Brushes.Green,
                    Fill = System.Windows.Media.Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "IDS",
                    Values = new ChartValues<double>(PerformanceDataRepository.PerformanceData["IDS"].ExecutionTimes),
                    Stroke = System.Windows.Media.Brushes.Orange,
                    Fill = System.Windows.Media.Brushes.Transparent
                }
            };

            // Nodes Visited Chart
            NodesVisitedChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "BFS",
                    Values = new ChartValues<int>(PerformanceDataRepository.PerformanceData["BFS"].NodesVisited),
                    Stroke = System.Windows.Media.Brushes.Blue,
                    Fill = System.Windows.Media.Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "DFS",
                    Values = new ChartValues<int>(PerformanceDataRepository.PerformanceData["DFS"].NodesVisited),
                    Stroke = System.Windows.Media.Brushes.Green,
                    Fill = System.Windows.Media.Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "IDS",
                    Values = new ChartValues<int>(PerformanceDataRepository.PerformanceData["IDS"].NodesVisited),
                    Stroke = System.Windows.Media.Brushes.Orange,
                    Fill = System.Windows.Media.Brushes.Transparent
                }
            };

            // Max Frontier Chart
            MaxFrontierChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "BFS",
                    Values = new ChartValues<int>(PerformanceDataRepository.PerformanceData["BFS"].MaxFrontier),
                    Stroke = System.Windows.Media.Brushes.Blue,
                    Fill = System.Windows.Media.Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "DFS",
                    Values = new ChartValues<int>(PerformanceDataRepository.PerformanceData["DFS"].MaxFrontier),
                    Stroke = System.Windows.Media.Brushes.Green,
                    Fill = System.Windows.Media.Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "IDS",
                    Values = new ChartValues<int>(PerformanceDataRepository.PerformanceData["IDS"].MaxFrontier),
                    Stroke = System.Windows.Media.Brushes.Orange,
                    Fill = System.Windows.Media.Brushes.Transparent
                }
            };

            // Path Length Chart
            PathLengthChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "BFS",
                    Values = new ChartValues<int>(PerformanceDataRepository.PerformanceData["BFS"].PathLengths),
                    Stroke = System.Windows.Media.Brushes.Blue,
                    Fill = System.Windows.Media.Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "DFS",
                    Values = new ChartValues<int>(PerformanceDataRepository.PerformanceData["DFS"].PathLengths),
                    Stroke = System.Windows.Media.Brushes.Green,
                    Fill = System.Windows.Media.Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "IDS",
                    Values = new ChartValues<int>(PerformanceDataRepository.PerformanceData["IDS"].PathLengths),
                    Stroke = System.Windows.Media.Brushes.Orange,
                    Fill = System.Windows.Media.Brushes.Transparent
                }
            };

            // Iterations Chart (Only for IDS)
            IterationsChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "IDS Iterations",
                    Values = new ChartValues<int>(PerformanceDataRepository.PerformanceData["IDS"].Iterations),
                    Stroke = System.Windows.Media.Brushes.Orange,
                    Fill = System.Windows.Media.Brushes.Transparent
                }
            };

            // Update the Axis Labels
            ExecutionTimeChart.AxisX[0].Labels = RunLabels;
            NodesVisitedChart.AxisX[0].Labels = RunLabels;
            MaxFrontierChart.AxisX[0].Labels = RunLabels;
            PathLengthChart.AxisX[0].Labels = RunLabels;
            IterationsChart.AxisX[0].Labels = RunLabels;
        }
    }
}
