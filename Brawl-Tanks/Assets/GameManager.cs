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

        Instantiate(player1, new Vector3(-4, 0, 0), Quaternion.identity);
        Instantiate(player2, new Vector3(4, 0, 0), Quaternion.identity);

        //Nastavim zacetni rezultat
        P1ScoreText.text = "P1: " + GameState.P1Score;
        P2ScoreText.text = "P2: " + GameState.P2Score;

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
}
