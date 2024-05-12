using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FiveSecondTimer());
    }

    // Update is called once per frame
    IEnumerator FiveSecondTimer()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
