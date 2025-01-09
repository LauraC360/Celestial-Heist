using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingPoint : MonoBehaviour
{

    public int index;
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<SpaceshipController>())
        {
            SpaceshipController ship = collision.gameObject.GetComponent<SpaceshipController>();
            if(ship.chekpointIndex == index - 1)
            {
                ship.chekpointIndex = index;
            }
        }
    }
}
