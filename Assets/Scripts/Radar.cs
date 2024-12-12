using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField] private GameObject parent;

    public List<EnemySpaceship> spaceships = new List<EnemySpaceship>();
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody == null)
            return;
        
        var spaceship = other.attachedRigidbody.GetComponent<EnemySpaceship>();
        if(spaceship != null && !spaceships.Contains(spaceship))
            spaceships.Add(spaceship);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.attachedRigidbody == null)
            return;
        
        var spaceship = other.attachedRigidbody.GetComponent<EnemySpaceship>();
        if(spaceship != null)
            spaceships.Remove(spaceship);
    }

    private void Update()
    {
        for(int i = 0; i < spaceships.Count; i++)
            if (spaceships[i] == null)
            {
                spaceships.RemoveAt(i);
                i--;
            }
    }
}
