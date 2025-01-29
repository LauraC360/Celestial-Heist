using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private int totalLaps = 1;
    public List<SpaceshipController> ships = new List<SpaceshipController>();
    private Dictionary<SpaceshipController, float> shipTimes = new Dictionary<SpaceshipController, float>();
    private bool raceFinished = false;

    private void Start()
    {
        //ships = FindObjectsOfType<SpaceshipController>().ToList();

        foreach (SpaceshipController ship in ships)
        {
            shipTimes[ship] = 0f;
        }

        Debug.LogWarning($"The race started with {ships.Count} ships.");
        StartCoroutine(UpdateRaceTimes());
    }

    private IEnumerator UpdateRaceTimes()
    {
        while (!raceFinished)
        {
            yield return new WaitForSeconds(1f);

            foreach (SpaceshipController ship in ships)
            {
                if (!raceFinished)
                {
                    shipTimes[ship] += 1f;
                }
            }
        }
    }

    public void OnShipFinishLap(SpaceshipController ship)
    {
        if (raceFinished) return;

        raceFinished = true;
        Debug.LogWarning($"Chequered flag! {ship.name} wins with a time of {shipTimes[ship]} seconds.");

        var sortedResults = shipTimes.OrderBy(x => x.Value).ToList();
        Debug.LogWarning("Final Standings:");

        foreach (var result in sortedResults)
        {
            Debug.LogWarning($"{result.Key.name}: {result.Value} seconds");
        }
    }
}
