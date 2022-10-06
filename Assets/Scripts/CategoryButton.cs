using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    private string id;
    
    public TMP_Text text;

    public Button button;

    private void Start()
    {
        button.onClick.AddListener(() => PlayerPrefs.SetString("category", id));
    }

    public void Setup(string label, string id, UnityAction onClick)
    {
       text.text = label;
       this.id = id;
       
       button.onClick.AddListener(onClick);
    }
}
