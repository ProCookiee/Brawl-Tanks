using UnityEngine;
using Pathfinding;
using System;

public class GridUpdater : MonoBehaviour
{
    // Reference to the A* grid graph
    //public GridGraph gridGraph;
    public GridGraph gridGraph;

    void Start()
    {
        // Get the A* grid graph
        gridGraph = AstarPath.active.data.gridGraph;
    }

    public void UpdateGrid()
    {
        gridGraph = AstarPath.active.data.gridGraph;
        // Update the entire grid
        AstarPath.active.Scan(gridGraph);
        Debug.Log("Grid updated");
    }
}