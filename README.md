# Grid Search Visualization

## Overview
The Grid Search Visualization project is a sophisticated WPF-based application designed to facilitate an interactive exploration of pathfinding algorithms. This platform supports the visualization of foundational search algorithms, including Breadth-First Search (BFS), Depth-First Search (DFS), Iterative Deepening Search (IDS), and A* (using Manhattan and Euclidean heuristics). Moreover, it incorporates robust metrics visualization tools to evaluate algorithmic performance through parameters such as runtime, nodes visited, maximum frontier size, and path length.

## Features
- **Interactive Algorithm Demonstrations**: Visualize algorithm execution dynamically.
- **Comprehensive Algorithm Support**:
  - Breadth-First Search (BFS)
  - Depth-First Search (DFS)
  - Iterative Deepening Search (IDS)
  - A* (Manhattan heuristic)
  - A* (Euclidean heuristic)
- **Detailed Performance Metrics**:
  - Execution Time
  - Nodes Explored
  - Maximum Frontier Size
  - Path Length
  - Iterations (specific to IDS)
- **Customizable Grid Environment**:
  - Configurable wall density via an adjustable slider.
  - Dynamic grid regeneration.
- **Rich Visualization Output**: Metrics comparisons are displayed using detailed, interactive charts.

## Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/MrStranger812/GridSearchVisualization.git
   ```
2. Open the project in Visual Studio.
3. Restore required NuGet packages if prompted.
4. Build and execute the application.

## Usage Instructions
1. **Launching the Application**: Open the application to load the grid-based interface.
2. **Selecting an Algorithm**: Utilize the control panel to select among BFS, DFS, IDS, or A* with either Manhattan or Euclidean heuristics.
3. **Adjusting Wall Density**: Modify the wall probability slider to alter grid complexity.
4. **Regenerating the Grid**: Click "Regenerate" to produce a new randomized grid configuration.
5. **Viewing Results**: Access the "Show Results" button for a comprehensive performance analysis window.
6. **Exiting**: Click "Quit" to terminate the application.

## Metrics Visualization
### Metrics Dashboard
The application includes a results interface offering:
- **Aggregate Metrics**: Analyze average metrics (e.g., execution time, nodes explored, maximum frontier size, and path length) across all algorithms.
- **Run-Specific Metrics**: Examine detailed charts capturing execution statistics for individual algorithm runs.
- **IDS-Specific Iteration Analysis**: Visualize the number of iterations performed during IDS executions.

## Project Architecture
- **MainWindow.xaml & MainWindow.xaml.cs**: Houses the primary grid visualization interface.
- **Results.xaml & Results.xaml.cs**: Manages the results visualization and performance metrics dashboard.
- **PerformanceDataRepository.cs**: Central repository for aggregating performance metrics.
- **Node.cs**: Encapsulates data structures for search algorithms, including parent pointers for path reconstruction.

## Core Technologies
- **Windows Presentation Foundation (WPF)**: Powers the graphical user interface.
- **C#**: Serves as the primary programming language for algorithm implementation.
- **LiveCharts**: Provides dynamic, visually engaging charting capabilities for performance metrics.

## Potential Future Enhancements
- Incorporate additional algorithms, such as Dijkstra's Algorithm or Greedy Best-First Search.
- Improve visual aesthetics with step-by-step algorithm animations.
- Enable manual grid customization, allowing users to define walls and start/goal points.

## Contributing
We welcome contributions from the community! To contribute:
1. Fork the repository.
2. Develop your feature or bug fix.
3. Submit a pull request for review.

## Screenshots
![Screenshot 2024-12-27 123703](https://github.com/user-attachments/assets/d9739076-6552-4990-9e7d-bd67d81628da)
![Screenshot 2024-12-27 123730](https://github.com/user-attachments/assets/e0adcdf1-c821-4cb1-9e5b-ab4a42c0dbe5)
![Screenshot 2024-12-27 123744](https://github.com/user-attachments/assets/2f6bf4f0-2c61-4d9e-a05e-fc6d158732cd)
## Contact
For inquiries or feedback, please contact us at [sinakhabaz79@gmail.com] or open an issue on the GitHub repository.

