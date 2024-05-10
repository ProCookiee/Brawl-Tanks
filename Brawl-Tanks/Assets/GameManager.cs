using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public enum NodeState
{
    Available,
    Current,
    Completed
}

public class MapCell
{
    public NodeState state;
    public int y;
    public int x;
    public bool top;
    public bool right;
    public bool bottom;
    public bool left;

    // Constructor to initialize a map cell with all walls initially present or absent
    public MapCell(int y, int x,  bool top, bool right, bool bottom, bool left)
    {
        this.y = y;
        this.x = x;
        this.state = NodeState.Available;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
        this.left = left;
    }
}

public class GameManager : MonoBehaviour
{
    // Singleton instance of the GameManager
    public static GameManager instance;
    // IDs for each player
    public enum PlayerID { Player1, Player2 };

    // Track the state of each player
    private bool player1Destroyed = false;
    private bool player2Destroyed = false;

    public TextMeshProUGUI goalText;
    //public TextMeshProUGUI gamemodeText;
    public TextMeshProUGUI P1ScoreText;
    public TextMeshProUGUI P2ScoreText;

    public GameObject player1;
    public GameObject player2;
    public GameObject wallPrefab;

    // Ensure only one instance of GameManager exists
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
        goalText.text = "First to: " + GameState.goal.ToString();
        //gamemodeText.text = GameState.gamemode;

        Instantiate(player1, GenerateSpawnLocation(1), Quaternion.identity);
        Instantiate(player2, GenerateSpawnLocation(2), Quaternion.identity);

        //Nastavim zacetni rezultat
        P1ScoreText.text = "P1: " + GameState.P1Score;
        P2ScoreText.text = "P2: " + GameState.P2Score;

        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (player1Destroyed || player2Destroyed)
        {
            // Trigger game over
            SceneManager.LoadScene("GameOver");
            P1ScoreText.text = "P1: " + GameState.P1Score;
            P2ScoreText.text = "P2: " + GameState.P2Score;
        }
    }
    // Method to call when a player is destroyed
    public void PlayerDestroyed(PlayerID playerID)
    {
        // Set the corresponding player destroyed state to true
        if (playerID == PlayerID.Player1)
        {
            GameState.P2Score++;
            player1Destroyed = true;
            PlayerPrefs.SetInt("DestroyedPlayerID", 1); // Player 1 was destroyed
        }
        else if (playerID == PlayerID.Player2)
        {
            GameState.P1Score++;
            player2Destroyed = true;
            PlayerPrefs.SetInt("DestroyedPlayerID", 2); // Player 2 was destroyed
        }
    }

    void GenerateMap()
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
        currentPath[0].state = NodeState.Current;
        
        while (completedNodes.Count < rows*cols)
        {
            List<MapCell> possibleNodes = new List<MapCell>(); 
            List<string> possibleDirections = new List<string>();

            MapCell currentNode = currentPath[currentPath.Count - 1];

            if (currentNode.y > 0)
            {
                MapCell neighbour = map[currentNode.y - 1, currentNode.x];
                if (!completedNodes.Contains(neighbour) && !currentPath.Contains(neighbour)) {
                    possibleDirections.Add("up");
                    possibleNodes.Add(neighbour);
                }
            }
            if (currentNode.y < rows - 1)
            {
                MapCell neighbour = map[currentNode.y + 1, currentNode.x];
                if (!completedNodes.Contains(neighbour) && !currentPath.Contains(neighbour)) {
                    possibleDirections.Add("down");
                    possibleNodes.Add(neighbour);
                }
            }
            if (currentNode.x > 0)
            {
                MapCell neighbour = map[currentNode.y, currentNode.x - 1];
                if (!completedNodes.Contains(neighbour) && !currentPath.Contains(neighbour)) {
                    possibleDirections.Add("left");
                    possibleNodes.Add(neighbour);
                }
            }
            if (currentNode.x < cols - 1)
            {
                MapCell neighbour = map[currentNode.y, currentNode.x + 1];
                if (!completedNodes.Contains(neighbour) && !currentPath.Contains(neighbour)) {
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
                chosenNode.state = NodeState.Current;
            } 
            else
            {
                completedNodes.Add(currentNode);
                currentNode.state = NodeState.Completed;
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
                    GameObject wall = Instantiate(wallPrefab, new Vector3(ConvertX(col+1), ConvertY(row) - 1f, 0f), Quaternion.identity);
                    wall.transform.localScale = new Vector3(0.1f, 2.1f, 0.2f);
                }
                if (row < rows - 1 && map[row, col].bottom)
                {
                    GameObject wall = Instantiate(wallPrefab, new Vector3(ConvertX(col) + 1f, ConvertY(row+1), 0f), Quaternion.identity);
                    wall.transform.localScale = new Vector3(2.1f, 0.1f, 0.2f);
                }
            }
        }
    }

    Vector3 GenerateSpawnLocation(int player)
    {
        float x, y;

        if (player == 1)
            x = ConvertX(Random.Range(0, 3));
        else
            x = ConvertX(Random.Range(5, 8));

        y = ConvertY(Random.Range(0, 4));

        return new Vector3(x+1f, y-1f, 0f);
    }

    float ConvertX(float x)
    {
        return x*2 - 8f;
    }

    float ConvertY(float input)
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
