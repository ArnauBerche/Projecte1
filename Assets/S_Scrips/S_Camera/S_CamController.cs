using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CamController : MonoBehaviour
{

    [SerializeField] private int dynamicList;
    [SerializeField] private GameObject[] instantiatedObjects;
    [SerializeField] private GameObject mainChar;
    [SerializeField] float distance;
    private void Start()
    {
        Fill();
    }

    public void Fill()
    {
        instantiatedObjects = new GameObject[dynamicList];
        for (int i = 0; i < dynamicList; i++)
        {
            instantiatedObjects[i] = GameObject.Find("CamPos" + i.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        distance = (mainChar.transform.position - instantiatedObjects[0].transform.position).magnitude;
        distance.Normalize();
        transform.position = instantiatedObjects[0].transform.position;
    }
}
