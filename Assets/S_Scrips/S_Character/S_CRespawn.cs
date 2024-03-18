using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CRespawn : MonoBehaviour
{
    private S_CMovement mainChar;

    // Start is called before the first frame update
    void Start()
    {
        mainChar = GameObject.Find("MainCharacter").GetComponent<S_CMovement>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        mainChar.respawnPoint = transform.position;
    }
}
