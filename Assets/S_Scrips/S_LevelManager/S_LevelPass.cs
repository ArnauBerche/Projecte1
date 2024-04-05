using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_LevelPass : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col) 
    {
        if(col.gameObject.tag == "Player") 
        {
            S_LevelLoader transition = GameObject.Find("LevelLoader").GetComponent<S_LevelLoader>();
            transition.CallPass(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
