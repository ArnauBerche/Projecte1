using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CCursor : MonoBehaviour
{
    public S_CHook grapplingGun;
    public Transform Main;
    public Transform Center;
    public bool onStay = false;

    public Vector3 closePointToMain;
    public Vector3 closePointToCenter;
    public Vector3 miPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerStay2D(Collider2D collider)
    {
        onStay = true;
        if (collider.tag != "Player") 
        {
            closePointToCenter = collider.ClosestPoint(Center.position);
            closePointToMain = collider.ClosestPoint(closePointToCenter);
            
            miPoint = closePointToMain;
            Debug.Log(miPoint);
        }  
    }
    public void OnTriggerExit2D(Collider2D collider)
    {
        onStay = false;
    }
}
