using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject devBON;
    public GameObject devBOFF;

    public void Awake()
    {
        if(PlayerPrefs.GetInt("DevMode") != 1)
        {
            devBON.SetActive(false);
            devBOFF.SetActive(true);
        }
        else
        {
            devBON.SetActive(true);
            devBOFF.SetActive(false);       
        }

    }

    public void Play()
    {
        if(PlayerPrefs.GetInt("DevMode") != 1)
        {
            S_LevelLoader transition = GameObject.Find("LevelLoader").GetComponent<S_LevelLoader>();
            transition.CallPass(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            S_LevelLoader transition = GameObject.Find("LevelLoader").GetComponent<S_LevelLoader>();
            transition.CallPass(2);
        }

    }

    public void BTM()
    {
        S_LevelLoader transition = GameObject.Find("LevelLoader").GetComponent<S_LevelLoader>();
        transition.CallPass(0);
    }

    public void Quit()
    {
        Debug.Log("Quit...");
        //Aquest boto funcionara cuan tinguem la aplicacio del joc creada
        Application.Quit();
    }

    public void DEV()
    {
        if(PlayerPrefs.GetInt("DevMode") == 1)
        {
            PlayerPrefs.SetInt("DevMode",0);
        }
        else
        {
            PlayerPrefs.SetInt("DevMode",1);
        }
        Debug.Log(PlayerPrefs.GetInt("DevMode"));
    }

    public void LoadLVL1()
    {
        S_LevelLoader transition = GameObject.Find("LevelLoader").GetComponent<S_LevelLoader>();
        transition.CallPass(1);
    }

    public void LoadLVL2()
    {
        S_LevelLoader transition = GameObject.Find("LevelLoader").GetComponent<S_LevelLoader>();
        transition.CallPass(2);
    }

    public void LoadLVL3()
    {
        S_LevelLoader transition = GameObject.Find("LevelLoader").GetComponent<S_LevelLoader>();
        transition.CallPass(4);
    }

    public void LoadLVL4()
    {
        S_LevelLoader transition = GameObject.Find("LevelLoader").GetComponent<S_LevelLoader>();
        transition.CallPass(5);
    }

}
