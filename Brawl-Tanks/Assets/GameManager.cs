using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI gamemodeText;

    public GameObject player1;
    public GameObject player2;

    void Start()
    {
        goalText.text = "First to: " + GameState.goal.ToString();
        gamemodeText.text = GameState.gamemode;

        Instantiate(player1, new Vector3(-4, 0, 0), Quaternion.identity);
        Instantiate(player2, new Vector3(4, 0, 0), Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
