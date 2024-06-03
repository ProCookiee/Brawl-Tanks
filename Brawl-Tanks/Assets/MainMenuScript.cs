using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private int goal;

    MainMenuScript()
    {
        goal = 5;
    }

    public void UpdateRounds(string inputString)
    {
        int newGoal;

        if (Int32.TryParse(inputString, out newGoal))
        {
            if (newGoal <= 0)
            {
                newGoal = 1;
            }
        } else
        {
            newGoal = 1;
        }

        goal = newGoal;
        Debug.Log(goal);
    }

    public void StartGame(string gamemode)
    {
        GameState.goal = goal;
        GameState.gamemode = gamemode;

        if (gamemode == "Singleplayer")
        {
            SceneManager.LoadScene("Survival");
            return;
        }

        SceneManager.LoadScene("DeathMatch");
    }
}
