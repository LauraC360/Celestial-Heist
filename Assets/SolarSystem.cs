using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class SolarSystem : MonoBehaviour
{
    [SerializeField] private Shader litShaderGraphics;
    [SerializeField] private CollectiblesGenerator collectiblesGenerator;
    [SerializeField] private GameObject planet;
    [SerializeField] private float minRadius;
    [SerializeField] private float radiusDistance;
    [SerializeField] private int minPlanets;
    [SerializeField] private int maxPlanets;
    [SerializeField] private int minPlanetRadius;
    [SerializeField] private int maxPlanetRadius;
    [SerializeField] private int minPlanetResolution;
    [SerializeField] private int maxPlanetResolution;
    [SerializeField] private float planetProtectorGenerationChance;
    
    [HideInInspector]
    public Queue<Tuple<GameObject, float>> planetQueue = new Queue<Tuple<GameObject, float>>();

    private void Start()
    {
        transform.rotation = Random.rotation;
        
        var planetCount = Random.Range(minPlanets, maxPlanets);
        var radius = minRadius;
        var maxRadius = minRadius + radiusDistance * (planetCount - 1);
        for (int i = 0; i < planetCount; i++)
        {
            var newPlanet = World.Instance.Instantiate(planet);
            
            newPlanet.transform.SetParent(transform);

            var circlePos = Random.insideUnitCircle.normalized * radius;
            newPlanet.transform.localPosition = new Vector3(circlePos.x, 0, circlePos.y);
            
            generateObject(newPlanet, newPlanet.transform.position, i);
            
            newPlanet.GetComponent<PlanetRotator>().Setup(i, transform, maxRadius);

            radius += radiusDistance;
            
            if (Random.Range(0f, 1f) < planetProtectorGenerationChance)
                transform.GetChild(i).GetComponent<PlanetProtectorGenerator>().GeneratePlanetProtectors();
        }
        
        collectiblesGenerator.planetQueue = planetQueue;
        collectiblesGenerator.GenerateCollectibles();
    }
    
    void generateObject(GameObject currentPlanet, Vector3 position, int planet_id)
    {
        byte[] randomBytes = new byte[4];
        RandomNumberGenerator.Create().GetBytes(randomBytes);
        int cryptoSeed = BitConverter.ToInt32(randomBytes, 0);

        Random.InitState(cryptoSeed);
        
        Planet planetScript = currentPlanet.GetComponentInChildren<Planet>();
        planetScript.resolution = Random.Range(minPlanetResolution, maxPlanetResolution);
        planetScript.origin = position;

        ShapeSettings shapeSettings = ScriptableObject.CreateInstance<ShapeSettings>();
        shapeSettings.planetRadius = Random.Range(minPlanetRadius, maxPlanetRadius); // Set a random planet radius
        shapeSettings.noiseLayers = GenerateRandomNoiseLayers(); // Generate random noise layers
        planetScript.shapeSettings = shapeSettings;

        ColourSettings colourSettings = ScriptableObject.CreateInstance<ColourSettings>();
        colourSettings.planetMaterial = new Material(litShaderGraphics); // Assign a default material
        colourSettings.gradient = generateRandomGradient(); // Generate a random gradient
        planetScript.colourSettings = colourSettings;

        planetScript.GeneratePlanet(planet_id);
        
        currentPlanet.GetComponentInChildren<PlanetGravity>().Setup(shapeSettings.planetRadius, (shapeSettings.planetRadius - minPlanetRadius) / (maxPlanetRadius - minPlanetRadius));

        planetQueue.Enqueue(new Tuple<GameObject, float>(currentPlanet, shapeSettings.planetRadius));
    }
    
    private ShapeSettings.NoiseLayer[] GenerateRandomNoiseLayers()
    {
        int numLayers = Random.Range(3, 7); // Generate a random number between 3 and 6
        ShapeSettings.NoiseLayer[] noiseLayers = new ShapeSettings.NoiseLayer[numLayers];

        for (int i = 0; i < numLayers; i++)
        {
            noiseLayers[i] = new ShapeSettings.NoiseLayer();
            noiseLayers[i].enabled = true;
            noiseLayers[i].useFirstLayerAsMask = Random.value > 0.5f;
            noiseLayers[i].noiseSettings = new NoiseSettings();
            noiseLayers[i].noiseSettings.filterType = (NoiseSettings.FilterType)Random.Range(0, 2); // Randomly select between Simple and Ridgid

            float strength = Random.Range(0.1f, 1f);
            float baseRoughness = Random.Range(0.1f, i * 2f + 1f);
            float persistence = Random.Range(0.1f, 0.3f);
            float minMinValue = (strength + persistence) * 0.6f;
            float maxMinValue = (strength + persistence) * 0.8f;

            if (noiseLayers[i].noiseSettings.filterType == NoiseSettings.FilterType.Simple)
            {
                noiseLayers[i].noiseSettings.simpleNoiseSettings = new NoiseSettings.SimpleNoiseSettings
                {
                    strength = strength,
                    numLayers = Random.Range(2, 5),
                    baseRoughness = baseRoughness,
                    roughness = Random.Range(0.1f, 5f),
                    persistence = persistence,
                    minValue = Random.Range(minMinValue, maxMinValue),
                    centre = new Vector3(0, 0, 0)
                };
            }
            else
            {
                noiseLayers[i].noiseSettings.ridgidNoiseSettings = new NoiseSettings.RidgidNoiseSettings
                {
                    strength = strength,
                    numLayers = Random.Range(1, 8),
                    baseRoughness = baseRoughness,
                    roughness = Random.Range(0.1f, 5f),
                    persistence = persistence,
                    centre = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)),
                    minValue = Random.Range(minMinValue * 2, maxMinValue * 4),
                    weightMultiplier = Random.Range(0.1f, 0.5f)
                };
            }
        }

        return noiseLayers;
    }

    private Gradient generateRandomGradient()
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[5];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[5];

        for (int i = 0; i < 5; i++)
        {
            colorKeys[i].color = new Color(Random.value, Random.value, Random.value);
            colorKeys[i].time = i / 4f; // Spread the color keys evenly across the gradient

            alphaKeys[i].alpha = Random.value;
            alphaKeys[i].time = i / 4f; // Spread the alpha keys evenly across the gradient
        }

        gradient.SetKeys(colorKeys, alphaKeys);

        return gradient;
    }
}
