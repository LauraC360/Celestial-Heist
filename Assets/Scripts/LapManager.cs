using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    public List<RacingPoint> racingPoints;
    public int totalLaps;
    private RaceManager raceManager;

    private void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        SpaceshipController ship = collision.gameObject.GetComponent<SpaceshipController>();

        if (ship != null && ship.chekpointIndex == racingPoints.Count)
        {
            ship.chekpointIndex = 0;
            ship.lapNumber++;

            Debug.LogWarning($"{ship.name} finished lap {ship.lapNumber}/{totalLaps}");

            if (ship.lapNumber >= totalLaps)
            {
                Debug.LogWarning($"{ship.name} finished the race!");
                raceManager.OnShipFinishLap(ship);
            }
        }
    }
}
