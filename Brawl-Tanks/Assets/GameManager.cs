using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    public TextMeshProUGUI MapResetTimer;

    public GameObject player1;
    public GameObject player2;
    public GameObject wallPrefab;
    public GameObject breakableWallPrefab;
    public GameObject pauseMenuCanvas;

    AbilitiesSpawning abilitiesSpawning;
    private GridUpdater gridUpdater;

    public int currentModifier;
    int resetTimer = 10;
    int breakableWallTimer = 3;
    bool setText = false;

    public bool isPaused = false;

    //SURVIVAL ONLY VARIABLES
    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hpText;

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
        Time.timeScale = 1;

        if (SceneManager.GetActiveScene().name == "DeathMatch")
        {
            abilitiesSpawning = GameObject.Find("GameManager").GetComponent<AbilitiesSpawning>();
            goalText.text = "First to: " + GameState.goal.ToString();
            //gamemodeText.text = GameState.gamemode;

            var p1 = Instantiate(player1, GenerateSpawnLocation(1), Quaternion.identity);
            p1.name = "P1_Tank";
            var p2 = Instantiate(player2, GenerateSpawnLocation(2), Quaternion.identity);
            p2.name = "P2_Tank";

            // Nastavim zacetni rezultat
            P1ScoreText.text = GameState.P1Score.ToString();
            P2ScoreText.text = GameState.P2Score.ToString();

            PlayerPrefs.SetInt("DestroyedPlayerID", 0);

            // To je grid za AI
            gridUpdater = FindFirstObjectByType<GridUpdater>();
            // Zgeneriram mapo
            MapGenerator.GenerateMap(wallPrefab);
            //updejtam grid za AI po ustvaritvi mape
            gridUpdater.UpdateGrid();


            currentModifier = Random.Range(0, 7);

            Debug.Log("current modifier " + currentModifier);
            if (currentModifier == 0)
            {
                MapResetTimer.text = "Map reset in 10s";
                StartCoroutine(RegenerateMapPeriodically());
            }
            else if (currentModifier == 1)
            {
                MapResetTimer.text = "Super speed!";
            }
            else if (currentModifier == 2)
            {
                MapResetTimer.text = "Power Madness!";
            }
            else if (currentModifier == 3)
            {
                MapResetTimer.text = "Inverted controls!";
            }
            else if (currentModifier == 4)
            {
                MapResetTimer.text = "Only power: ";
            }
            else if (currentModifier == 5)
            {
                MapResetTimer.text = "Breakable walls!";
                StartCoroutine(MakeBreakableWallsPeriodically());
            }
            else if (currentModifier == 6)
            {
                MapResetTimer.text = "Empty Map!";
                MapGenerator.DestroyMap();
                gridUpdater.UpdateGrid();
            }
            else
            {
                MapResetTimer.text = "";
            }
        }
        else
        {
            GameState.survivalScore = 0;
            var p1 = Instantiate(player1, GenerateSpawnLocation(1), Quaternion.identity);
            p1.name = "P1_Tank";
            GameState.playerHP = 20;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "DeathMatch")
        {
            if (player1Destroyed || player2Destroyed)
            {
                StartCoroutine(LoadNextScene());
                // Trigger game over
                //SceneManager.LoadScene("GameOver");
                P1ScoreText.text = GameState.P1Score.ToString();
                P2ScoreText.text = GameState.P2Score.ToString();
            }
            if (currentModifier == 4 && !setText)
            {
                if (abilitiesSpawning.chosenAbility == 0)
                {
                    MapResetTimer.text = "Only power: AI Rocket";
                }
                else if (abilitiesSpawning.chosenAbility == 1)
                {
                    MapResetTimer.text = "Only power: DeathRay";
                }
                else if (abilitiesSpawning.chosenAbility == 2)
                {
                    MapResetTimer.text = "Only power: FragBomb";
                }
                else if (abilitiesSpawning.chosenAbility == 3)
                {
                    MapResetTimer.text = "Only power: Gatling gun";
                }
                else if (abilitiesSpawning.chosenAbility == 4)
                {
                    MapResetTimer.text = "Only power: RC Missile";
                }
                else if (abilitiesSpawning.chosenAbility == 5)
                {
                    MapResetTimer.text = "Only power: Shield";
                }
                setText = true;
            }
        }
        else
        {
            scoreText.text = "Score: " + GameState.survivalScore;
            hpText.text = "HP: " + GameState.playerHP;
            if (GameState.playerHP <= 0){
                SceneManager.LoadScene("GameOverSingleplayer");
            }
        }
    }
    // Coroutine to load the next scene
    IEnumerator LoadNextScene()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(2);
        // Load the next scene
        SceneManager.LoadScene("GameOver");
    }

    IEnumerator RegenerateMapPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (resetTimer == 1)
            {
                MapGenerator.RegenerateMap(wallPrefab);
                gridUpdater.UpdateGrid();
                resetTimer = 11;
            }

            resetTimer--;
            MapResetTimer.text = "Map reset in " + resetTimer + "s";
        }
    }

    IEnumerator MakeBreakableWallsPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (breakableWallTimer == 1)
            {
                MapGenerator.MakeBreakableWall(breakableWallPrefab);
                breakableWallTimer = 4;
            }

            breakableWallTimer--;
        }
    }

    // Method to call when a player is destroyed
    public void PlayerDestroyed(PlayerID playerID)
    {
        if (SceneManager.GetActiveScene().name == "DeathMatch")
        {
            int destroyedPlayerID = PlayerPrefs.GetInt("DestroyedPlayerID");

            // Set the corresponding player destroyed state to true
            if (playerID == PlayerID.Player1)
            {
                GameState.P2Score++;
                player1Destroyed = true;

                if (destroyedPlayerID == 2)
                    PlayerPrefs.SetInt("DestroyedPlayerID", 3); // Draw
                else
                    PlayerPrefs.SetInt("DestroyedPlayerID", 1); // Player 1 was destroyed
            }
            else if (playerID == PlayerID.Player2)
            {
                GameState.P1Score++;
                player2Destroyed = true;

                if (destroyedPlayerID == 1)
                    PlayerPrefs.SetInt("DestroyedPlayerID", 3); // Draw
                else
                    PlayerPrefs.SetInt("DestroyedPlayerID", 2); // Player 2 was destroyed
            }
        }else{

        }
    }

    public Vector3 GenerateSpawnLocation(int player)
    {
        if (SceneManager.GetActiveScene().name == "DeathMatch")
        {
            float x, y;

            // mapa je 8x4, torej ConvertY prejme 0-4, ConvertX pa 0-8
            if (player == 1)
                x = MapGenerator.ConvertX(Random.Range(0, 3));
            else if (player == 2)
                x = MapGenerator.ConvertX(Random.Range(5, 8));
            else
                x = MapGenerator.ConvertX(Random.Range(0, 8));

            y = MapGenerator.ConvertY(Random.Range(0, 4));

            // x+1f in y-1f, da premaknem iz zgornje levega kota celice v sredino celice
            return new Vector3(x + 1f, y - 1f, 0f);
        }
        else{
            return new Vector3(0, 0, 0);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            pauseMenuCanvas.SetActive(true);
        } else
        {
            Time.timeScale = 1;
            pauseMenuCanvas.SetActive(false);
        }
    }

    public void ExitGame()
    {
        GameState.P1Score = 0;
        GameState.P2Score = 0;
        SceneManager.LoadScene("MainMenu");
    }
}
