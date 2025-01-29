using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMover : MonoBehaviour
{
    private Rigidbody[] rigidbodies;
    
    private void FixedUpdate()
    { 
        var delta = transform.position;
        World.Instance.Move(-delta);
        transform.position = Vector3.zero;
    }
}
