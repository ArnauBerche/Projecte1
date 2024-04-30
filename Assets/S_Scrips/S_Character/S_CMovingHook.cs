using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CMovingHook : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("hi");
        if(collision.transform.tag == "MP")
        {
            Debug.Log("hi2");
        }

    }
}
