using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCell
{
    public int y;
    public int x;
    public bool top;
    public bool right;
    public bool bottom;
    public bool left;
    

    // Constructor to initialize a map cell with all walls initially present or absent
    public MapCell(int y, int x, bool top, bool right, bool bottom, bool left)
    {
        this.y = y;
        this.x = x;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
        this.left = left;
    }
}

public static class MapGenerator
{
    public static void GenerateMap(GameObject wallPrefab)
    {
        int rows = 4;
        int cols = 8;

        MapCell[,] map = new MapCell[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
                map[row, col] = new MapCell(row, col, true, true, true, true);
        }

        // DFS
        List<MapCell> currentPath = new List<MapCell>();
        List<MapCell> completedNodes = new List<MapCell>();

        currentPath.Add(map[Random.Range(0, rows), Random.Range(0, cols)]);

        while (completedNodes.Count < rows * cols)
        {
            List<MapCell> possibleNodes = new List<MapCell>();
            List<string> possibleDirections = new List<string>();

            MapCell currentNode = currentPath[currentPath.Count - 1];

            if (currentNode.y > 0)
            {
                MapCell neighbour = map[currentNode.y - 1, currentNode.x];
                if (!completedNodes.Contains(neighbour) && !currentPath.Contains(neighbour))
                {
                    possibleDirections.Add("up");
                    possibleNodes.Add(neighbour);
                }
            }
            if (currentNode.y < rows - 1)
            {
                MapCell neighbour = map[currentNode.y + 1, currentNode.x];
                if (!completedNodes.Contains(neighbour) && !currentPath.Contains(neighbour))
                {
                    possibleDirections.Add("down");
                    possibleNodes.Add(neighbour);
                }
            }
            if (currentNode.x > 0)
            {
                MapCell neighbour = map[currentNode.y, currentNode.x - 1];
                if (!completedNodes.Contains(neighbour) && !currentPath.Contains(neighbour))
                {
                    possibleDirections.Add("left");
                    possibleNodes.Add(neighbour);
                }
            }
            if (currentNode.x < cols - 1)
            {
                MapCell neighbour = map[currentNode.y, currentNode.x + 1];
                if (!completedNodes.Contains(neighbour) && !currentPath.Contains(neighbour))
                {
                    possibleDirections.Add("right");
                    possibleNodes.Add(neighbour);
                }
            }

            if (possibleDirections.Count > 0)
            {
                int direction = Random.Range(0, possibleDirections.Count);
                MapCell chosenNode = possibleNodes[direction];

                switch (possibleDirections[direction])
                {
                    case "up":
                        currentNode.top = false;
                        chosenNode.bottom = false;
                        break;
                    case "down":
                        currentNode.bottom = false;
                        chosenNode.top = false;
                        break;
                    case "left":
                        currentNode.left = false;
                        chosenNode.right = false;
                        break;
                    case "right":
                        currentNode.right = false;
                        chosenNode.left = false;
                        break;
                    default:
                        break;
                }

                currentPath.Add(chosenNode);
            }
            else
            {
                completedNodes.Add(currentNode);
                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }

        // Remove some more walls randomly
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (row > 0 && map[row, col].top && Random.Range(0, 10) == 0)
                {
                    map[row, col].top = false;
                    map[row - 1, col].bottom = false;
                }
                if (row < rows - 1 && map[row, col].bottom && Random.Range(0, 10) == 0)
                {
                    map[row, col].bottom = false;
                    map[row + 1, col].top = false;
                }
                if (col > 0 && map[row, col].left && Random.Range(0, 10) == 0)
                {
                    map[row, col].left = false;
                    map[row, col - 1].right = false;
                }
                if (col < cols - 1 && map[row, col].right && Random.Range(0, 10) == 0)
                {
                    map[row, col].right = false;
                    map[row, col + 1].left = false;
                }
            }
        }

        // Generate wall objects
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (col < cols - 1 && map[row, col].right)
                {
                    GameObject wall = UnityEngine.Object.Instantiate(wallPrefab, new Vector3(ConvertX(col + 1), ConvertY(row) - 1f, 0f), Quaternion.identity);
                    wall.transform.localScale = new Vector3(0.1f, 2.1f, 0.2f);
                }
                if (row < rows - 1 && map[row, col].bottom)
                {
                    GameObject wall = UnityEngine.Object.Instantiate(wallPrefab, new Vector3(ConvertX(col) + 1f, ConvertY(row + 1), 0f), Quaternion.identity);
                    wall.transform.localScale = new Vector3(2.1f, 0.1f, 0.2f);
                }
            }
        }
    }


    public static void RegenerateMap(GameObject wallPrefab)
    {
        // Find all GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        // Iterate through all found GameObjects
        foreach (GameObject obj in allObjects)
        {
            // Check if the object's name is "Wall(Clone)"
            if (obj.name == "Wall(Clone)")
            {
                // Destroy the object
                UnityEngine.Object.Destroy(obj);
            }
        }

        GenerateMap(wallPrefab);
    }

    public static void MakeBreakableWall(GameObject breakableWallPrefab)
    {
        // Find all GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        List<GameObject> wallObjects = new List<GameObject>();

        // Iterate through all found GameObjects
        foreach (GameObject obj in allObjects)
        {
            // Check if the object's name is "Wall(Clone)"
            if (obj.name == "Wall(Clone)")
            {
                // Append to list of wall objects
                wallObjects.Add(obj);
            }
        }

        if (wallObjects.Count == 0)
        {
            return;
        }

        GameObject selectedWall = wallObjects[Random.Range(0, wallObjects.Count)];

        GameObject breakableWall = GameObject.Instantiate(breakableWallPrefab, selectedWall.transform.position, Quaternion.identity);
        breakableWall.transform.localScale = selectedWall.transform.localScale;
        breakableWall.tag = "BreakableWall";

        UnityEngine.Object.Destroy(selectedWall);
    }

    public static float ConvertX(float x)
    {
        return x * 2 - 8f;
    }

    public static float ConvertY(float input)
    {
        // Define the range and corresponding values
        float minInput = 0f;
        float maxInput = 4f;
        float minValue = 3.5f;
        float maxValue = -4.5f;

        // Clamp the input to the range
        input = Mathf.Clamp(input, minInput, maxInput);

        // Calculate the interpolation factor
        float t = (input - minInput) / (maxInput - minInput);

        // Perform linear interpolation between minValue and maxValue
        return Mathf.Lerp(minValue, maxValue, t);
    }
}
