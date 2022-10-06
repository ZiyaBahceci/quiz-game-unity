using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayRandomButton : MonoBehaviour
{
    public Button button;
    
    private void Awake()
    {
        button.onClick.AddListener(() => PlayerPrefs.DeleteKey("category"));
    }
}