using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Parallax : MonoBehaviour
{
    private float lenght,startPos;
    public GameObject cam;
    private GameObject SnowMountains;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        SnowMountains = GameObject.Find("SnowMountains");
        startPos = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (cam.transform.position.x * (1.1f - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startPos + dist, transform.position.y,transform.position.z);
            if(temp > startPos+lenght)
            {
                startPos += lenght;
            }
            else if(temp < startPos-lenght)
            {
                startPos -= lenght;
            }
    }
}
