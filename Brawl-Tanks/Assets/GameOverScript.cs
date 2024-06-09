using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GameOverScript : MonoBehaviour
{
     // Reference to the TextMeshProUGUI component
    public TextMeshProUGUI playerDestroyedText;
    // Variable to store the retrieved player ID
    private string mainMenuSceneName = "MainMenu";
    private string gameSceneName = "DeathMatch";
    private bool GameOver = false;
    public Button mainMenuButton;
    public GameObject mainMenuButtonObject;
    private GridUpdater gridUpdater;
    public void RestartGame()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    IEnumerator LoadMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Load the game scene
        SceneManager.LoadScene(gameSceneName);
    }


    // Start is called before the first frame update
    void Start()
    {
        mainMenuButtonObject.SetActive(false);
        // Retrieve the player ID from PlayerPrefs
        int destroyedPlayerID = PlayerPrefs.GetInt("DestroyedPlayerID");
        // Set the text of the UI Text component to display the destroyed player's ID
        if(GameState.P1Score == GameState.goal || GameState.P2Score == GameState.goal)
        {
            GameOver = true;
            mainMenuButton.interactable = true;
            mainMenuButtonObject.SetActive(true);

            if (GameState.P1Score == GameState.goal && GameState.P2Score == GameState.goal)
                playerDestroyedText.text = "Draw!";
            else if (GameState.P1Score == GameState.goal)
            {
                playerDestroyedText.text = "Player 1 wins!";
                playerDestroyedText.color = Color.blue;
            }
            else if (GameState.P2Score == GameState.goal)
            {
                playerDestroyedText.text = "Player 2 wins!";
                playerDestroyedText.color = Color.red;
            }

            GameState.P1Score = 0;
            GameState.P2Score = 0;
        }
        
        // Start the coroutine to load the main menu scene after 5 seconds
        if(!GameOver){
            StartCoroutine(LoadMainMenuAfterDelay(2f));

            if (destroyedPlayerID == 3)
            {
                playerDestroyedText.text = "Both players destroyed";
               // playerDestroyedText.color = Color.purple;
            }
            else
            {
                playerDestroyedText.text = "Player " + destroyedPlayerID + " destroyed";

                if (destroyedPlayerID == 1)
                {
                    playerDestroyedText.color = Color.red;
                } else
                {
                    playerDestroyedText.color = Color.blue;
                }
            }
                
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
