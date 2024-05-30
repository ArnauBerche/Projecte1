using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public GameObject textLine;
    public GameObject LVL1;
    public GameObject LVL2;
    public GameObject LVL3;
    public GameObject LVL4;

    public GameObject LVL1B;
    public GameObject LVL2B;
    public GameObject LVL3B;
    public GameObject LVL4B;
    

    // Start is called before the first frame update
    public void Update()
    {
        if(PlayerPrefs.GetInt("DevMode") != 1)
        {
            textLine.SetActive(true);
            LVL1B.SetActive(true);
            LVL2B.SetActive(true);
            LVL3B.SetActive(true);
            LVL4B.SetActive(true);
            LVL1.SetActive(false);
            LVL2.SetActive(false);
            LVL3.SetActive(false);
            LVL4.SetActive(false);
        }
        else
        {
            textLine.SetActive(false);
            LVL1B.SetActive(false);
            LVL2B.SetActive(false);
            LVL3B.SetActive(false);
            LVL4B.SetActive(false);
            LVL1.SetActive(true);
            LVL2.SetActive(true);
            LVL3.SetActive(true);
            LVL4.SetActive(true);
        }

    }
}
