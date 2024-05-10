using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_LEnableComponents : MonoBehaviour
{
    public bool hook;
    public bool parachute;
    public S_CMovement movScript;
    public void OnTriggerEnter2D(Collider2D col)
    {
        if(parachute)
        {
            movScript.parachuteIsEnabled = true;
        }
        if(hook)
        {
            movScript.HookIsEnabled = true;
        }
        gameObject.SetActive(false);
    }
}
