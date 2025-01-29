using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShapeSettings", menuName = "ScriptableObjects/ShapeSettings", order = 1)]
public class ColourSettings : ScriptableObject {

    public Gradient gradient;
    public Material planetMaterial;
} 