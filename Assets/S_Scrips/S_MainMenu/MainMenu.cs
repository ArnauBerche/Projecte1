using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {

        Debug.Log("Quit...");
        //Aquest boto funcionara cuan tinguem la aplicacio del joc creada
        Application.Quit();

    }

}
