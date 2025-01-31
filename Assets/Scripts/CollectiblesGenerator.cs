using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollectiblesGenerator : MonoBehaviour
{
    [SerializeField] private SolarSystem solarSystem;
    
    [HideInInspector]
    public Queue<Tuple<GameObject, float>> planetQueue;

    public List<GameObject> collectibles = new List<GameObject>();

    const int MIN_COLLECTIBLES_PER_TYPE = 5;
    const int MAX_COLLECTIBLES_PER_TYPE = 10;
    
    public void GenerateCollectibles()
    {
        planetQueue = solarSystem.planetQueue;
        
        int id=0;
        foreach (Tuple<GameObject, float> planet in planetQueue)
        {
            GameObject currentPlanet = planet.Item1;
            float planetRadius = planet.Item2;
            Planet planetScript = currentPlanet.GetComponentInChildren<Planet>();

            for (int i = 0; i < Random.Range(MIN_COLLECTIBLES_PER_TYPE, MAX_COLLECTIBLES_PER_TYPE); i++)
            {
                Vector3 position = Random.onUnitSphere * planetRadius;
                Vector3 surfacePosition = currentPlanet.transform.position + position;

                float surfaceHeight = planetScript.GetSurfaceHeight(surfacePosition, id);

                if (surfacePosition.magnitude < surfaceHeight)
                {
                    surfacePosition = surfacePosition.normalized * surfaceHeight; 
                }

                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, position.normalized);
                GameObject collectible = Instantiate(collectibles[Random.Range(0, collectibles.Count)], surfacePosition, rotation);
                collectible.name = $"Collectible_{id++}";
                collectible.tag = "Collectible";
                collectible.transform.parent = currentPlanet.transform;
            }
        }
    }
}
