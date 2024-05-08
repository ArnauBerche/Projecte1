using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_ButtonsInGame : MonoBehaviour
{

    public void BackMenu()
    {
        S_LevelLoader transition = GameObject.Find("LevelLoader").GetComponent<S_LevelLoader>();
        transition.CallPass(0);
    }
   
}
