using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VRLog : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    
    public static VRLog instance;

    private void Awake()
    {
        instance = this;
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
