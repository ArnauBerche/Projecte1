using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CMovingHook : MonoBehaviour
{
    public S_CHook hook;
    public float enterDistance;

    void Update()
    {
        if(Input.GetAxisRaw("Vertical") > 0 && enterDistance > hook.minDistance)
        {
            enterDistance -= 5 * Time.deltaTime;
        }
        else if(Input.GetAxisRaw("Vertical") < 0 && enterDistance < hook.maxDistance)
        {
            enterDistance += 5 * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        enterDistance = hook.m_springJoint2D.distance;
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        
        if(collision.gameObject.tag == "MP")
        {
            hook.grapplePoint = collision.gameObject.transform.position;
            hook.m_springJoint2D.connectedAnchor = hook.grapplePoint;
            hook.m_springJoint2D.distance = enterDistance;
            gameObject.transform.position = hook.grapplePoint;
        }

    }
}
