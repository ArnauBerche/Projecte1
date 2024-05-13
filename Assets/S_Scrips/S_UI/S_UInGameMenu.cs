using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public Canvas canvas;

    void Start()
    {
        canvas.enabled = false;
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.enabled = !canvas.enabled;
        }
    }
}
