using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void Play()
    {
        S_LevelLoader transition = GameObject.Find("LevelLoader").GetComponent<S_LevelLoader>();
        transition.CallPass(SceneManager.GetActiveScene().buildIndex + 1);
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

}
