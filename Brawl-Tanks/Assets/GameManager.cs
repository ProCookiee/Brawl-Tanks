using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    void Start()
    {
        Instantiate(player1, new Vector3(-4, 0, 0), Quaternion.identity);
        Instantiate(player2, new Vector3(4, 0, 0), Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
