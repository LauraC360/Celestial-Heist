using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField] private SpaceshipController parent;

    public List<SpaceshipController> spaceships = new List<SpaceshipController>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody == null)
            return;
        
        var spaceship = other.attachedRigidbody.GetComponent<SpaceshipController>();
        if(spaceship != null && spaceship.Team != parent.Team && !spaceships.Contains(spaceship))
            spaceships.Add(spaceship);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.attachedRigidbody == null)
            return;
        
        var spaceship = other.attachedRigidbody.GetComponent<SpaceshipController>();
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
