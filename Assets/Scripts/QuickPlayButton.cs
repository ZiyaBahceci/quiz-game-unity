using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickPlayButton : MonoBehaviour
{
    public void OnEnable()
    {
        if (!PlayerPrefs.HasKey("category"))
        {   
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
    }
}
