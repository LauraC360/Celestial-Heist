using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetProtectorRadar : MonoBehaviour
{
    public SpaceshipController playerSpaceship = null;
    public Action onPlayerSpaceshipChanged;

    private void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody == null)
            return;
        
        var spaceship = other.attachedRigidbody.GetComponent<SpaceshipController>();
        if (spaceship != null && spaceship.gameObject.CompareTag("Player"))
        {
            playerSpaceship = spaceship;
            onPlayerSpaceshipChanged?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.attachedRigidbody == null)
            return;
        
        var spaceship = other.attachedRigidbody.GetComponent<SpaceshipController>();
        if (spaceship != null && spaceship.gameObject.CompareTag("Player"))
        {
            playerSpaceship = null;
            onPlayerSpaceshipChanged?.Invoke();
        }
    }
}
