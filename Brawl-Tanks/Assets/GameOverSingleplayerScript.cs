using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverSingleplayerScript : MonoBehaviour
{
    // Make the scoreText field public so it can be assigned in the Inspector
    public TextMeshProUGUI scoreText;
    private string mainMenuSceneName = "MainMenu";
    private string gameSceneName = "Survival";

    void Start()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + GameState.survivalScore.ToString();
        }
        else
        {
            Debug.LogError("Score Text is not assigned in the Inspector.");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
