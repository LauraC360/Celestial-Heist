using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LapManager : MonoBehaviour
{
    public List<RacingPoint> racingPoints;
    public int totalLaps;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<SpaceshipController>())
        {
            SpaceshipController ship = collision.gameObject.GetComponent<SpaceshipController>();
            if (ship.chekpointIndex == racingPoints.Count)
            {
                ship.chekpointIndex = 0;
                ship.lapNumber++;
                if (ship.lapNumber >= totalLaps)
                {
                    //TODO: STOP racing
                }
            }
        }
    }
}