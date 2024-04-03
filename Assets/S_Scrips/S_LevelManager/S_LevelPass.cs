using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_LevelPass : MonoBehaviour
{
    public int id = 0;
    void OnTriggerEnter2D(Collider2D col) 
    {
        if(col.gameObject.tag == "Player") 
        {
            if (id == 0) 
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
