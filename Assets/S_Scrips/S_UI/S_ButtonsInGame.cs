using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Not Optimaized

public class S_ButtonsInGame : MonoBehaviour
{
    private Toggle Slow;
    private Toggle Range;
    private Toggle Para;

    public void Awake()
    {
        Slow = GameObject.Find("Slow").GetComponent<Toggle>();
        if(PlayerPrefs.GetInt("InfiniteSlowMo") != 1)
        {
            Slow.isOn = false;
        }
        else
        {
            Slow.isOn = true;      
        }

        Range = GameObject.Find("Range").GetComponent<Toggle>();
        if(PlayerPrefs.GetInt("ShowRangeHook") != 1)
        {
            Range.isOn = false;
        }
        else
        {
            Range.isOn = true;      
        }
        
        Para = GameObject.Find("Para").GetComponent<Toggle>();
        if(PlayerPrefs.GetInt("FloatingParavela") != 1)
        {
            Para.isOn = false;
        }
        else
        {
            Para.isOn = true;      
        }
    }

    void Update()
    {
        if(Slow.isOn)
        {
            SetInfiniteSlowMo(true);
        }
        else
        {
            SetInfiniteSlowMo(false);
        }

        if(Range.isOn)
        {
            SetInfiniteRangeHook(true);
        }
        else
        {
            SetInfiniteRangeHook(false);
        }

        if(Para.isOn)
        {
            SetFloatingParavela(true);
        }
        else
        {
            SetFloatingParavela(false);
        }
    }

    public void BackMenu()
    {
        S_LevelLoader transition = GameObject.Find("LevelLoader").GetComponent<S_LevelLoader>();
        transition.CallPass(0);
    }

    public void SetInfiniteRangeHook(bool value)
    {
        if(value)
        {
            PlayerPrefs.SetInt("ShowRangeHook",1);
        }
        else
        {
            PlayerPrefs.SetInt("ShowRangeHook",0);
        }
    }

    public void SetInfiniteSlowMo(bool value)
    {

        if(value)
        {
            PlayerPrefs.SetInt("InfiniteSlowMo",1);
        }
        else
        {
            PlayerPrefs.SetInt("InfiniteSlowMo",0);
        }  
    }

    public void SetFloatingParavela(bool value)
    {
        if(value)
        {
            PlayerPrefs.SetInt("FloatingParavela",1);
        }
        else
        {
            PlayerPrefs.SetInt("FloatingParavela",0);
        }  
    }
   
}
