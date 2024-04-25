using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_LWind : MonoBehaviour
{
    public GameObject Wind;
    private ParticleSystem psWind;

    public float WindLength = 1f;
    public float WindSize = 1f;
    public float WindStrenght;

    public bool onlyAfectParavela = false;

    public float particleLifeSpan;

    void Start()
    {
        psWind = Wind.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        SetShapeToSize(WindLength,WindSize);
        ModifySize();
    }

    void ModifySize()
    {
        var main = psWind.main;
        var shapeModule = psWind.shape;
        particleLifeSpan = (transform.localScale.x*4)/100;
        
        main.startLifetime = particleLifeSpan;
        shapeModule.radius = transform.localScale.y;
    }

    void SetShapeToSize(float length,float rad)
    {
        transform.localScale = new Vector3(length,rad,1);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = col.gameObject.GetComponent<Rigidbody2D>();
            col.gameObject.GetComponent<S_CMovement>().extraWindInertia = 5;
            if (playerRb != null)
            {
                Vector2 direction = -transform.right;
                if(onlyAfectParavela)
                {
                    if(col.gameObject.GetComponent<S_CMovement>().parachute)
                    {
                        playerRb.velocity += direction * -WindStrenght;
                    }
                }
                else
                {
                    playerRb.velocity += direction * -WindStrenght;
                }               
            }
        }
        else
        {
            col.gameObject.GetComponent<S_CMovement>().extraWindInertia = 0;
        }
    }
}
