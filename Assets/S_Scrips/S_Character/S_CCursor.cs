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
    public float distance;

    public GameObject DCenter;
    public GameObject DClose;
    public GameObject DMidle;

    [Range(0f, 0.5f)][SerializeField] private float cursorForgivenes = 0.5f;

    // Start is called before the first frame update

    private void Update() 
    {
        distance = Vector2.Distance(Main.position, Center.position);
        DCenter.transform.position = closePointToCenter;
        DClose.transform.position = closePointToMain;
        DMidle.transform.position = miPoint;
        if (distance > 7)
        {
            miPoint = Vector3.Lerp(closePointToCenter, closePointToMain, cursorForgivenes);
        }
        else
        {
            miPoint = Center.position;
        }
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        onStay = true;
        if (collider.tag != "Player") 
        {
            closePointToCenter = collider.ClosestPoint(Center.position);
            closePointToMain = collider.ClosestPoint(Main.position);
        }  
    }
    public void OnTriggerExit2D(Collider2D collider)
    {
        onStay = false;
    }
}
