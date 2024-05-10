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

        // Nastavim zacetni rezultat
        P1ScoreText.text = "P1: " + GameState.P1Score;
        P2ScoreText.text = "P2: " + GameState.P2Score;

        // Zgeneriram mapo
        MapGenerator.GenerateMap(wallPrefab);
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

    Vector3 GenerateSpawnLocation(int player)
    {
        float x, y;

        // mapa je 8x4, torej ConvertY prejme 0-4, ConvertX pa 0-8
        if (player == 1)
            x = MapGenerator.ConvertX(Random.Range(0, 3));
        else
            x = MapGenerator.ConvertX(Random.Range(5, 8));

        y = MapGenerator.ConvertY(Random.Range(0, 4));

        // x+1f in y-1f, da premaknem iz zgornje levega kota celice v sredino celice
        return new Vector3(x+1f, y-1f, 0f);
    }
}
