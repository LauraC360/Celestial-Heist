using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SolarSystem : MonoBehaviour
{
    [SerializeField] private GameObject planet;
    [SerializeField] private float minRadius;
    [SerializeField] private float radiusDistance;
    [SerializeField] private int minPlanets;
    [SerializeField] private int maxPlanets;
    [SerializeField] private float planetProtectorGenerationChance;

    private void Start()
    {
        transform.rotation = Random.rotation;
        
        var planetCount = Random.Range(minPlanets, maxPlanets);
        var radius = minRadius;
        for (int i = 0; i < planetCount; i++)
        {
            var newPlanet = World.Instance.Instantiate(planet);
            newPlanet.transform.SetParent(transform);

            var circlePos = Random.insideUnitCircle.normalized * radius;
            newPlanet.transform.localPosition = new Vector3(circlePos.x, 0, circlePos.y);
            
            newPlanet.GetComponent<Planet>().Setup(i, transform);

            radius += radiusDistance;
            
            if (Random.Range(0f, 1f) < planetProtectorGenerationChance)
                transform.GetChild(i).GetComponent<PlanetProtectorGenerator>().GeneratePlanetProtectors();
        }
    }
}
