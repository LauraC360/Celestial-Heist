using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CollectiblesGenerator : MonoBehaviour
{
    public List<GameObject> collectibles = new List<GameObject>();

    // PlanetGenerator planetGenerator;
    [HideInInspector]
    public Queue<Tuple<GameObject, float>> planetQueue;
    

    const int MIN_COLLECTIBLES_PER_TYPE = 30;
    const int MAX_COLLECTIBLES_PER_TYPE = 50;

    // void Start()
    // {
    //     // planetGenerator = GameObject.Find("PlanetGenerator").GetComponent<PlanetGenerator>();
    //     planetQueue = planetGenerator.planetQueue;
    // }

    public void GenerateCollectibles()
    {
        int id=0;
        foreach (Tuple<GameObject, float> planet in planetQueue)
        {
            GameObject currentPlanet = planet.Item1;
            float planetRadius = planet.Item2;
            Planet planetScript = currentPlanet.GetComponent<Planet>();

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
                collectible.transform.parent = currentPlanet.transform;

                collectible.AddComponent<Collectible>();
                collectible.GetComponent<Collectible>().Initialize(id++);
            }
        }
    }
}
