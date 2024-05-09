using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_LMovingPlatform : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
    Vector3 targetPos;
    public int StopTime;
    public int timeToMove;


    private void Start()
    {
        targetPos = posB.position;
        timeToMove = StopTime;
    }         

    private void Update()
    {
        timeToMove--;
        if(Vector2.Distance(transform.position, posA.position) < 0.05f)
        {
            if(timeToMove < 0 && targetPos != posB.position)
            {
                timeToMove = StopTime;
            }
            targetPos = posB.position;
        }   
                                                                         
        if(Vector2.Distance(transform.position, posB.position) < 0.05f)
        {
            if(timeToMove < 0 && targetPos != posA.position)
            {
                timeToMove = StopTime;
            }
            targetPos = posA.position;
        }
        
        if(timeToMove < 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.transform.parent = this.transform;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Input.GetButton("Fire1"))
        {
            collision.transform.parent = GameObject.Find("CharacterController").transform;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = GameObject.Find("CharacterController").transform;
        }

    }




}
